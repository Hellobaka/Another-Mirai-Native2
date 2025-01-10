using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Another_Mirai_Native.UI.Pages
{
    /// <summary>
    /// Test_APIPage.xaml 的交互逻辑
    /// </summary>
    public partial class Test_APIPage : Page, INotifyPropertyChanged
    {
        public Test_APIPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public List<ProtocolMethod> DisplayProtocolMethods { get; set; } = [];

        public bool FormLoaded { get; set; }

        public List<InvokeArugment> InvokeArugments { get; set; } = [];

        public List<ProtocolMethod> ProtocolMethods { get; set; } = [];

        public List<TestPage.Remark> Remarks { get; set; } = [];

        private ProtocolMethod CurrentMethod { get; set; }

        private Debouncer SearchDebounder { get; set; } = new();

        /// <summary>
        /// MVVM
        /// </summary>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void BuildArgumentList()
        {
            ArgumentList.Children.Clear();
            foreach (var item in InvokeArugments)
            {
                ArgumentList.Children.Add(new TextBlock() { Text = item.Name, Margin = new Thickness(0, 5, 0, 0) });
                TextBox inputTextbox = new TextBox() { Tag = item, Text = item.Value, Margin = new Thickness(0, 3, 0, 0) };
                inputTextbox.TextChanged += (sender, _) =>
                {
                    if (sender is TextBox textBox && textBox.Tag is InvokeArugment arugment)
                    {
                        arugment.Value = textBox.Text;
                    }
                };
                ArgumentList.Children.Add(inputTextbox);
            }

            EmptyHint.Visibility = ArgumentList.Children.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void FilterMethods()
        {
            Dispatcher.BeginInvoke(() =>
            {
                DisplayProtocolMethods = ProtocolMethods.Where(x => x.MethodName.ToLower().Contains(SearchTextValue.Text.ToLower())
                                                                || x.MethodDescription.ToLower().Contains(SearchTextValue.Text.ToLower())).ToList();
                OnPropertyChanged(nameof(DisplayProtocolMethods));
            });
        }

        private async void InvokeButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentMethod == null || CurrentMethod.Method == null)
            {
                DialogHelper.ShowSimpleDialog("调用测试方法", $"未选择测试方法");
                return;
            }
            List<object> arugments = [];
            foreach (var item in InvokeArugments)
            {
                item.Value = item.Value?.Trim() ?? "";
                try
                {
                    switch (item.TargetType.Name)
                    {
                        case "Int64":
                            arugments.Add(long.Parse(item.Value));
                            break;

                        case "Int32":
                            arugments.Add(int.Parse(item.Value));
                            break;

                        case "String":
                            arugments.Add(item.Value);
                            break;

                        case "Boolean":
                            arugments.Add(bool.Parse(item.Value));
                            break;

                        default:
                            break;
                    }
                }
                catch
                {
                    DialogHelper.ShowSimpleDialog("测试参数转换", $"参数转换失败:\n\n{item.Name} 的值 {item.Value} 无法转换为 {item.TargetType}");
                    return;
                }
            }

            if (!await DialogHelper.ShowConfirmDialog("调用测试方法", $"确定要调用测试方法 {CurrentMethod.MethodName} 吗？请二次确认输入的参数。"))
            {
                return;
            }
            CallStatus.Visibility = Visibility.Visible;
            InvokeButton.IsEnabled = false;
            FunctionInvokeResult.Text = "调用中...";

            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                object? ret = null;
                await Task.Run(() => ret = CurrentMethod.Method.Invoke(ProtocolManager.Instance.CurrentProtocol, arugments.ToArray()));
                string time = $" ({stopwatch.ElapsedMilliseconds} ms)";

                if (ret is IDictionary dict)
                {
                    StringBuilder sb = new();
                    foreach (DictionaryEntry item in dict)
                    {
                        sb.AppendLine($"{item.Key} = {item.Value}");
                    }
                    FunctionInvokeResult.Text = sb.ToString() + time;
                    return;
                }
                if (ret is IList list)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in list)
                    {
                        sb.AppendLine(item.ToString());
                    }
                    FunctionInvokeResult.Text = sb.ToString() + time;
                    return;
                }
                FunctionInvokeResult.Text = ret?.ToString() + time;
            }
            catch (Exception ex)
            {
                string time = $" ({stopwatch.ElapsedMilliseconds} ms)";

                DialogHelper.ShowSimpleDialog("调用测试方法", $"调用目标发生了异常: {ex.Message}\n{ex.StackTrace}");
                FunctionInvokeResult.Text = "调用发生了异常" + time;
            }
            finally
            {
                CallStatus.Visibility = Visibility.Collapsed;
                InvokeButton.IsEnabled = true;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (FormLoaded)
            {
                return;
            }
            FormLoaded = true;
            ProtocolMethods.Clear();

            Remarks = BuildRemarkList();
            var interfaceList = typeof(IProtocol).GetMethods();
            foreach (var item in ProtocolManager.Instance.CurrentProtocol.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance).OrderBy(x => x.Name))
            {
                if (!interfaceList.Any(x => x.Name == item.Name) || item.Name.StartsWith("get_") || item.Name.StartsWith("set_"))
                {
                    continue;
                }
                var remark = Remarks.FirstOrDefault(x => x.Name == item.Name);
                if (remark == null)
                {
                    continue;
                }
                var protocolMethod = new ProtocolMethod
                {
                    MethodName = item.Name,
                    MethodNameDisplay = remark.DisplayName,
                    ReturnDescription = $"{item.GetParameters().Length}个参数 返回值 {item.ReturnType.Name}",
                    Method = item
                };
                protocolMethod.MethodDescription = remark.SummaryDescription;
                ProtocolMethods.Add(protocolMethod);
            }
            DisplayProtocolMethods = [.. ProtocolMethods];
            OnPropertyChanged(nameof(DisplayProtocolMethods));
        }

        private void ProtocolMethodList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0 || e.AddedItems[0] is not ProtocolMethod method)
            {
                return;
            }
            CurrentMethod = method;
            InvokeArugments.Clear();
            var remark = Remarks.FirstOrDefault(x => x.Name == method.MethodName);
            if (remark != null)
            {
                MethodNameDisplay.Text = remark.Name;
                MethodDescDisplay.Text = remark.SummaryDescription;
                MethodReturnDescDisplay.Text = remark.ReturnDescription;
            }
            foreach (var item in method.Method.GetParameters())
            {
                if (item.Name == null)
                {
                    continue;
                }
                string desc = remark != null ? (remark.ArgumentList.TryGetValue(item.Name, out string? v) ? $" ({v})" : "") : "";

                InvokeArugments.Add(new InvokeArugment
                {
                    Name = item.Name + desc + $" ({item.ParameterType.Name})",
                    TargetType = item.ParameterType
                });
            }
            BuildArgumentList();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            BuildArgumentList();
        }

        private void SearchTextValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!FormLoaded)
            {
                return;
            }
            SearchDebounder.Debounce(FilterMethods, 500);
        }

        private List<TestPage.Remark> BuildRemarkList()
        {
            return [
                new TestPage.Remark()
                {
                    Name = "Connect",
                    DisplayName = "协议连接",
                    SummaryDescription = "与协议连接端进行连接动作",
                    ReturnDescription = "是否成功",
                    ArgumentList = new()
                    {
                    }
                },
                new TestPage.Remark()
                {
                    Name = "Disconnect",
                    DisplayName = "协议断开连接",
                    SummaryDescription = "主动与协议连接端断开连接, 通常指示着要切换协议, 此时应当中断所有线程以及定时器",
                    ReturnDescription = "是否断开成功",
                    ArgumentList = new()
                    {
                    }
                },
                new TestPage.Remark()
                {
                    Name = "GetConnectionConfig",
                    DisplayName = "获取连接参数",
                    SummaryDescription = "获取连接参数, Item 表示每个连接参数, Key为参数名称, Value为参数的值. 参数的值可以将保存的 值填入",
                    ReturnDescription = "连接参数",
                    ArgumentList = new()
                    {
                    }
                },
                new TestPage.Remark()
                {
                    Name = "SetConnectionConfig",
                    DisplayName = "设置连接参数",
                    SummaryDescription = "设置连接参数, 检查传入的  的每一项是否符合要求 若符合要求便保存并应用参数",
                    ReturnDescription = "是否检查通过",
                    ArgumentList = new()
                    {
                        { "config", "欲设置的连接参数" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "CanSendImage",
                    DisplayName = "协议是否支持发送图片",
                    SummaryDescription = "协议是否支持发送图片",
                    ReturnDescription = "1为 True; 0为 False",
                    ArgumentList = new()
                    {
                    }
                },
                new TestPage.Remark()
                {
                    Name = "CanSendRecord",
                    DisplayName = "协议是否支持发送语音",
                    SummaryDescription = "协议是否支持发送语音",
                    ReturnDescription = "1为 True; 0为 False",
                    ArgumentList = new()
                    {
                    }
                },
                new TestPage.Remark()
                {
                    Name = "DeleteMsg",
                    DisplayName = "撤回消息",
                    SummaryDescription = "撤回消息",
                    ReturnDescription = "1为 成功; 0为 失败",
                    ArgumentList = new()
                    {
                        { "msgId", "消息 ID" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "GetCookies",
                    DisplayName = "获取指定域名的 Cookie",
                    SummaryDescription = "获取指定域名的 Cookie",
                    ReturnDescription = "Cookie",
                    ArgumentList = new()
                    {
                        { "domain", "指定域名" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "GetCsrfToken",
                    DisplayName = "获取 CsrfToken",
                    SummaryDescription = "获取 CsrfToken",
                    ReturnDescription = "CsrfToken",
                    ArgumentList = new()
                    {
                    }
                },
                new TestPage.Remark()
                {
                    Name = "GetFriendList",
                    DisplayName = "获取序列化后的好友列表",
                    SummaryDescription = "获取序列化后的好友列表",
                    ReturnDescription = "序列化后的好友列表",
                    ArgumentList = new()
                    {
                        { "reserved", "是否倒序" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "GetGroupInfo",
                    DisplayName = "获取序列化后的群组信息",
                    SummaryDescription = "获取序列化后的群组信息",
                    ReturnDescription = "序列化后的群组信息",
                    ArgumentList = new()
                    {
                        { "groupId", "群 ID" },
                        { "notCache", "不使用缓存" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "GetGroupList",
                    DisplayName = "获取序列化后的群组列表",
                    SummaryDescription = "获取序列化后的群组列表",
                    ReturnDescription = "序列化后的群组列表",
                    ArgumentList = new()
                    {
                    }
                },
                new TestPage.Remark()
                {
                    Name = "GetGroupMemberInfo",
                    DisplayName = "获取序列化后的群成员信息",
                    SummaryDescription = "获取序列化后的群成员信息",
                    ReturnDescription = "序列化后的群成员信息",
                    ArgumentList = new()
                    {
                        { "groupId", "群 ID" },
                        { "qqId", "成员 ID" },
                        { "isCache", "是否使用缓存" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "GetGroupMemberList",
                    DisplayName = "获取序列化后的群成员列表",
                    SummaryDescription = "获取序列化后的群成员列表",
                    ReturnDescription = "序列化后的群成员列表",
                    ArgumentList = new()
                    {
                        { "groupId", "群 ID" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "GetRawFriendList",
                    DisplayName = "获取原始好友列表",
                    SummaryDescription = "获取原始好友列表",
                    ReturnDescription = "好友列表",
                    ArgumentList = new()
                    {
                        { "reserved", "是否倒序" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "GetRawGroupInfo",
                    DisplayName = "获取原始群组信息",
                    SummaryDescription = "获取原始群组信息",
                    ReturnDescription = "群组信息",
                    ArgumentList = new()
                    {
                        { "groupId", "群 ID" },
                        { "notCache", "不使用缓存" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "GetRawGroupList",
                    DisplayName = "获取原始群组列表",
                    SummaryDescription = "获取原始群组列表",
                    ReturnDescription = "群组列表",
                    ArgumentList = new()
                    {
                    }
                },
                new TestPage.Remark()
                {
                    Name = "GetRawGroupMemberInfo",
                    DisplayName = "获取原始群成员信息",
                    SummaryDescription = "获取原始群成员信息",
                    ReturnDescription = "群成员信息",
                    ArgumentList = new()
                    {
                        { "groupId", "群 ID" },
                        { "qqId", "成员 ID" },
                        { "isCache", "是否使用缓存" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "GetRawGroupMemberList",
                    DisplayName = "获取群成员列表",
                    SummaryDescription = "获取群成员列表",
                    ReturnDescription = "原始群成员列表",
                    ArgumentList = new()
                    {
                        { "groupId", "群 ID" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "GetLoginNick",
                    DisplayName = "获取登录实例的账号昵称",
                    SummaryDescription = "获取登录实例的账号昵称",
                    ReturnDescription = "账号昵称",
                    ArgumentList = new()
                    {
                    }
                },
                new TestPage.Remark()
                {
                    Name = "GetLoginQQ",
                    DisplayName = "获取登录实例的账号 ID",
                    SummaryDescription = "获取登录实例的账号 ID",
                    ReturnDescription = "账号 ID",
                    ArgumentList = new()
                    {
                    }
                },
                new TestPage.Remark()
                {
                    Name = "GetStrangerInfo",
                    DisplayName = "获取序列后的陌生人信息",
                    SummaryDescription = "获取序列后的陌生人信息",
                    ReturnDescription = "序列后的陌生人信息",
                    ArgumentList = new()
                    {
                        { "qqId", "陌生人 ID" },
                        { "notCache", "强制缓存" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "SendGroupMessage",
                    DisplayName = "发送群组信息",
                    SummaryDescription = "发送群组信息",
                    ReturnDescription = "消息ID 失败时返回0",
                    ArgumentList = new()
                    {
                        { "groupId", "群 ID" },
                        { "msg", "欲发送的消息" },
                        { "msgId", "引用消息 ID; 当 ID = 0 时表示不引用发送" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "SendLike",
                    DisplayName = "发送名片赞",
                    SummaryDescription = "发送名片赞",
                    ReturnDescription = "0为 成功; 1为 失败",
                    ArgumentList = new()
                    {
                        { "qqId", "欲发送的 ID" },
                        { "count", "发送数量" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "SendPrivateMessage",
                    DisplayName = "发送单聊信息",
                    SummaryDescription = "发送单聊信息",
                    ReturnDescription = "消息ID 失败时返回0",
                    ArgumentList = new()
                    {
                        { "qqId", "群 ID" },
                        { "msg", "欲发送的消息" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "SendDiscussMsg",
                    DisplayName = "发送讨论组信息",
                    SummaryDescription = "发送讨论组信息",
                    ReturnDescription = "消息ID 失败时返回0",
                    ArgumentList = new()
                    {
                        { "discussId", "讨论组 ID" },
                        { "msg", "欲发送的消息" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "SetDiscussLeave",
                    DisplayName = "主动离开讨论组",
                    SummaryDescription = "主动离开讨论组",
                    ReturnDescription = "0为 成功; 1为 失败",
                    ArgumentList = new()
                    {
                        { "discussId", "讨论组 ID" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "SetFriendAddRequest",
                    DisplayName = "处理好友添加请求",
                    SummaryDescription = "处理好友添加请求",
                    ReturnDescription = "0为 操作成功; 1为 操作失败",
                    ArgumentList = new()
                    {
                        { "identifying", "请求标识" },
                        { "responseType", "处理结果: 1 为通过; 2 为拒绝" },
                        { "appendMsg", "备注消息" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "SetGroupAddRequest",
                    DisplayName = "处理群组添加请求",
                    SummaryDescription = "处理群组添加请求",
                    ReturnDescription = "0为 操作成功; 1为 操作失败",
                    ArgumentList = new()
                    {
                        { "identifying", "请求标识" },
                        { "requestType", "添加类型: 1 为主动进群; 2 为邀请进群" },
                        { "responseType", "处理结果: 1 为通过; 2 为拒绝" },
                        { "appendMsg", "备注消息" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "SetGroupAdmin",
                    DisplayName = "设置群管理",
                    SummaryDescription = "设置群管理",
                    ReturnDescription = "0为 操作成功; 1为 操作失败",
                    ArgumentList = new()
                    {
                        { "groupId", "群组 ID" },
                        { "qqId", "被操作者 ID" },
                        { "isSet", "True 为设置; False 为罢免" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "SetGroupAnonymous",
                    DisplayName = "设置群组是否开启匿名",
                    SummaryDescription = "设置群组是否开启匿名",
                    ReturnDescription = "0为 操作成功; 1为 操作失败",
                    ArgumentList = new()
                    {
                        { "groupId", "群组 ID" },
                        { "isOpen", "是否开启匿名" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "SetGroupAnonymousBan",
                    DisplayName = "禁言群匿名成员",
                    SummaryDescription = "禁言群匿名成员",
                    ReturnDescription = "0为 操作成功; 1为 操作失败",
                    ArgumentList = new()
                    {
                        { "groupId", "群组 ID" },
                        { "anonymous", "匿名 ID" },
                        { "banTime", "禁言时长 (单位: 秒)" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "SetGroupBan",
                    DisplayName = "禁言群成员",
                    SummaryDescription = "禁言群成员",
                    ReturnDescription = "0为 操作成功; 1为 操作失败",
                    ArgumentList = new()
                    {
                        { "groupId", "群组 ID" },
                        { "qqId", "成员 ID" },
                        { "banTime", "禁言时长 (单位: 秒)" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "SetGroupCard",
                    DisplayName = "设置群组成员名片",
                    SummaryDescription = "设置群组成员名片",
                    ReturnDescription = "0为 操作成功; 1为 操作失败",
                    ArgumentList = new()
                    {
                        { "groupId", "群组 ID" },
                        { "qqId", "成员 ID" },
                        { "newCard", "新名片" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "SetGroupKick",
                    DisplayName = "移除群组成员",
                    SummaryDescription = "移除群组成员",
                    ReturnDescription = "0为 操作成功; 1为 操作失败",
                    ArgumentList = new()
                    {
                        { "groupId", "群组 ID" },
                        { "qqId", "成员 ID" },
                        { "refuses", "是否拒绝后续入群" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "SetGroupLeave",
                    DisplayName = "主动离开群组",
                    SummaryDescription = "主动离开群组",
                    ReturnDescription = "0为 操作成功; 1为 操作失败",
                    ArgumentList = new()
                    {
                        { "groupId", "群组 ID" },
                        { "isDisband", "True 为解散; False 为离开" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "SetGroupSpecialTitle",
                    DisplayName = "设置群组成员头衔",
                    SummaryDescription = "设置群组成员头衔",
                    ReturnDescription = "0为 操作成功; 1为 操作失败",
                    ArgumentList = new()
                    {
                        { "groupId", "群组 ID" },
                        { "qqId", "成员 ID" },
                        { "title", "头衔" },
                        { "durationTime", "过期时间" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "SetGroupWholeBan",
                    DisplayName = "设置群组全员禁言",
                    SummaryDescription = "设置群组全员禁言",
                    ReturnDescription = "0为 操作成功; 1为 操作失败",
                    ArgumentList = new()
                    {
                        { "groupId", "群组 ID" },
                        { "isDisband", "True 为开启; False 为关闭" },
                    }
                },
            ];
        }

        public class InvokeArugment
        {
            public string Name { get; set; }

            public Type TargetType { get; set; }

            public string Value { get; set; }
        }

        public class ProtocolMethod
        {
            public MethodInfo Method { get; set; }

            public string MethodDescription { get; set; }

            public string ReturnDescription { get; set; }

            public string MethodName { get; set; }

            public string MethodNameDisplay { get; set; }

            public string MethodNameIcon => MethodName.Substring(0, 1);
        }
    }
}