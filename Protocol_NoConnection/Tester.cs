﻿using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private bool AutoRecall { get; set; } = false;

        private void Tester_Load(object sender, EventArgs e)
        {
            GroupValue.Text = CommonConfig.GetConfig("TesterGroup", @"conf/Test.json", (long)10001).ToString();
            QQValue.Text = CommonConfig.GetConfig("TesterQQ", @"conf/Test.json", (long)10001).ToString();
            MessageHistories = CommonConfig.GetConfig("MessageHistories", @"conf/Test.json", new List<string>());
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
                int msgId = MsgId++;
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
                new Thread(() =>
                {
                    if (AutoRecall is false)
                    {
                        return;
                    }
                    Thread.Sleep(3000);
                    if (PrivateSelector.Checked)
                    {
                        PluginManagerProxy.Instance.Event_OnPrivateMsgRecall(msgId, QQId, msg);
                    }
                    else
                    {
                        PluginManagerProxy.Instance.Event_OnGroupMsgRecall(msgId, GroupId, msg);
                    }
                }).Start();
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
    }
}
