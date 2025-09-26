using Another_Mirai_Native;
using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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

        private int AutoRecallInteval => int.TryParse(AutoRecallValue.Text, out int value) ? value : -1;

        private bool UseCustomMessageID => UseMessageID.Checked;

        private int CustomMessageID => int.TryParse(MessageIDValue.Text, out int value) ? value : -1;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Protocol Protocol { get; set; }

        private byte[] AlterQRCodePicture { get; set; } = [];

        private void Tester_Load(object sender, EventArgs e)
        {
            GroupValue.Text = CommonConfig.GetConfig("TesterGroup", @"conf/Test.json", (long)1919810).ToString();
            QQValue.Text = CommonConfig.GetConfig("TesterQQ", @"conf/Test.json", (long)1145141919).ToString();
            MessageHistories = CommonConfig.GetConfig("MessageHistories", @"conf/Test.json", new List<string>());

            PicButton.Enabled = PicServer.Instance?.Running ?? false;
            SendValue.Focus();
        }

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
                    if (AutoRecall is false || AutoRecallInteval < 0)
                    {
                        return;
                    }
                    Thread.Sleep((int)TimeSpan.FromSeconds(AutoRecallInteval).TotalMilliseconds);
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

        private void PicButton_Click(object sender, EventArgs e)
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
                string fileName = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
                if (path.EndsWith(".cqimg"))
                {
                    SendValue.Text += $"[CQ:image,file={Path.GetFileNameWithoutExtension(path)}] ";
                    continue;
                }
                File.WriteAllText(Path.Combine(dialog.InitialDirectory, fileName + ".cqimg"), $"[url]\r\nmd5=0\r\nsize=0\r\nurl={PicServer.Instance.ListenURL}{path}");
                SendValue.Text += $"[CQ:image,file={fileName}] ";
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
    }
}
