using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Protocol_NoConnection
{
    public partial class Tester
    {
        private List<MethodInfo> EventTestMethods { get; set; } = [];

        private Dictionary<string, Control> EventParamInputs { get; } = [];

        private static readonly Dictionary<string, string> EventDisplayNames = new()
        {
            ["Event_OnAdminChange"] = "群管理员变动",
            ["Event_OnDiscussMsg"] = "收到讨论组消息",
            ["Event_OnEnable"] = "应用启用",
            ["Event_OnDisable"] = "应用禁用",
            ["Event_OnExit"] = "框架退出",
            ["Event_OnFriendAdded"] = "好友已添加",
            ["Event_OnFriendAddRequest"] = "好友添加请求",
            ["Event_OnGroupAddRequest"] = "群添加请求",
            ["Event_OnGroupBan"] = "群禁言事件",
            ["Event_OnGroupMemberDecrease"] = "群成员减少",
            ["Event_OnGroupMemberIncrease"] = "群成员增加",
            ["Event_OnStartUp"] = "框架启动",
            ["Event_OnUpload"] = "群文件上传",
            ["Event_OnGroupMsgRecall"] = "群消息撤回",
            ["Event_OnPrivateMsgRecall"] = "私聊消息撤回",
            ["Event_OnGroupMemberCardChanged"] = "群名片变更",
            ["Event_OnFriendNickChanged"] = "好友昵称变更",
            ["Event_OnGroupNameChanged"] = "群名称变更"
        };

        private static readonly Dictionary<string, string> CommonParamDisplayNames = new()
        {
            ["subType"] = "子类型",
            ["sendTime"] = "发生时间",
            ["msgId"] = "消息ID",
            ["fromGroup"] = "来源群号",
            ["fromQQ"] = "来源QQ",
            ["beingOperateQQ"] = "被操作QQ",
            ["fromNative"] = "来源讨论组ID",
            ["fromAnonymous"] = "匿名标识",
            ["msg"] = "消息内容",
            ["font"] = "字体ID",
            ["duration"] = "时长(秒)",
            ["responseFlag"] = "响应标识",
            ["file"] = "文件名",
            ["group"] = "群号",
            ["qq"] = "QQ",
            ["card"] = "群名片",
            ["nick"] = "昵称",
            ["name"] = "名称"
        };

        private static readonly Dictionary<string, Dictionary<string, string>> EventParamDescriptions = new()
        {
            [nameof(PluginManagerProxy.Event_OnAdminChange)] = new()
            {
                ["subType"] = "子类型：1=取消管理员，2=设置管理员",
                ["sendTime"] = "事件发生时间（将自动转为时间戳）",
                ["fromGroup"] = "来源群ID",
                ["beingOperateQQ"] = "被操作的QQ"
            },
            [nameof(PluginManagerProxy.Event_OnFriendAdded)] = new()
            {
                ["subType"] = "子类型固定为 1",
                ["sendTime"] = "事件发生时间（将自动转为时间戳）",
                ["fromQQ"] = "来源QQ"
            },
            [nameof(PluginManagerProxy.Event_OnFriendAddRequest)] = new()
            {
                ["subType"] = "子类型固定为 1",
                ["sendTime"] = "事件发生时间（将自动转为时间戳）",
                ["fromQQ"] = "来源QQ",
                ["msg"] = "附加消息",
                ["responseFlag"] = "响应标识"
            },
            [nameof(PluginManagerProxy.Event_OnGroupAddRequest)] = new()
            {
                ["subType"] = "子类型：1=申请入群，2=邀请入群",
                ["sendTime"] = "事件发生时间（将自动转为时间戳）",
                ["fromGroup"] = "来源群ID",
                ["fromQQ"] = "来源QQ",
                ["msg"] = "附加消息",
                ["responseFlag"] = "响应标识"
            },
            [nameof(PluginManagerProxy.Event_OnGroupBan)] = new()
            {
                ["subType"] = "子类型：1=解除禁言，2=禁言",
                ["sendTime"] = "事件发生时间（将自动转为时间戳）",
                ["fromGroup"] = "来源群ID",
                ["fromQQ"] = "操作者QQ",
                ["beingOperateQQ"] = "被禁言QQ（全体禁言填0）",
                ["duration"] = "禁言时长（秒）"
            },
            [nameof(PluginManagerProxy.Event_OnGroupMemberDecrease)] = new()
            {
                ["subType"] = "子类型：1=主动退出，2=被踢出",
                ["sendTime"] = "事件发生时间（将自动转为时间戳）",
                ["fromGroup"] = "来源群ID",
                ["fromQQ"] = "事件主体QQ",
                ["beingOperateQQ"] = "操作者QQ"
            },
            [nameof(PluginManagerProxy.Event_OnGroupMemberIncrease)] = new()
            {
                ["subType"] = "子类型：1=主动进群，2=邀请入群",
                ["sendTime"] = "事件发生时间（将自动转为时间戳）",
                ["fromGroup"] = "来源群ID",
                ["fromQQ"] = "事件主体QQ",
                ["beingOperateQQ"] = "操作者QQ"
            },
            [nameof(PluginManagerProxy.Event_OnDiscussMsg)] = new()
            {
                ["subType"] = "子类型固定为 1",
                ["msgId"] = "消息ID",
                ["fromNative"] = "消息来源讨论组ID",
                ["fromQQ"] = "消息来源QQ",
                ["msg"] = "消息内容",
                ["font"] = "字体ID"
            },
            [nameof(PluginManagerProxy.Event_OnUpload)] = new()
            {
                ["subType"] = "子类型固定为 1",
                ["sendTime"] = "事件发生时间（将自动转为时间戳）",
                ["fromGroup"] = "来源群ID",
                ["fromQQ"] = "文件来源QQ",
                ["file"] = "文件名"
            }
        };

        private static readonly Dictionary<string, List<SubTypeOption>> SubTypeOptions = new()
        {
            [nameof(PluginManagerProxy.Event_OnAdminChange)] = [new(1, "取消管理员"), new(2, "设置管理员")],
            [nameof(PluginManagerProxy.Event_OnFriendAdded)] = [new(1, "固定")],
            [nameof(PluginManagerProxy.Event_OnFriendAddRequest)] = [new(1, "固定")],
            [nameof(PluginManagerProxy.Event_OnGroupAddRequest)] = [new(1, "申请入群"), new(2, "邀请入群")],
            [nameof(PluginManagerProxy.Event_OnGroupBan)] = [new(1, "解除禁言"), new(2, "禁言")],
            [nameof(PluginManagerProxy.Event_OnGroupMemberDecrease)] = [new(1, "主动退出"), new(2, "被踢出")],
            [nameof(PluginManagerProxy.Event_OnGroupMemberIncrease)] = [new(1, "主动进群"), new(2, "邀请入群")],
            [nameof(PluginManagerProxy.Event_OnDiscussMsg)] = [new(1, "固定")],
            [nameof(PluginManagerProxy.Event_OnUpload)] = [new(1, "固定")]
        };

        private void Tester_Load(object sender, EventArgs e)
        {
            GroupValue.Text = CommonConfig.GetConfig("TesterGroup", @"conf/Test.json", (long)1919810).ToString();
            QQValue.Text = CommonConfig.GetConfig("TesterQQ", @"conf/Test.json", (long)1145141919).ToString();
            MessageHistories = CommonConfig.GetConfig("MessageHistories", @"conf/Test.json", new List<string>());

            PicButton.Enabled = PicServer.Instance?.Running ?? false;
            InitializeEventTester();
            SendValue.Focus();
        }

        private void InitializeEventTester()
        {
            EventTestMethods = typeof(PluginManagerProxy)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.Name.StartsWith("Event_", StringComparison.Ordinal))
                .Where(x => x.Name != nameof(PluginManagerProxy.Event_OnPrivateMsg) && x.Name != nameof(PluginManagerProxy.Event_OnGroupMsg))
                .Where(x => x.GetParameters().Any() is false || x.GetParameters()[0].ParameterType != typeof(CQPluginProxy))
                .OrderBy(x => x.Name)
                .ThenBy(x => x.GetParameters().Length)
                .ToList();

            EventSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            EventSelector.Items.Clear();
            foreach (var item in EventTestMethods)
            {
                EventSelector.Items.Add(new EventMethodItem(item, GetEventDisplayName(item)));
            }
            if (EventSelector.Items.Count > 0)
            {
                EventSelector.SelectedIndex = 0;
            }
        }

        private void EventMethodSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            RenderEventParamInputs();
        }

        private void RenderEventParamInputs()
        {
            EventArgumentsPanel.Controls.Clear();
            EventParamInputs.Clear();
            MethodInfo method = GetSelectedMethod();
            if (method == null)
            {
                return;
            }

            int top = 6;
            foreach (var parameter in method.GetParameters())
            {
                Label label = new()
                {
                    AutoSize = true,
                    Left = 4,
                    Top = top + 4,
                    Text = $"{GetParameterDisplayName(method.Name, parameter.Name)} ({GetFriendlyTypeName(parameter.ParameterType)})"
                };
                Control input = CreateParamInputControl(method.Name, parameter, top);
                EventArgumentsPanel.Controls.Add(label);
                EventArgumentsPanel.Controls.Add(input);
                string description = GetParameterDescription(method.Name, parameter.Name);
                if (!string.IsNullOrWhiteSpace(description))
                {
                    Label descLabel = new()
                    {
                        AutoSize = true,
                        Left = 4,
                        Top = top + 32,
                        ForeColor = System.Drawing.SystemColors.GrayText,
                        Text = description
                    };
                    EventArgumentsPanel.Controls.Add(descLabel);
                    top += 59;
                }
                else
                {
                    top += 40;
                }
                EventParamInputs[parameter.Name] = input;
            }
        }

        private Control CreateParamInputControl(string methodName, ParameterInfo parameter, int top)
        {
            if (parameter.Name == "subType" && parameter.ParameterType == typeof(int))
            {
                ComboBox combo = new()
                {
                    Left = 206,
                    Top = top,
                    Width = 250,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                if (SubTypeOptions.TryGetValue(methodName, out var options) && options.Count > 0)
                {
                    foreach (var option in options)
                    {
                        combo.Items.Add(option);
                    }
                    combo.SelectedIndex = 0;
                    combo.Enabled = options.Count > 1;
                }
                else
                {
                    combo.Items.Add(new SubTypeOption(1, "默认"));
                    combo.SelectedIndex = 0;
                    combo.Enabled = false;
                }
                return combo;
            }
            if (parameter.Name == "sendTime" && parameter.ParameterType == typeof(long))
            {
                return new DateTimePicker
                {
                    Left = 206,
                    Top = top,
                    Width = 250,
                    Format = DateTimePickerFormat.Custom,
                    CustomFormat = "yyyy-MM-dd HH:mm:ss"
                };
            }
            if (parameter.ParameterType == typeof(bool))
            {
                return new CheckBox
                {
                    Left = 206,
                    Top = top + 2,
                    Width = 250,
                    Checked = bool.TryParse(GetDefaultValueText(parameter), out bool checkedValue) && checkedValue
                };
            }
            if (parameter.ParameterType == typeof(DateTime))
            {
                DateTimePicker picker = new()
                {
                    Left = 206,
                    Top = top,
                    Width = 250,
                    Format = DateTimePickerFormat.Custom,
                    CustomFormat = "yyyy-MM-dd HH:mm:ss"
                };
                if (DateTime.TryParse(GetDefaultValueText(parameter), out DateTime dt))
                {
                    picker.Value = dt;
                }
                return picker;
            }

            return new TextBox
            {
                Left = 206,
                Top = top,
                Width = 250,
                Text = GetDefaultValueText(parameter)
            };
        }

        private static string GetFriendlyTypeName(Type type)
        {
            if (type == typeof(int))
            {
                return "int";
            }

            if (type == typeof(long))
            {
                return "long";
            }

            if (type == typeof(bool))
            {
                return "bool";
            }

            if (type == typeof(double))
            {
                return "double";
            }

            if (type == typeof(decimal))
            {
                return "decimal";
            }

            if (type == typeof(string))
            {
                return "string";
            }

            if (type == typeof(DateTime))
            {
                return "DateTime";
            }

            return type.Name;
        }

        private string GetDefaultValueText(ParameterInfo parameter)
        {
            string name = parameter.Name?.ToLowerInvariant() ?? string.Empty;
            if (parameter.ParameterType == typeof(long) && name.Contains("time"))
            {
                return DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            }
            if (parameter.ParameterType == typeof(long) && name.Contains("group") && GroupId > 0)
            {
                return GroupId.ToString();
            }
            if (parameter.ParameterType == typeof(long) && name.Contains("qq") && QQId > 0)
            {
                return QQId.ToString();
            }
            if (parameter.ParameterType == typeof(int) && name.Contains("msgid"))
            {
                return MsgId.ToString();
            }
            if (parameter.ParameterType == typeof(int) && name.Contains("subtype"))
            {
                return "1";
            }
            if (parameter.ParameterType == typeof(string) && name.Contains("msg"))
            {
                return string.IsNullOrWhiteSpace(SendValue.Text) ? "test" : SendValue.Text;
            }
            if (parameter.HasDefaultValue && parameter.DefaultValue != null)
            {
                return parameter.DefaultValue.ToString();
            }
            if (parameter.ParameterType == typeof(DateTime))
            {
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return parameter.ParameterType.IsValueType ? Activator.CreateInstance(parameter.ParameterType)?.ToString() ?? "0" : string.Empty;
        }

        private static string GetEventDisplayName(MethodInfo method)
        {
            if (EventDisplayNames.TryGetValue(method.Name, out string name))
            {
                return $"{name} ({method.Name})";
            }
            return method.Name;
        }

        private static string GetParameterDisplayName(string methodName, string parameterName)
        {
            if (methodName == nameof(PluginManagerProxy.Event_OnGroupBan))
            {
                if (parameterName == "fromQQ")
                {
                    return "操作者QQ";
                }

                if (parameterName == "beingOperateQQ")
                {
                    return "被禁言QQ";
                }
            }
            if (methodName == nameof(PluginManagerProxy.Event_OnGroupMemberDecrease) || methodName == nameof(PluginManagerProxy.Event_OnGroupMemberIncrease))
            {
                if (parameterName == "fromQQ")
                {
                    return "事件主体QQ";
                }

                if (parameterName == "beingOperateQQ")
                {
                    return "操作者QQ";
                }
            }
            if (CommonParamDisplayNames.TryGetValue(parameterName, out string name))
            {
                return name;
            }
            return parameterName;
        }

        private static string GetParameterDescription(string methodName, string parameterName)
        {
            if (EventParamDescriptions.TryGetValue(methodName, out var methodParams) && methodParams.TryGetValue(parameterName, out var description))
            {
                return description;
            }
            return string.Empty;
        }

        private MethodInfo GetSelectedMethod()
        {
            return EventSelector.SelectedItem switch
            {
                EventMethodItem item => item.Method,
                MethodInfo method => method,
                _ => null
            };
        }

        private void EventInvokeButton_Click(object sender, EventArgs e)
        {
            if (PluginManagerProxy.Instance == null)
            {
                MessageBox.Show("PluginManagerProxy 尚未初始化", "调用失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            MethodInfo method = GetSelectedMethod();
            if (method == null)
            {
                MessageBox.Show("请选择一个事件方法", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            object[] args = new object[method.GetParameters().Length];
            for (int i = 0; i < method.GetParameters().Length; i++)
            {
                var parameter = method.GetParameters()[i];
                if (!EventParamInputs.TryGetValue(parameter.Name, out Control input) || !TryParseInputValue(input, parameter.ParameterType, out object value))
                {
                    MessageBox.Show($"参数 {parameter.Name} 无效，期望类型: {GetFriendlyTypeName(parameter.ParameterType)}", "参数错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                args[i] = value;
            }

            int logId = LogHelper.WriteLog(
                LogLevel.InfoReceive,
                "AMN框架",
                "[↓]事件测试调用",
                BuildEventInvokeDetail(method, args),
                "处理中...");

            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                object result = method.Invoke(PluginManagerProxy.Instance, args);
                sw.Stop();
                string status = $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s";
                if (result is CQPluginProxy plugin && plugin?.AppInfo?.name != null)
                {
                    status += $"(由 {plugin.AppInfo.name} 结束事件处理)";
                }
                LogHelper.UpdateLogStatus(logId, status);
                MessageBox.Show($"调用成功\n方法: {method.Name}\n耗时: {sw.ElapsedMilliseconds} ms\n返回: {FormatInvokeResult(result)}", "事件调用结果", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (TargetInvocationException ex)
            {
                sw.Stop();
                LogHelper.UpdateLogStatus(logId, $"x {sw.ElapsedMilliseconds / (double)1000:f2} s");
                LogHelper.Error("事件测试调用", ex.InnerException ?? ex);
                MessageBox.Show($"调用失败\n方法: {method.Name}\n耗时: {sw.ElapsedMilliseconds} ms\n异常: {ex.InnerException?.Message ?? ex.Message}", "事件调用结果", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                sw.Stop();
                LogHelper.UpdateLogStatus(logId, $"x {sw.ElapsedMilliseconds / (double)1000:f2} s");
                LogHelper.Error("事件测试调用", ex);
                MessageBox.Show($"调用失败\n方法: {method.Name}\n耗时: {sw.ElapsedMilliseconds} ms\n异常: {ex.Message}", "事件调用结果", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string BuildEventInvokeDetail(MethodInfo method, object[] args)
        {
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length == 0)
            {
                return $"方法:{method.Name} 参数:(无)";
            }
            string argsText = string.Join(", ", parameters.Select((p, i) => $"{p.Name}={args[i]}"));
            return $"方法:{method.Name} 参数:{argsText}";
        }

        private static bool TryParseInputValue(Control inputControl, Type targetType, out object value)
        {
            value = null;
            if (inputControl is ComboBox comboBox && targetType == typeof(int) && comboBox.SelectedItem is SubTypeOption option)
            {
                value = option.Value;
                return true;
            }
            if (inputControl is DateTimePicker sendTimePicker && targetType == typeof(long))
            {
                value = new DateTimeOffset(sendTimePicker.Value).ToUnixTimeSeconds();
                return true;
            }

            string input = inputControl switch
            {
                TextBox textBox => textBox.Text,
                DateTimePicker dateTimePicker => dateTimePicker.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                CheckBox checkBox => checkBox.Checked.ToString(),
                _ => inputControl.Text
            };

            if (targetType == typeof(bool) && inputControl is CheckBox check)
            {
                value = check.Checked;
                return true;
            }
            if (targetType == typeof(DateTime) && inputControl is DateTimePicker picker)
            {
                value = picker.Value;
                return true;
            }

            if (targetType == typeof(string))
            {
                value = input;
                return true;
            }
            if (targetType == typeof(int))
            {
                if (int.TryParse(input, out int intValue))
                {
                    value = intValue;
                    return true;
                }
                return false;
            }
            if (targetType == typeof(long))
            {
                if (long.TryParse(input, out long longValue))
                {
                    value = longValue;
                    return true;
                }
                return false;
            }
            if (targetType == typeof(bool))
            {
                if (bool.TryParse(input, out bool boolValue))
                {
                    value = boolValue;
                    return true;
                }
                return false;
            }
            if (targetType == typeof(double))
            {
                if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double doubleValue))
                {
                    value = doubleValue;
                    return true;
                }
                return false;
            }
            if (targetType == typeof(decimal))
            {
                if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal decimalValue))
                {
                    value = decimalValue;
                    return true;
                }
                return false;
            }
            if (targetType == typeof(DateTime))
            {
                if (DateTime.TryParse(input, out DateTime dateTimeValue))
                {
                    value = dateTimeValue;
                    return true;
                }
                return false;
            }
            if (targetType.IsEnum)
            {
                try
                {
                    value = Enum.Parse(targetType, input, true);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            try
            {
                value = Convert.ChangeType(input, targetType);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string FormatInvokeResult(object result)
        {
            if (result == null)
            {
                return "(无返回值)";
            }
            if (result is CQPluginProxy plugin)
            {
                return plugin.AppInfo?.name != null ? $"由 {plugin.AppInfo.name} 结束处理" : plugin.ToString();
            }
            return result.ToString();
        }

        private sealed class EventMethodItem(MethodInfo method, string displayName)
        {
            public MethodInfo Method { get; } = method;

            public string DisplayName { get; } = displayName;

            public override string ToString()
            {
                return DisplayName;
            }
        }

        private sealed class SubTypeOption(int value, string text)
        {
            public int Value { get; } = value;

            public string Text { get; } = $"{value} - {text}";

            public override string ToString()
            {
                return Text;
            }
        }
    }
}