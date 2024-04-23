using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.Protocol.Satori.Enums;
using Another_Mirai_Native.Protocol.Satori.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using WebSocketSharp;
using LogLevel = Another_Mirai_Native.Model.Enums.LogLevel;

namespace Another_Mirai_Native.Protocol.Satori
{
    public partial class Protocol
    {
        public WebSocketSharp.WebSocket EventClient { get; set; } = new("ws://127.0.0.1");

        public bool ExitFlag { get; private set; }

        public int ReconnectCount { get; private set; }

        public string WsURL { get; private set; }

        public string CurrentPlatform { get; set; }

        public string Token { get; set; }

        private bool ConnectEventServer()
        {
            if (string.IsNullOrEmpty(WsURL))
            {
                LogHelper.Error("连接事件服务器", "参数无效");
                return false;
            }
            string event_ConnectUrl = $"{WsURL}/v1/events";
            EventClient = new(event_ConnectUrl);
            EventClient.OnOpen += EventClient_OnOpen;
            EventClient.OnClose += EventClient_OnClose;
            EventClient.OnMessage += EventClient_OnMessage;
            EventClient.Connect();

            bool waitFlag = RequestWaiter.Wait("Satori_Identity", EventClient, int.MaxValue, out object success);
            return (bool)success && EventClient.ReadyState == WebSocketState.Open;
        }

        private void EventClient_OnMessage(object? sender, MessageEventArgs e)
        {
            LogHelper.Debug("Event", e.Data);
            Task.Run(() => HandleServerMessage(e.Data));
        }

        private void HandleServerMessage(string data)
        {
            try
            {
                ServerMessage e = JsonConvert.DeserializeObject<ServerMessage>(data);

                switch (e.op)
                {
                    case EventOp.EVENT:
                        HandleEvent(e.body);
                        break;

                    case EventOp.READY:
                        HandleReady(e.body["logins"]);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("处理事件", data);
                LogHelper.Error("处理事件", ex);
            }
        }

        private void HandleReady(JToken body)
        {
            if (body is JArray array)
            {
                foreach (var item in array)
                {
                    var login = item.ToObject<Login>();
                    if (login == null
                        || login.status != LoginStatus.ONLINE
                        || !long.TryParse(login.self_id, out long qq)
                        || string.IsNullOrEmpty(login.platform))
                    {
                        continue;
                    }
                    AppConfig.Instance.CurrentQQ = qq;
                    CurrentPlatform = login.platform;
                }
                if (AppConfig.Instance.CurrentQQ != 0 && !string.IsNullOrEmpty(CurrentPlatform))
                {
                    StartHeartBeat();
                }
                RequestWaiter.TriggerByKey("Satori_Identity", true);
            }
            else
            {
                LogHelper.Error("鉴权", "无法获取当前登录实例");
                EventClient.Close();
                RequestWaiter.ResetSignalByWebSocket(EventClient);
            }
        }

        private void StartHeartBeat()
        {
            new Thread(() =>
            {
                while (IsConnected && !ExitFlag)
                {
                    Thread.Sleep(10 * 1000);
                    EventClient.Send(new
                    {
                        op = (int)EventOp.PING
                    }.ToJson());
                }
            }).Start();
        }

        private void HandleEvent(JToken body)
        {
            Events e = body.ToObject<Events>();
            if (e == null)
            {
                LogHelper.Error("处理事件", "无法解析事件对象");
                return;
            }
            Stopwatch sw = new();
            sw.Start();

            int logId = 0;
            CQPluginProxy handledPlugin = null;
            switch (e.type)
            {
                case "friend-request":
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "添加好友请求", $"QQ:{e.user.id}({e.user.nick})", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.Event_OnFriendAddRequest(1, e.timestamp, long.Parse(e.user.id), "", e.id.ToString());
                    break;
                case "guild-added":
                    break;
                case "guild-member-added":
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群成员增加", $"群:{e.guild.id}({e.guild.name}) QQ:{e.user.id}({e.user.nick}) 操作人: {e.opeator.id}({e.opeator.nick})", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.Event_OnGroupMemberIncrease(1, e.timestamp, long.Parse(e.guild.id), long.Parse(e.opeator.id), long.Parse(e.user.id));
                    break;
                case "guild-member-removed":
                    if (e.opeator == null)
                    {
                        logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群成员离开", $"群:{e.guild.id}({e.guild.name}) QQ:{e.user.id}({e.user.nick})", "处理中...");
                        handledPlugin = PluginManagerProxy.Instance.Event_OnGroupMemberDecrease(1, Helper.TimeStamp, long.Parse(e.guild.id), 0, long.Parse(e.user.id));
                    }
                    else
                    {
                        logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群成员被踢出", $"群:{e.guild.id}({e.guild.name}) QQ:{e.user.id}({e.user.nick}) 操作者:{e.opeator.id}({e.opeator.nick})", "处理中...");
                        handledPlugin = PluginManagerProxy.Instance.Event_OnGroupMemberDecrease(2, Helper.TimeStamp, long.Parse(e.guild.id), long.Parse(e.opeator.id), long.Parse(e.user.id));
                    }
                    break;
                case "guild-member-request":
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "添加群请求", $"群:{e.guild.id}({e.guild.name}) QQ:{e.user.id}({e.user.nick})", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.Event_OnGroupAddRequest(1, e.timestamp, long.Parse(e.guild.id), long.Parse(e.user.id), "", "");
                    break;
                case "guild-member-updated":
                    break;
                case "guild-removed":
                    break;
                case "guild-request":
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "添加群请求", $"群:{e.guild.id}({e.guild.name}) QQ:{e.user.id}({e.user.nick})", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.Event_OnGroupAddRequest(1, e.timestamp, long.Parse(e.guild.id), long.Parse(e.user.id), "", e.id.ToString());
                    break;
                case "guild-role-updated":
                case "guild-role-created":
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群员权限变更", $"群:{e.guild.id}({e.guild.name}) QQ:{e.user.id}({e.user.nick}) 被设置为管理员", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.Event_OnAdminChange(2, e.timestamp, long.Parse(e.guild.id), long.Parse(e.user.id));
                    break;
                case "guild-role-deleted":
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群员权限变更", $"群:{e.guild.id}({e.guild.name}) QQ:{e.user.id}({e.user.nick}) 被取消管理员", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.Event_OnAdminChange(1, e.timestamp, long.Parse(e.guild.id), long.Parse(e.user.id));
                    break;
                case "guild-updated":
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群成员名片改变", $"群:{e.guild.id}({e.guild.name}) QQ:{e.user.id}({e.user.nick}) 名片:{e.user.nick}", "处理中...");
                    PluginManagerProxy.Instance.Event_OnGroupMemberCardChanged(long.Parse(e.guild.id), long.Parse(e.user.id), e.user.nick);
                    break;
                case "login-added":
                    break;
                case "login-removed":
                    break;
                case "login-updated":
                    break;
                case "message-created":
                    HandleMessage(e.guild, e.user, e.member, e.message);
                    break;
                case "message-deleted":
                    string msg = "内容未捕获";
                    int messageId = GetMessageIdFromDB(e.message.id, out _);
                    var msgCache = RequestCache.Message.Last(x => x.Item1 == messageId);
                    if (!string.IsNullOrEmpty(msgCache.Item2))
                    {
                        msg = msgCache.Item2;
                    }
                    if (e.guild != null)
                    {
                        PluginManagerProxy.Instance.Event_OnGroupMsgRecall(messageId, long.Parse(e.guild.id), msg);
                        logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群消息撤回", $"群:{e.guild.id}({e.guild.name}) 内容:{msg}", "处理中...");
                    }
                    else if (e.user != null)
                    {
                        PluginManagerProxy.Instance.Event_OnPrivateMsgRecall(messageId, long.Parse(e.user.id), msg);
                        logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "好友消息撤回", $"QQ:{e.user.id}({e.user.nick}) 内容:{msg}", "处理中...");
                    }
                    break;
                case "message-updated":
                    break;
                case "reaction-added":
                    break;
                case "reaction-removed":
                    break;
                case "internal":
                    break;
                case "interaction/button":
                    break;
                case "interaction/command":
                    break;
                default:
                    break;
            }
            sw.Stop();
            string updateMsg = $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s";
            if (handledPlugin != null)
            {
                updateMsg += $"(由 {handledPlugin.AppInfo.name} 结束消息处理)";
            }
            LogHelper.UpdateLogStatus(logId, updateMsg);
        }

        int i = 0;
        private int GetMessageIdFromDB(string id, out long parentId)
        {
            parentId = 0;
            return i++;
        }

        private int SaveMessageIdInDB(string id)
        {
            return i++;
        }

        private void HandleMessage(Guild? guild, User? user, GuildMember? member, Message? rawMessage)
        {
            if (rawMessage == null)
            {
                return;
            }
            int msgId = SaveMessageIdInDB(rawMessage.id);
            string message = CQCodeBuilder.RawParseToCQCode(rawMessage.content, CurrentPlatform);
            Stopwatch sw = new();
            sw.Start();

            int logId = 0;
            CQPluginProxy handledPlugin = null;
            string nick = member != null ? member.nick : !string.IsNullOrEmpty(user?.name) ? user.name : "";
            long groupId = long.TryParse(guild?.id, out long value) ? value : 0;
            long qqId = long.TryParse(user?.id, out value) ? value : 0;
            if (!string.IsNullOrEmpty(message) && guild == null && user != null && qqId > 0)
            {
                logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "[↓]收到好友消息", $"QQ:{user.id}({nick}) {message}", "处理中...");
                handledPlugin = PluginManagerProxy.Instance.Event_OnPrivateMsg(11, msgId, qqId, message, 0, DateTime.Now);
            }
            else if (!string.IsNullOrEmpty(message) && guild != null && user != null && groupId > 0 && qqId > 0)
            {
                logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "[↓]收到消息", $"群:{guild.id}({guild.name}) QQ:{user.id}({nick}) {message}", "处理中...");
                handledPlugin = PluginManagerProxy.Instance.Event_OnGroupMsg(1, msgId, groupId, qqId, "", message, 0, DateTime.Now);
            }
            sw.Stop();
            string updateMsg = $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s";
            if (handledPlugin != null)
            {
                updateMsg += $"(由 {handledPlugin.AppInfo.name} 结束消息处理)";
            }
            if (logId > 0)
            {
                LogHelper.UpdateLogStatus(logId, updateMsg);
            }
        }

        public static string UnescapeRawMessage(string msg)
        {
            return msg.Replace("&#91;", "[").Replace("&#93;", "]").Replace("&#44;", ",").Replace("&amp;", "&");
        }

        public static string EscapeRawMessage(string msg)
        {
            return msg.Replace("&", "&amp;").Replace("[", "&#91;").Replace("]", "&#93;").Replace(",", "&#44;");
        }

        private void EventClient_OnClose(object? sender, CloseEventArgs e)
        {
            AppConfig.Instance.CurrentQQ = 0;
            CurrentPlatform = "";
            RequestWaiter.ResetSignalByWebSocket(EventClient);
            if (ExitFlag)
            {
                return;
            }
            ReconnectCount++;
            RequestWaiter.ResetSignalByWebSocket(EventClient);
            LogHelper.Error("事件服务器连接断开", $"{AppConfig.Instance.ReconnectTime} ms后重新连接...");
            Thread.Sleep(AppConfig.Instance.ReconnectTime);
            ConnectEventServer();
        }

        private void EventClient_OnOpen(object? sender, EventArgs e)
        {
            ReconnectCount = 0;
            LogHelper.Info("事件服务器", "成功连接到事件服务器，开始鉴权");
            SendIdentity();
        }

        private void SendIdentity()
        {
            if (IsConnected == false)
            {
                LogHelper.Error("鉴权", "连接未建立，无法发送鉴权消息");
                return;
            }
            EventClient.Send(new
            {
                op = (int)EventOp.IDENTIFY,
                body = new Identity
                {
                    token = Token
                }
            }.ToJson());
        }
    }
}
