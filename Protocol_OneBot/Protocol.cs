using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.Protocol.OneBot.Enums;
using Another_Mirai_Native.Protocol.OneBot.Messages;
using Another_Mirai_Native.Protocol.OneBot.Notice;
using Another_Mirai_Native.Protocol.OneBot.Requests;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Another_Mirai_Native.Protocol.OneBot
{
    public partial class OneBotAPI // 需实现Header
    {
        public WebSocketSharp.WebSocket APIClient { get; set; } = new("ws://127.0.0.1");

        public string AuthKey { get; set; } = "";

        public WebSocketSharp.WebSocket EventClient { get; set; } = new("ws://127.0.0.1");

        public bool ExitFlag { get; private set; }

        public int ReconnectCount { get; private set; }

        public Dictionary<int, WaitingMessage> WaitingMessages { get; set; } = new();

        /// <summary>
        /// 与OneBot通信的连接, 通常以ws开头
        /// </summary>
        public string WsURL { get; set; } = "";

        public JToken CallOneBotAPI(APIType type, Dictionary<string, object> param)
        {
            int syncId;
            do
            {
                syncId = Helper.MakeRandomID();
            } while (WaitingMessages.ContainsKey(syncId));
            object body = new
            {
                action = type.ToString(),
                echo = syncId,
                @params = param
            };
            return CallOneBotAPI(syncId, body);
        }

        public JToken CallOneBotAPI(int syncId, object obj)
        {
            var msg = new WaitingMessage();
            WaitingMessages.Add(syncId, msg);
            APIClient.Send(obj.ToJson());
            JObject result = null;
            for (int i = 0; i < AppConfig.PluginInvokeTimeout / 10; i++)
            {
                if (msg.Finished)
                {
                    result = msg.Result;
                    break;
                }
                Thread.Sleep(10);
            }
            if (result == null)
            {
                LogHelper.Debug("OneBotAPI", "Timeout");
            }
            else
            {
                if (result.ContainsKey("retcode") && result["retcode"].ToString() != "0")
                {
                    LogHelper.Debug("OneBotAPI", $"retcode: {result["retcode"]}");
                    result = null;
                }
            }
            return result["data"];
        }

        public bool ConnectAPIServer()
        {
            if (string.IsNullOrEmpty(WsURL))
            {
                LogHelper.Error("连接API服务器", "参数无效");
                return false;
            }
            string event_ConnectUrl = $"{WsURL}/api?access_token={AuthKey}";
            APIClient = new(event_ConnectUrl);
            APIClient.OnOpen += APIClient_OnOpen;
            APIClient.OnClose += APIClient_OnClose;
            APIClient.OnMessage += APIClient_OnMessage;
            APIClient.Connect();

            return APIClient.ReadyState == WebSocketSharp.WebSocketState.Open;
        }

        public bool ConnectEventServer()
        {
            if (string.IsNullOrEmpty(WsURL))
            {
                LogHelper.Error("连接事件服务器", "参数无效");
                return false;
            }
            string event_ConnectUrl = $"{WsURL}/event?access_token={AuthKey}";
            EventClient = new(event_ConnectUrl);
            EventClient.OnOpen += EventClient_OnOpen;
            EventClient.OnClose += EventClient_OnClose;
            EventClient.OnMessage += EventClient_OnMessage;
            EventClient.Connect();

            return EventClient.ReadyState == WebSocketSharp.WebSocketState.Open;
        }

        private void APIClient_OnClose(object? sender, WebSocketSharp.CloseEventArgs e)
        {
            if (ExitFlag)
            {
                return;
            }
            ReconnectCount++;
            IsConnected = APIClient.ReadyState == WebSocketSharp.WebSocketState.Open &&
              EventClient.ReadyState == WebSocketSharp.WebSocketState.Open;
            LogHelper.Error("API服务器连接断开", $"{AppConfig.ReconnectTime} ms后重新连接...");
            Thread.Sleep(AppConfig.ReconnectTime);
            ConnectEventServer();
        }

        private void APIClient_OnMessage(object? sender, WebSocketSharp.MessageEventArgs e)
        {
            Console.WriteLine($"[API]\t" + e.Data);
            Task.Run(() => HandleAPI(e.Data));
        }

        private void APIClient_OnOpen(object? sender, EventArgs e)
        {
            ReconnectCount = 0;
            LogHelper.WriteLog(LogLevel.Debug, "API服务器", "连接到API服务器");
            IsConnected = APIClient.ReadyState == WebSocketSharp.WebSocketState.Open &&
              EventClient.ReadyState == WebSocketSharp.WebSocketState.Open;
        }

        private void DispatchGroupMessage(JObject message)
        {
            GroupMessage groupMessage = message.ToObject<GroupMessage>();
            if (groupMessage == null)
            {
                return;
            }
            groupMessage.ParsedMessage = CQCode.Parse(groupMessage.raw_message);
            SaveCQCodeCache(groupMessage.ParsedMessage);
            groupMessage.raw_message = UnescapeRawMessage(groupMessage.raw_message);
            RequestCache.Message.Add((groupMessage.message_id, groupMessage.raw_message));
            while (RequestCache.Message.Count > AppConfig.MessageCache)
            {
                RequestCache.Message.RemoveAt(0);
            }
            Stopwatch sw = new();
            sw.Start();
            int logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "[↓]收到消息", $"群:{groupMessage.group_id} QQ:{groupMessage.user_id}({groupMessage.sender?.nickname}) {groupMessage.raw_message}", "处理中...");
            CQPluginProxy handledPlugin = PluginManagerProxy.Instance.InvokeEvent(PluginEventType.GroupMsg, 1, groupMessage.message_id, groupMessage.group_id, groupMessage.user_id, "", groupMessage.raw_message, 0);
            string updateMsg = $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s";
            if (handledPlugin != null)
            {
                updateMsg += $"(由 {handledPlugin.AppInfo.name} 结束消息处理)";
            }
            LogHelper.UpdateLogStatus(logId, updateMsg);
        }

        private string UnescapeRawMessage(string msg)
        {
            return msg.Replace("&#91;", "[").Replace("&#93;", "]").Replace("&#44;", ",").Replace("&amp;", "&");
        }

        private string EscapeRawMessage(string msg)
        {
            return msg.Replace("&", "&amp;").Replace("[", "&#91;").Replace("]", "&#93;").Replace(",", "&#44;");
        }

        private void DispatchMessage(JObject message)
        {
            if (message == null || message.ContainsKey("message_type") is false)
            {
                return;
            }
            string messageType = message["message_type"].ToString();
            if (messageType.ToString() == "group")
            {
                DispatchGroupMessage(message);
            }
            else if (messageType.ToString() == "private")
            {
                DispatchPrivateMessage(message);
            }
        }

        private void DispatchNotice(JObject notice)
        {
            if (notice == null || (!notice.ContainsKey("notice_type") && !notice.ContainsKey("sub_type")))
            {
                return;
            }
            NoticeType noticeType = (NoticeType)Enum.Parse(typeof(NoticeType), notice["notice_type"].ToString());
            NoticeType subType = NoticeType.notify;
            if (notice.ContainsKey("sub_type"))
            {
                subType = Enum.TryParse<NoticeType>(notice["sub_type"].ToString(), out NoticeType value) ? value : NoticeType.notify;
            }
            Stopwatch sw = new();
            sw.Start();

            int logId = 0;
            CQPluginProxy handledPlugin = null;
            switch (noticeType)
            {
                case NoticeType.notify:
                    switch (subType)
                    {
                        case NoticeType.poke:
                            Poke poke = notice.ToObject<Poke>();
                            logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "戳一戳", $"群:{poke.group_id} {poke.user_id} 戳了戳 {poke.target_id}", "处理中...");
                            break;

                        case NoticeType.lucky_king:
                            break;
                    }
                    break;

                case NoticeType.group_upload:
                    FileUpload fileUpload = notice.ToObject<FileUpload>();
                    MemoryStream stream = new();
                    BinaryWriter binaryWriter = new(stream);
                    BinaryWriterExpand.Write_Ex(binaryWriter, fileUpload.file.id);
                    BinaryWriterExpand.Write_Ex(binaryWriter, fileUpload.file.name);
                    BinaryWriterExpand.Write_Ex(binaryWriter, fileUpload.file.size);
                    BinaryWriterExpand.Write_Ex(binaryWriter, fileUpload.file.busid);
                    PluginManagerProxy.Instance.InvokeEvent(PluginEventType.Upload, 1, Helper.TimeStamp, fileUpload.group_id, fileUpload.user_id, Convert.ToBase64String(stream.ToArray()));
                    sw.Stop();
                    LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "文件上传", $"来源群:{fileUpload.group_id} 来源QQ:{fileUpload.user_id} " +
                        $"文件名:{fileUpload.file.name} 大小:{fileUpload.file.size / 1000}KB FileID:{fileUpload.file.id}", $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s");
                    return;

                case NoticeType.group_admin:
                    AdminChange adminChange = notice.ToObject<AdminChange>();
                    int adminSet = adminChange.sub_type == "set" ? 2 : 1;
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群员权限变更", $"群:{adminChange.group_id} QQ:{adminChange.user_id} 被{(adminSet == 2 ? "设置为" : "取消")}管理员", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.InvokeEvent(PluginEventType.AdminChange, adminSet, adminChange.time, adminChange.group_id, adminChange.user_id);
                    break;

                case NoticeType.group_decrease:
                    GroupMemberLeave leave = notice.ToObject<GroupMemberLeave>();
                    switch (leave.sub_type)
                    {
                        case "leave":
                            logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群成员离开", $"群:{leave.group_id} QQ:{leave.user_id}", "处理中...");
                            break;

                        case "kick":
                            logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群成员被踢出", $"群:{leave.group_id} QQ:{leave.user_id} 操作者:{leave.operator_id}", "处理中...");
                            break;

                        case "kick_me":
                            logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "Bot被踢出群聊", $"群:{leave.group_id} 操作人: {leave.operator_id}", "处理中...");
                            break;
                    }
                    handledPlugin = PluginManagerProxy.Instance.InvokeEvent(PluginEventType.GroupMemberDecrease, 1, Helper.TimeStamp, leave.group_id, leave.operator_id, leave.user_id);
                    break;

                case NoticeType.group_increase:
                    GroupMemberJoin join = notice.ToObject<GroupMemberJoin>();
                    int groupInviteType = join.sub_type == "approve" ? 1 : 2;
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群成员增加", $"群:{join.group_id} QQ:{join.user_id} 操作人: {join.operator_id}", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.InvokeEvent(PluginEventType.GroupMemberIncrease, groupInviteType, join.time, join.group_id, join.operator_id, join.user_id);
                    break;

                case NoticeType.group_ban:
                    GroupBan ban = notice.ToObject<GroupBan>();
                    int banId = ban.sub_type == "ban" ? 2 : 1;
                    switch (ban.sub_type)
                    {
                        case "ban":
                            logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群员被禁言", $"群:{ban.group_id} QQ:{ban.user_id} 禁言时长:{ban.duration} 秒 操作人:{ban.operator_id}", "处理中...");
                            break;

                        case "lift_ban":
                            logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群员被解除禁言", $"群:{ban.group_id} QQ:{ban.user_id} 操作人:{ban.operator_id}", "处理中...");
                            break;
                    }
                    handledPlugin = PluginManagerProxy.Instance.InvokeEvent(PluginEventType.GroupBan, banId, Helper.TimeStamp, ban.group_id, ban.operator_id, ban.user_id, ban.duration);
                    break;

                case NoticeType.friend_add:
                    FriendAdd friendAdd = notice.ToObject<FriendAdd>();
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "好友添加", $"QQ:{friendAdd.user_id}", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.InvokeEvent(PluginEventType.FriendAdded, subType, friendAdd.time, friendAdd.user_id);
                    break;

                case NoticeType.group_recall:
                    GroupMessageRecall groupMessageRecall = notice.ToObject<GroupMessageRecall>();
                    string msg = "内容未捕获";
                    var msgCache = RequestCache.Message.Last(x => x.Item1 == groupMessageRecall.message_id);
                    if (!string.IsNullOrEmpty(msgCache.Item2))
                    {
                        msg = msgCache.Item2;
                    }
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群消息撤回", $"群:{groupMessageRecall.group_id} 内容:{msg}", "处理中...");
                    break;

                case NoticeType.friend_recall:
                    FriendMessageRecall friendMessageRecall = notice.ToObject<FriendMessageRecall>();
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "好友消息撤回", $"QQ:{friendMessageRecall.user_id} 内容:{friendMessageRecall.message_id}", "处理中...");
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

        private void DispatchPrivateMessage(JObject message)
        {
            PrivateMessage privateMessage = message.ToObject<PrivateMessage>();
            if (privateMessage == null)
            {
                return;
            }
            privateMessage.ParsedMessage = CQCode.Parse(privateMessage.raw_message);
            privateMessage.raw_message = UnescapeRawMessage(privateMessage.raw_message);
            SaveCQCodeCache(privateMessage.ParsedMessage);
            RequestCache.Message.Add((privateMessage.message_id, privateMessage.raw_message));
            while (RequestCache.Message.Count > AppConfig.MessageCache)
            {
                RequestCache.Message.RemoveAt(0);
            }
            Stopwatch sw = new();
            sw.Start();
            int logId = 0;
            CQPluginProxy handledPlugin = null;
            switch (privateMessage.sub_type)
            {
                case "friend":
                    logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "[↓]收到好友消息", $"QQ:{privateMessage.user_id}({privateMessage.sender?.nickname}) {privateMessage.raw_message}", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.InvokeEvent(PluginEventType.PrivateMsg, 11, privateMessage.message_id, privateMessage.user_id, privateMessage.raw_message, 0);
                    break;

                case "group":
                    logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "[↓]收到群临时消息", $"群:{privateMessage.sender?.group_id} QQ:{privateMessage.user_id}({privateMessage.sender?.nickname}) {privateMessage.raw_message}", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.InvokeEvent(PluginEventType.PrivateMsg, 2, privateMessage.message_id, privateMessage.user_id, privateMessage.raw_message, 0);
                    break;

                case "other":
                    logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "[↓]收到陌生人消息", $"QQ:{privateMessage.user_id}({privateMessage.sender?.nickname}) {privateMessage.raw_message}", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.InvokeEvent(PluginEventType.PrivateMsg, 1, privateMessage.message_id, privateMessage.user_id, privateMessage.raw_message, 0);
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

        private void DispatchRequest(JObject request)
        {
            if (request == null)
            {
                return;
            }
            Stopwatch sw = new();
            sw.Start();

            int logId = 0;
            CQPluginProxy handledPlugin = null;
            switch (request["request_type"].ToString())
            {
                case "group":
                    GroupRequest groupRequest = request.ToObject<GroupRequest>();
                    RequestCache.GroupRequest.Add(groupRequest.flag, (groupRequest.user_id, "", groupRequest.group_id, ""));
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "添加群请求", $"群:{groupRequest.group_id} QQ:{groupRequest.user_id} 备注:{groupRequest.comment}", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.InvokeEvent(PluginEventType.GroupAddRequest, 1, groupRequest.time, groupRequest.group_id, groupRequest.user_id, groupRequest.comment, groupRequest.comment);
                    break;

                case "friend":
                    FriendRequest friendRequest = request.ToObject<FriendRequest>();
                    RequestCache.FriendRequest.Add(friendRequest.flag, (friendRequest.user_id, ""));
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "添加好友请求", $"QQ:{friendRequest.user_id} 备注:{friendRequest.comment} 来源群:{friendRequest.group_id}", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.InvokeEvent(PluginEventType.FriendRequest, 1, friendRequest.time, friendRequest.user_id, friendRequest.comment, friendRequest.flag);
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

        private void EventClient_OnClose(object? sender, WebSocketSharp.CloseEventArgs e)
        {
            if (ExitFlag)
            {
                return;
            }
            ReconnectCount++;
            IsConnected = APIClient.ReadyState == WebSocketSharp.WebSocketState.Open &&
               EventClient.ReadyState == WebSocketSharp.WebSocketState.Open;
            LogHelper.Error("事件服务器连接断开", $"{AppConfig.ReconnectTime} ms后重新连接...");
            Thread.Sleep(AppConfig.ReconnectTime);
            ConnectEventServer();
        }

        private void EventClient_OnMessage(object? sender, WebSocketSharp.MessageEventArgs e)
        {
            Console.WriteLine($"[Event]\t" + e.Data);
            Task.Run(() => HandleEvent(e.Data));
        }

        private void EventClient_OnOpen(object? sender, EventArgs e)
        {
            ReconnectCount = 0;
            LogHelper.WriteLog(LogLevel.Debug, "事件服务器", "连接到事件服务器");
            IsConnected = APIClient.ReadyState == WebSocketSharp.WebSocketState.Open &&
                   EventClient.ReadyState == WebSocketSharp.WebSocketState.Open;
        }

        private void HandleAPI(string data)
        {
            try
            {
                // data = data.Replace("msgId", "message_id").Replace("Id", "_id").Replace("retCode", "retcode").Replace("Type", "_type").Replace("Message", "_message");
                JObject json = JObject.Parse(data);
                if (json.ContainsKey("echo"))
                {
                    int echo = int.TryParse(json["echo"].ToString(), out int value) ? value : 0;
                    if (WaitingMessages.ContainsKey(echo))
                    {
                        WaitingMessages[echo].Result = json;
                        WaitingMessages[echo].Finished = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("处理API", ex);
            }
        }

        private void HandleEvent(string data)
        {
            try
            {
                // data = data.Replace("msgId", "message_id").Replace("Id", "_id").Replace("retCode", "retcode").Replace("Type", "_type").Replace("Message", "_message");
                JObject e = JObject.Parse(data);
                if (e.ContainsKey("post_type") is false)
                {
                    return;
                }
                EventType eventTypes = (EventType)Enum.Parse(typeof(EventType), e["post_type"].ToString());
                switch (eventTypes)
                {
                    case EventType.message:
                        DispatchMessage(e);
                        break;

                    case EventType.notice:
                        DispatchNotice(e);
                        break;

                    case EventType.request:
                        DispatchRequest(e);
                        break;

                    case EventType.meta_event: // Ignore
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("处理消息", ex);
            }
        }

        private void SaveCQCodeCache(List<CQCode> cqCodes)
        {
            foreach (var item in cqCodes)
            {
                if (item.IsImageCQCode)
                {
                    string imgId = item.Items["file"].Split('.').First();
                    Directory.CreateDirectory("data\\image");
                    File.WriteAllText($"data\\image\\{imgId}.cqimg", $"[image]\nmd5=0\nsize=0\nurl={(item.Items.ContainsKey("url") ? item.Items["url"] : "")}");
                }
                else if (item.IsRecordCQCode)
                {
                    string voiceId = item.Items["file"].Replace(".amr", "");
                    Directory.CreateDirectory("data\\record");
                    File.WriteAllText($"data\\record\\{voiceId}.cqrecord", $"[record]\nurl={(item.Items.ContainsKey("url") ? item.Items["url"] : "")}");
                }
            }
        }
    }

    public class WaitingMessage
    {
        public bool Finished { get; set; }

        public JObject Result { get; set; }
    }
}