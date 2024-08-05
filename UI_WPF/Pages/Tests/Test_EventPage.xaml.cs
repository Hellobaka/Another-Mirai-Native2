using Another_Mirai_Native.Native;
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
    /// Test_EventPage.xaml 的交互逻辑
    /// </summary>
    public partial class Test_EventPage : Page, INotifyPropertyChanged
    {
        public Test_EventPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public List<PluginEvent> DisplayPluginEvents { get; set; } = [];

        public bool FormLoaded { get; set; }

        public bool PageEnabled { get; set; }

        public CQPluginProxy? TestPlugin => TestPage.CurrentPlugin;

        public List<InvokeArugment> InvokeArugments { get; set; } = [];

        public List<PluginEvent> PluginEvents { get; set; } = [];

        public List<TestPage.Remark> Remarks { get; set; } = [];

        private PluginEvent CurrentMethod { get; set; }

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
                    InvokeArugment arugment = (sender as TextBox).Tag as InvokeArugment;
                    arugment.Value = (sender as TextBox).Text;
                };
                ArgumentList.Children.Add(inputTextbox);
            }

            EmptyHint.Visibility = ArgumentList.Children.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void FilterMethods()
        {
            Dispatcher.BeginInvoke(() =>
            {
                DisplayPluginEvents = PluginEvents.Where(x => x.EventName.ToLower().Contains(SearchTextValue.Text.ToLower())
                            || x.EventDescription.ToLower().Contains(SearchTextValue.Text.ToLower())).ToList();
                OnPropertyChanged(nameof(DisplayPluginEvents));
            });
        }

        private async void InvokeButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentMethod == null || CurrentMethod.Method == null)
            {
                DialogHelper.ShowSimpleDialog("调用测试方法", $"未选择测试方法");
                return;
            }
            List<object> arugments = [TestPlugin];
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

            if (!await DialogHelper.ShowConfirmDialog("调用测试方法", $"确定要调用测试方法 {CurrentMethod.EventName} 吗？请二次确认输入的参数。"))
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
                await Task.Run(() => ret = CurrentMethod.Method.Invoke(PluginManagerProxy.Instance, arugments.ToArray()));

                if (ret is not int result)
                {
                    throw new InvalidCastException("返回值无法转换为 Int32");
                }
                string time = $" ({stopwatch.ElapsedMilliseconds} ms)";
                switch (result)
                {
                    case 1:
                        FunctionInvokeResult.Text = "调用成功，结果：阻塞后续请求" + time;
                        break;

                    case 0:
                        FunctionInvokeResult.Text = "调用成功，结果：放行" + time;
                        break;

                    default:
                        break;
                }
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
            PageEnabled = TestPlugin != null;
            OnPropertyChanged(nameof(PageEnabled));

            if (FormLoaded)
            {
                return;
            }
            FormLoaded = true;
            PluginEvents.Clear();

            Remarks = BuildRemarkList();
            foreach (var item in PluginManagerProxy.Instance.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance).OrderBy(x => x.Name))
            {
                if (!item.Name.StartsWith("Event_") || item.ReturnParameter.ParameterType != typeof(int))
                {
                    continue;
                }
                var remark = Remarks.FirstOrDefault(x => x.Name == item.Name.Replace("Event_On", ""));
                if (remark == null)
                {
                    continue;
                }

                var pluginEvent = new PluginEvent
                {
                    EventName = item.Name.Replace("Event_On", ""),
                    EventNameDisplay = remark.DisplayName,
                    ReturnDescription = $"{item.GetParameters().Length - 1}个参数",
                    Method = item
                };
                pluginEvent.EventDescription = remark.SummaryDescription;
                PluginEvents.Add(pluginEvent);
            }
            DisplayPluginEvents = [.. PluginEvents];
            OnPropertyChanged(nameof(DisplayPluginEvents));
        }

        private void ProtocolMethodList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0 || e.AddedItems[0] is not PluginEvent method)
            {
                return;
            }
            CurrentMethod = method;
            InvokeArugments.Clear();
            var remark = Remarks.FirstOrDefault(x => x.Name == method.EventName);
            if (remark != null)
            {
                EventNameDisplay.Text = remark.Name;
                EventDescDisplay.Text = remark.SummaryDescription;
                EventReturnDescDisplay.Text = remark.ReturnDescription;
            }
            foreach (var item in method.Method.GetParameters())
            {
                if (string.IsNullOrEmpty(item.Name) || item.ParameterType == typeof(CQPluginProxy))
                {
                    continue;
                }
                string desc = remark != null ? (remark.ArgumentList.TryGetValue(item.Name, out string v) ? $" ({v})" : "") : "";
                InvokeArugments.Add(new InvokeArugment
                {
                    Name = item.Name + desc + $" ({item.ParameterType.Name})",
                    TargetType = item.ParameterType,
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
                    Name = "AdminChange",
                    DisplayName = "群管理变动事件",
                    SummaryDescription = "Type: 101 群管理变动事件",
                    ReturnDescription = "1为阻塞 0为放行",
                    ArgumentList = new()
                    {
                        { "subType", "子类型 取消: 1 设置: 2" },
                        { "sendTime", "发生时间" },
                        { "fromGroup", "来源群ID" },
                        { "beingOperateQQ", "操作者QQ" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "Disable",
                    DisplayName = "应用禁用",
                    SummaryDescription = "Type: 1004 应用禁用",
                    ReturnDescription = "无特殊定义",
                    ArgumentList = new()
                    {
                    }
                },
                new TestPage.Remark()
                {
                    Name = "DiscussMsg",
                    DisplayName = "收到讨论组消息",
                    SummaryDescription = "Type: 4 收到讨论组消息",
                    ReturnDescription = "1为阻塞 0为放行",
                    ArgumentList = new()
                    {
                        { "subType", "子类型 固定为1" },
                        { "msgId", "消息ID" },
                        { "fromNative", "消息来源讨论组ID" },
                        { "fromQQ", "消息来源QQ" },
                        { "msg", "消息来源文本" },
                        { "font", "字体ID" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "Enable",
                    DisplayName = "应用启用",
                    SummaryDescription = "Type: 1003 应用启用",
                    ReturnDescription = "无特殊定义",
                    ArgumentList = new()
                    {
                    }
                },
                new TestPage.Remark()
                {
                    Name = "Exit",
                    DisplayName = "框架退出",
                    SummaryDescription = "Type: 1002 框架退出",
                    ReturnDescription = "无特殊定义",
                    ArgumentList = new()
                    {
                    }
                },
                new TestPage.Remark()
                {
                    Name = "FriendAdded",
                    DisplayName = "好友已添加",
                    SummaryDescription = "Type: 201 好友已添加",
                    ReturnDescription = "1为阻塞 0为放行",
                    ArgumentList = new()
                    {
                        { "subType", "子类型 固定为1" },
                        { "sendTime", "发生时间" },
                        { "fromQQ", "来源QQ" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "FriendAddRequest",
                    DisplayName = "好友添加请求",
                    SummaryDescription = "Type: 301 好友添加请求",
                    ReturnDescription = "1为阻塞 0为放行",
                    ArgumentList = new()
                    {
                        { "subType", "子类型 固定为1" },
                        { "sendTime", "发生时间" },
                        { "fromQQ", "来源QQ" },
                        { "msg", "附加消息" },
                        { "responseFlag", "响应标识" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "GroupAddRequest",
                    DisplayName = "群添加请求",
                    SummaryDescription = "Type: 302 群添加请求",
                    ReturnDescription = "1为阻塞 0为放行",
                    ArgumentList = new()
                    {
                        { "subType", "子类型 申请入群: 1 邀请入群: 2" },
                        { "sendTime", "发生时间" },
                        { "fromGroup", "来源群ID" },
                        { "fromQQ", "来源QQ" },
                        { "msg", "附加消息" },
                        { "responseFlag", "响应标识" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "GroupBan",
                    DisplayName = "群禁言事件",
                    SummaryDescription = "Type: 104 群禁言事件",
                    ReturnDescription = "1为阻塞 0为放行",
                    ArgumentList = new()
                    {
                        { "subType", "子类型 解除禁言: 1 禁言: 2" },
                        { "sendTime", "发生时间" },
                        { "fromGroup", "来源群ID" },
                        { "fromQQ", "来源QQ" },
                        { "beingOperateQQ", "操作者QQ" },
                        { "duration", "禁言时长(s) 仅在禁言时生效" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "GroupMemberDecrease",
                    DisplayName = "群成员减少事件",
                    SummaryDescription = "Type: 102 群成员减少事件",
                    ReturnDescription = "1为阻塞 0为放行",
                    ArgumentList = new()
                    {
                        { "subType", "子类型 主动退出: 1 被踢出: 2" },
                        { "sendTime", "发生时间" },
                        { "fromGroup", "来源群ID" },
                        { "fromQQ", "来源QQ" },
                        { "beingOperateQQ", "操作者QQ" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "GroupMemberIncrease",
                    DisplayName = "群成员添加事件",
                    SummaryDescription = "Type: 103 群成员添加事件",
                    ReturnDescription = "1为阻塞 0为放行",
                    ArgumentList = new()
                    {
                        { "subType", "子类型 主动进群: 1 邀请入群: 2" },
                        { "sendTime", "发生时间" },
                        { "fromGroup", "来源群ID" },
                        { "fromQQ", "来源QQ" },
                        { "beingOperateQQ", "操作者QQ" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "GroupMsg",
                    DisplayName = "收到群消息",
                    SummaryDescription = "Type: 2 收到群消息",
                    ReturnDescription = "1为阻塞 0为放行",
                    ArgumentList = new()
                    {
                        { "subType", "子类型 固定为1" },
                        { "msgId", "消息ID" },
                        { "fromGroup", "消息来源ID" },
                        { "fromQQ", "消息来源QQ" },
                        { "fromAnonymous", "匿名标识" },
                        { "msg", "消息内容" },
                        { "font", "字体ID" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "PrivateMsg",
                    DisplayName = "收到好友消息",
                    SummaryDescription = "Type: 21 收到好友消息",
                    ReturnDescription = "1为阻塞 0为放行",
                    ArgumentList = new()
                    {
                        { "subType", "子类型 固定为11" },
                        { "msgId", "消息ID" },
                        { "fromQQ", "消息来源QQ" },
                        { "msg", "消息内容" },
                        { "font", "字体ID" },
                    }
                },
                new TestPage.Remark()
                {
                    Name = "StartUp",
                    DisplayName = "框架启动事件",
                    SummaryDescription = "Type: 1001 框架启动事件",
                    ReturnDescription = "无定义",
                    ArgumentList = new()
                    {
                    }
                },
                new TestPage.Remark()
                {
                    Name = "Upload",
                    DisplayName = "群文件上传事件",
                    SummaryDescription = "Type: 11 群文件上传事件",
                    ReturnDescription = "1为阻塞 0为放行",
                    ArgumentList = new()
                    {
                        { "subType", "子类型 固定为1" },
                        { "sendTime", "发生时间" },
                        { "fromGroup", "来源群ID" },
                        { "fromQQ", "文件来源QQ" },
                        { "file", "文件名" },
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

        public class PluginEvent
        {
            public MethodInfo Method { get; set; }

            public string ReturnDescription { get; set; }

            public string EventDescription { get; set; }

            public string EventName { get; set; }

            public string EventNameDisplay { get; set; }
       
            public string EventNameIcon => EventName.Substring(0, 1);
        }
    }
}