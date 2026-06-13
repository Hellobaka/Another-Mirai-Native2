using Another_Mirai_Native;
using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Protocol_NoConnection
{
    public partial class Tester : Form
    {
        public Tester()
        {
            InitializeComponent();
            SendValue.KeyPress += (_, e) =>
            {
                if (e.KeyChar == System.Convert.ToChar(13))
                {
                    e.Handled = true;
                }
            };
        }

        private long GroupId => long.TryParse(GroupValue.Text, out long value) ? value : -1;

        private long QQId => long.TryParse(QQValue.Text, out long value) ? value : -1;

        private int MsgId { get; set; } = 1;

        private List<string> MessageHistories { get; set; } = new();

        private int MessageHistoryIndex { get; set; }

        private bool AutoRecall => UseAutoRecall.Checked;

        private int AutoRecallInterval => int.TryParse(AutoRecallValue.Text, out int value) ? value : -1;

        private bool UseCustomMessageID => UseMessageID.Checked;

        private int CustomMessageID => int.TryParse(MessageIDValue.Text, out int value) ? value : -1;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Protocol Protocol { get; set; }

        private byte[] AlterQRCodePicture { get; set; } = [];

        private void SendButton_Click(object sender, EventArgs e)
        {
            if (GroupId < 0)
            {
                MessageBox.Show("GroupId 无效");
                return;
            }
            if (QQId < 0)
            {
                MessageBox.Show("QQId 无效");
                return;
            }
            CommonConfig.SetConfig("TesterGroup", GroupId, @"conf/Test.json");
            CommonConfig.SetConfig("TesterQQ", QQId, @"conf/Test.json");
            string msg = SendValue.Text;
            SendValue.Text = "";
            MessageHistories.Remove(msg);
            MessageHistories.Add(msg);
            CommonConfig.SetConfig("MessageHistories", MessageHistories, @"conf/Test.json");
            MessageHistoryIndex = 0;
            Task.Run(() =>
            {
                Stopwatch sw = new();
                sw.Start();
                int logId;
                CQPluginProxy handledPlugin = null;
                int msgId = UseCustomMessageID ? CustomMessageID : MsgId++;
                if (msgId < 0)
                {
                    msgId = MsgId++;
                }
                RequestCache.AddMessageCache(msgId, msg);
                if (PrivateSelector.Checked)
                {
                    logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "[↓]收到好友消息", $"QQ:{QQId} 消息: {msg}", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.Event_OnPrivateMsg(11, msgId, QQId, msg, 0, DateTime.Now);
                }
                else
                {
                    logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "[↓]收到消息", $"群:{GroupId} QQ:{QQId} 消息: {msg}", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.Event_OnGroupMsg(1, msgId, GroupId, QQId, "", msg, 0, DateTime.Now);
                }
                sw.Stop();
                Task.Run(() =>
                {
                    if (AutoRecall is false || AutoRecallInterval < 0)
                    {
                        return;
                    }
                    Thread.Sleep((int)TimeSpan.FromSeconds(AutoRecallInterval).TotalMilliseconds);
                    if (PrivateSelector.Checked)
                    {
                        PluginManagerProxy.Instance.Event_OnPrivateMsgRecall(msgId, QQId, msg);
                    }
                    else
                    {
                        PluginManagerProxy.Instance.Event_OnGroupMsgRecall(msgId, GroupId, msg);
                    }
                });
                string updateMsg = $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s";
                LogHelper.LocalDebug("", updateMsg);
                if (handledPlugin != null)
                {
                    updateMsg += $"(由 {handledPlugin.AppInfo.name} 结束消息处理)";
                }
                LogHelper.UpdateLogStatus(logId, updateMsg);
            });
        }

        private void AtButton_Click(object sender, EventArgs e)
        {
            SendValue.Text += $"[CQ:at,qq={Protocol.Instance.GetLoginQQ()}]";
        }

        private async void PicButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "image"),
                Filter = "图片文件|*.png;*.jpg;*.jpeg;*.gif|CQ 图片文件|*.cqimg|任何文件|*.*",
                Title = "选择图片",
                Multiselect = true,
                AddExtension = false
            };
            dialog.ShowDialog();
            foreach (var item in dialog.FileNames)
            {
                var path1 = Path.GetFullPath(item).TrimEnd(Path.DirectorySeparatorChar);
                var path2 = Path.GetFullPath(dialog.InitialDirectory).TrimEnd(Path.DirectorySeparatorChar);
                string path;
                if (path1.StartsWith(path2 + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                {
                    // 为同一目录
                    path = Helper.GetRelativePath(item, dialog.InitialDirectory);
                }
                else
                {
                    // 移动文件至默认目录
                    File.Copy(item, Path.Combine(dialog.InitialDirectory, Path.GetFileName(item)), true);
                    path = Path.GetFileName(item);
                }
                if (path.EndsWith(".cqimg"))
                {
                    SendValue.Text += $"[CQ:image,file={Path.GetFileNameWithoutExtension(path)}] ";
                    continue;
                }
                string imgId = await ChatHistoryHelper.CacheMessageFile(CachedFileType.Image, $"{PicServer.Instance.ListenURL}{path}");
                SendValue.Text += $"[CQ:image,file={imgId}] ";
            }
        }

        private void PrivateSelector_CheckedChanged(object sender, EventArgs e)
        {
            GroupValue.Enabled = !PrivateSelector.Checked;
        }

        private void SendValue_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Up)
                {
                    MessageHistoryIndex = Math.Min(MessageHistories.Count, MessageHistoryIndex + 1);
                    if (MessageHistories.Count > 0 && MessageHistories.Count > MessageHistoryIndex - 1)
                    {
                        // SendMessage.Text = MessageHistories[MessageHistories.Count - MessageHistoryIndex];
                        SendValue.Text = MessageHistories[MessageHistories.Count - MessageHistoryIndex];
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    MessageHistoryIndex = Math.Max(MessageHistoryIndex - 1, 0);
                    try
                    {
                        SendValue.Text = MessageHistories[MessageHistories.Count - MessageHistoryIndex];
                    }
                    catch
                    {
                        SendValue.Text = "";
                    }
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    SendButton.PerformClick();
                    e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void Tester_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            WindowState = FormWindowState.Minimized;
        }

        private void UseAutoRecall_CheckedChanged(object sender, EventArgs e)
        {
            AutoRecallValue.Enabled = UseAutoRecall.Checked;
        }

        private void UseMessageID_CheckedChanged(object sender, EventArgs e)
        {
            MessageIDValue.Enabled = UseMessageID.Checked;
        }

        private void Tester_Resize(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
            {
                SendValue.Focus();
            }
        }

        private byte[] ReadEmbeddedImageBytes(string resourceName)
        {
            // 获取当前程序集
            var assembly = Assembly.GetExecutingAssembly();

            // 资源名称通常为 "命名空间.文件名"
            // 你可以通过 assembly.GetManifestResourceNames() 查看所有资源名
            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                return [];

            using MemoryStream ms = new();
            stream.CopyTo(ms);
            return ms.ToArray();
        }

        private void ShowQRCodeButton_Click(object sender, EventArgs e)
        {
            if (AlterQRCodePicture?.Length > 0)
            {
                Protocol.ShowQRCode("HelloWorld", AlterQRCodePicture);
            }
            else
            {
                Protocol.ShowQRCode("HelloWorld", ReadEmbeddedImageBytes("Protocol_NoConnection.QRCode.png"));
            }
        }

        private void HideQRCodeButton_Click(object sender, EventArgs e)
        {
            Protocol.HideQRCode();
        }

        private void ChangeQRCodeButton_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "图片文件|*.png;*.jpg;*.jpeg|全部文件|*.*",
                Multiselect = false,
                CheckFileExists = true
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                AlterQRCodePicture = File.ReadAllBytes(dialog.FileName);
                MessageBox.Show("图片已替换", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BotOnlineButton_Click(object sender, EventArgs e)
        {
            Protocol.SetProtocolOnline();
        }

        private void BotOfflineButton_Click(object sender, EventArgs e)
        {
            Protocol.SetProtocolOffline();
            LogHelper.Info("模拟离线", $"已触发离线事件，{AppConfig.Instance.ActionAfterOfflineSeconds}秒后执行离线操作");
        }

        private async void AudioButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "record"),
                Filter = "音频文件|*.mp3;*.wav;*.ogg|任何文件|*.*",
                Title = "选择音频",
                Multiselect = true,
                AddExtension = false
            };
            dialog.ShowDialog();
            foreach (var item in dialog.FileNames)
            {
                var path1 = Path.GetFullPath(item).TrimEnd(Path.DirectorySeparatorChar);
                var path2 = Path.GetFullPath(dialog.InitialDirectory).TrimEnd(Path.DirectorySeparatorChar);
                string path;
                if (path1.StartsWith(path2 + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                {
                    // 为同一目录
                    path = Helper.GetRelativePath(item, dialog.InitialDirectory);
                }
                else
                {
                    // 移动文件至默认目录
                    File.Copy(item, Path.Combine(dialog.InitialDirectory, Path.GetFileName(item)), true);
                    path = Path.GetFileName(item);
                }
                if (path.EndsWith(".cqrecord"))
                {
                    SendValue.Text += $"[CQ:record,file={Path.GetFileNameWithoutExtension(path)}] ";
                    continue;
                }
                string recordId = await ChatHistoryHelper.CacheMessageFile(CachedFileType.Record, $"{PicServer.Instance.ListenURL}{path}");
                SendValue.Text += $"[CQ:record,file={recordId}] ";
            }
        }

        #region MCP 服务器控制

        /// <summary>
        /// 初始化 MCP Tab 页的控件状态
        /// </summary>
        private void InitializeMCPTab()
        {
#if !NET9_0_OR_GREATER
            if (DesignMode)
            {
                return; // VS 设计器中保持控件可见，不做任何修改
            }
            // .NET Framework 4.8 运行时下禁用 MCP Tab
            tabPage4.Enabled = false;
            foreach (Control c in tabPage4.Controls)
            {
                c.Enabled = false;
            }
            MCPStatusLabel.Text = "状态: 不可用 (.NET 9 独占功能)";
            return;
#else
            if (Protocol == null)
            {
                return;
            }

            MCPEnableCheckBox.Checked = Protocol.MCPServerEnabled;
            MCPIPValue.Text = Protocol.MCPServerListenIP;
            MCPPortValue.Text = Protocol.MCPServerListenPort.ToString();

            MCPIPValue.Enabled = Protocol.MCPServerEnabled;
            MCPPortValue.Enabled = Protocol.MCPServerEnabled;
            MCPApplyButton.Enabled = Protocol.MCPServerEnabled;

            UpdateMCPStatusLabel();
#endif
        }

        /// <summary>
        /// 更新 MCP 状态标签
        /// </summary>
        private void UpdateMCPStatusLabel()
        {
#if NET9_0_OR_GREATER
            if (Protocol?.MCPServer?.Running == true)
            {
                MCPStatusLabel.Text = $"状态: 运行中 ({Protocol.MCPServer.ListenURL})";
                MCPStatusLabel.ForeColor = System.Drawing.Color.Green;
            }
            else if (Protocol?.MCPServerEnabled == true)
            {
                MCPStatusLabel.Text = "状态: 已启用（等待启动）";
                MCPStatusLabel.ForeColor = System.Drawing.Color.DarkOrange;
            }
            else
            {
                MCPStatusLabel.Text = "状态: 未启动";
                MCPStatusLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            }
#endif
        }

        /// <summary>
        /// MCP 启用复选框状态变更
        /// </summary>
        private void MCPEnableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
#if NET9_0_OR_GREATER
            bool enabled = MCPEnableCheckBox.Checked;
            MCPIPValue.Enabled = enabled;
            MCPPortValue.Enabled = enabled;
            MCPApplyButton.Enabled = enabled;

            if (Protocol != null)
            {
                Protocol.MCPServerEnabled = enabled;
                Protocol.SetConfig("MCPServerEnabled", enabled);
            }
#endif
        }

        /// <summary>
        /// MCP 配置值（IP/端口）文本变更时同步到配置文件
        /// </summary>
        private void MCPConfigValue_Changed(object sender, EventArgs e)
        {
#if NET9_0_OR_GREATER
            if (Protocol == null)
            {
                return;
            }

            if (sender == MCPIPValue)
            {
                Protocol.MCPServerListenIP = MCPIPValue.Text;
                Protocol.SetConfig("MCPServerListenIP", MCPIPValue.Text);
            }
            else if (sender == MCPPortValue)
            {
                if (ushort.TryParse(MCPPortValue.Text, out ushort port))
                {
                    Protocol.MCPServerListenPort = port;
                    Protocol.SetConfig("MCPServerListenPort", port);
                }
            }
#endif
        }

        /// <summary>
        /// MCP 应用按钮点击 - 重启 MCP 服务器以应用新配置
        /// </summary>
        private void MCPApplyButton_Click(object sender, EventArgs e)
        {
#if NET9_0_OR_GREATER
            if (Protocol == null)
            {
                return;
            }

            // 保存当前配置
            Protocol.MCPServerEnabled = MCPEnableCheckBox.Checked;
            Protocol.MCPServerListenIP = MCPIPValue.Text;
            if (ushort.TryParse(MCPPortValue.Text, out ushort port))
            {
                Protocol.MCPServerListenPort = port;
            }
            Protocol.SetConfig("MCPServerEnabled", Protocol.MCPServerEnabled);
            Protocol.SetConfig("MCPServerListenIP", Protocol.MCPServerListenIP);
            Protocol.SetConfig("MCPServerListenPort", Protocol.MCPServerListenPort);

            // 停止现有服务
            Protocol.MCPServer?.Stop();

            // 如果启用则启动新服务（通过动态加载 MCP 程序集）
            if (Protocol.MCPServerEnabled)
            {
                Protocol.MCPServer = new MCPServer(Protocol.MCPServerListenIP, Protocol.MCPServerListenPort, Protocol);
                Protocol.MCPServer?.Start();
                MCPStatusLabel.Text = "状态: 启动中...";
                MCPStatusLabel.ForeColor = System.Drawing.Color.DarkOrange;

                // 延迟更新状态
                Task.Run(async () =>
                {
                    await Task.Delay(2000);
                    if (Protocol?.MCPServer?.Running == true)
                    {
                        this.Invoke(() => UpdateMCPStatusLabel());
                    }
                });
            }
            else
            {
                Protocol.MCPServer = null;
                UpdateMCPStatusLabel();
            }
#endif
        }

        #endregion
    }
}
