using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Enums;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.Protocol.MiraiAPIHttp.MiraiAPIResponse;
using Another_Mirai_Native.RPC.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Protocol.MiraiAPIHttp
{
    public partial class Protocol
    {
        /// <summary>
        /// 退出Flag
        /// </summary>
        public static bool ExitFlag { get; set; }

        /// <summary>
        /// MHA配置中所定义的SecretKey
        /// </summary>
        public string AuthKey { get; set; }

        public WebSocketClient EventConnection { get; set; } = new("ws://127.0.0.1");

        public WebSocketClient MessageConnection { get; set; } = new("ws://127.0.0.1");

        private Task HeartBeatTask { get; set; }

        /// <summary>
        /// Mirai框架中登录中 且 希望控制逻辑的QQ号
        /// </summary>
        public long QQ { get; set; }

        /// <summary>
        /// 重连次数
        /// </summary>
        public int ReconnectCount { get; set; }

        /// <summary>
        /// 保存事件服务器的SessionKey
        /// </summary>
        public string SessionKey_Event { get; set; }

        /// <summary>
        /// 保存消息服务器的SessionKey
        /// </summary>
        public string SessionKey_Message { get; set; }

        /// <summary>
        /// 待返回的队列
        /// </summary>
        public Dictionary<string, WaitingMessage> WaitingMessages { get; set; } = new();

        /// <summary>
        /// 与MHA通信的连接, 通常以ws开头
        /// </summary>
        public string WsURL { get; set; }

        public JObject CallMiraiAPI(MiraiApiType type, object data)
        {
            string apiType = Enum.GetName(typeof(MiraiApiType), type);
            string command = apiType, subCommand = "";
            if (apiType.Contains('_'))// 子命令
            {
                var c = apiType.Split('_');
                command = c[0];
                subCommand = c[1];
            }
            string syncId;
            do
            {
                syncId = Helper.MakeUniqueID().ToString();
            } while (WaitingMessages.ContainsKey(syncId));
            object body = new
            {
                syncId,
                command,
                subCommand,
                content = data
            };
            return CallMiraiAPI(syncId, body, type == MiraiApiType.botProfile 
                || type == MiraiApiType.friendProfile 
                || type == MiraiApiType.memberProfile 
                || type == MiraiApiType.userProfile);
        }

        public JObject CallMiraiAPI(string syncId, object obj, bool isProfilerRequest = false)
        {
            var msg = new WaitingMessage
            {
                ProfilerRequest = isProfilerRequest
            };
            WaitingMessages.Add(syncId, msg);
            MessageConnection.Send(obj.ToJson());
            if (RequestWaiter.Wait(syncId, MessageConnection, AppConfig.Instance.PluginInvokeTimeout, out _))
            {
                WaitingMessages.Remove(syncId);
                return msg.Result;
            }
            LogHelper.Debug("调用MiraiAPI", "Timeout");
            return null;
        }

        private bool ConnectEventServer()
        {
            if (string.IsNullOrEmpty(WsURL) || string.IsNullOrEmpty(AuthKey) || QQ < 0)
            {
                LogHelper.Error("连接事件服务器", "参数无效");
                return false;
            }

            string event_ConnectUrl = $"{WsURL}/event?verifyKey={AuthKey}&qq={QQ}";
            EventConnection = new(event_ConnectUrl);
            var waitTask = Task.Run(() =>
            {
                RequestWaiter.Wait("MAH_EventAuthKey", AppConfig.Instance.LoadTimeout, out _);
            });
            EventConnection.OnOpen += EventConnection_OnOpen;
            EventConnection.OnClose += EventConnection_OnClose;
            EventConnection.OnMessage += EventConnection_OnMessage;
            EventConnection.Connect();
            StartHeartBeatTask();
            waitTask.Wait();
            return EventConnection.ReadyState == WebSocketState.Open;
        }

        private void StartHeartBeatTask()
        {
            if (HeartBeatTask != null)
            {
                return;
            }
            HeartBeatTask = Task.Factory.StartNew(() =>
            {
                while (IsConnected)
                {
                    Thread.Sleep(AppConfig.Instance.HeartBeatInterval);
                    EventConnection.Send(Array.Empty<byte>());
                    MessageConnection.Send(Array.Empty<byte>());
                }
                HeartBeatTask = null;
            });
        }

        private void EventConnection_OnMessage(string message)
        {
            LogHelper.Debug("Event", message);
            Task.Run(() => HandleEvent(message));
        }

        private void EventConnection_OnClose()
        {
            if (ExitFlag)
            {
                return;
            }
            ReconnectCount++;
            LogHelper.Error("事件服务器连接断开", $"{AppConfig.Instance.ReconnectTime} ms后重新连接...");
            RequestWaiter.ResetSignalByConnection(EventConnection);
            Thread.Sleep(AppConfig.Instance.ReconnectTime);
            ConnectEventServer();
        }

        private void EventConnection_OnOpen()
        {
            ReconnectCount = 0;
            LogHelper.Info("事件服务器", "成功连接到事件服务器");
        }

        private bool ConnectMessageServer()
        {
            if (string.IsNullOrEmpty(WsURL) || string.IsNullOrEmpty(AuthKey) || QQ < 0)
            {
                LogHelper.Error("连接消息服务器", "参数无效");
                return false;
            }

            string message_ConnectUrl = $"{WsURL}/message?verifyKey={AuthKey}&qq={QQ}";
            MessageConnection = new(message_ConnectUrl);
            var waitTask = Task.Run(() =>
            {
                RequestWaiter.Wait("MAH_MessageAuthKey", AppConfig.Instance.LoadTimeout, out _);
            });
            MessageConnection.OnOpen += MessageConnection_OnOpen;
            MessageConnection.OnClose += MessageConnection_OnClose;
            MessageConnection.OnMessage += MessageConnection_OnMessage;
            MessageConnection.Connect();
            StartHeartBeatTask();
            waitTask.Wait();
            return MessageConnection.ReadyState == WebSocketState.Open;
        }

        private void MessageConnection_OnMessage(string message)
        {
            LogHelper.Debug("Message", message);
            Task.Run(() => HandleMessage(message));
        }

        private void MessageConnection_OnClose()
        {
            if (ExitFlag)
            {
                return;
            }
            ReconnectCount++;
            LogHelper.Error("消息服务器连接断开", $"{AppConfig.Instance.ReconnectTime} ms后重新连接...");
            RequestWaiter.ResetSignalByConnection(MessageConnection);
            Thread.Sleep(AppConfig.Instance.ReconnectTime);
            ConnectMessageServer();
        }

        private void MessageConnection_OnOpen()
        {
            ReconnectCount = 0;
            LogHelper.Info("消息服务器", "成功连接到消息服务器");
        }

        private void HandleEvent(string message)
        {
            try
            {
                Console.WriteLine(message);
                var api = JsonConvert.DeserializeObject<APIResponse>(message);
                if (api.data == null)
                {
                    return;
                }
                var data = JObject.FromObject(api.data);
                if (string.IsNullOrEmpty(SessionKey_Event))
                {
                    var sessionKey = data.ToObject<SessionKey>();
                    if (sessionKey.code == 0)
                    {
                        SessionKey_Event = sessionKey.session;
                        RequestWaiter.TriggerByKey("MAH_EventAuthKey");
                    }
                    else
                    {
                        LogHelper.Error("事件SessionKey", $"code: {sessionKey.code} msg: {sessionKey.msg}");
                    }
                    return;
                }
                ParseAndDispatchEvent(data);
            }
            catch (Exception ex)
            {
                LogHelper.Error("处理事件", ex);
            }
        }

        private void HandleMessage(string message)
        {
            try
            {
                var api = JsonConvert.DeserializeObject<APIResponse>(message);
                if (api.data == null)
                {
                    return;
                }
                var data = JObject.FromObject(api.data);
                if (string.IsNullOrEmpty(SessionKey_Message))
                {
                    var sessionKey = data.ToObject<SessionKey>();
                    if (sessionKey.code == 0)
                    {
                        SessionKey_Message = sessionKey.session;
                        RequestWaiter.TriggerByKey("MAH_MessageAuthKey");
                    }
                    else
                    {
                        LogHelper.Error("消息SessionKey", $"code: {sessionKey.code} msg: {sessionKey.msg}");
                    }
                    return;
                }
                if (data.ContainsKey("code")) // 为API调用结果，根据 syncID 查询转换类型
                {
                    if (WaitingMessages.TryGetValue(api.syncId, out var value))
                    {
                        var waiting = value;
                        waiting.Result = data;
                        waiting.Finished = true;
                        RequestWaiter.TriggerByKey(api.syncId);
                    }
                }
                else
                {
                    if (data.ContainsKey("nickname"))
                    {
                        var profilerRequest = WaitingMessages.FirstOrDefault(x => x.Value.ProfilerRequest);
                        if (!string.IsNullOrEmpty(profilerRequest.Key))
                        {
                            profilerRequest.Value.Result = data;
                            profilerRequest.Value.Finished = true;
                            RequestWaiter.TriggerByKey(profilerRequest.Key);
                            return;
                        }
                    }
                    ParseAndDispatchMessage(data);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("处理消息", ex);
            }
        }

        private void ParseAndDispatchEvent(JObject msg)
        {
            if (PluginManagerProxy.Instance == null)
            {
                return;
            }
            Stopwatch sw = new();
            sw.Start();
            MiraiEvents events = Helper.String2Enum<MiraiEvents>(msg["type"].ToString());
            int logId = 0;
            CQPluginProxy handledPlugin = null;
            switch (events)
            {
                case MiraiEvents.BotOnlineEvent:
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "Bot登录成功", $"登录成功", "处理中...");
                    break;

                case MiraiEvents.BotOfflineEventActive:
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "Bot主动离线", $"主动离线", "处理中...");
                    break;

                case MiraiEvents.BotOfflineEventForce:
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "Bot被挤下线", $"被动离线", "处理中...");
                    break;

                case MiraiEvents.BotOfflineEventDropped:
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "Bot掉线", $"被服务器断开或因网络问题而掉线", "处理中...");
                    break;

                case MiraiEvents.BotReloginEvent:
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "Bot主动重新登录", $"主动重新登录", "处理中...");
                    break;

                case MiraiEvents.FriendInputStatusChangedEvent:
                    var friendInputStatusChangedEvent = msg.ToObject<FriendInputStatusChangedEvent>();
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "好友输入状态改变", $"QQ:{friendInputStatusChangedEvent.friend.id}({friendInputStatusChangedEvent.friend.nickname}) 变更为:{friendInputStatusChangedEvent.inputting}", "处理中...");
                    break;

                case MiraiEvents.FriendNickChangedEvent:
                    var friendNickChangedEvent = msg.ToObject<FriendNickChangedEvent>();
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "好友昵称改变", $"QQ:{friendNickChangedEvent.friend.id}({friendNickChangedEvent.from}) 变更为:{friendNickChangedEvent.to}", "处理中...");
                    PluginManagerProxy.Instance.Event_OnFriendNickChanged(friendNickChangedEvent.friend.id, friendNickChangedEvent.to);
                    break;

                case MiraiEvents.BotGroupPermissionChangeEvent:
                    var botGroupPermissionChange = msg.ToObject<BotGroupPermissionChangeEvent>();
                    int botGroupPermissionChangeStatus = 1;
                    if (botGroupPermissionChange.origin == "MEMBER")
                    {
                        botGroupPermissionChangeStatus = 2;
                    }

                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "Bot权限变更", $"群:{botGroupPermissionChange.group.id}({botGroupPermissionChange.group.name}) 新权限为:{botGroupPermissionChange.current}", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.Event_OnAdminChange(botGroupPermissionChangeStatus, Helper.TimeStamp, botGroupPermissionChange.group.id, QQ);
                    break;

                case MiraiEvents.BotMuteEvent:
                    var botMute = msg.ToObject<BotMuteEvent>();
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "Bot被禁言", $"群:{botMute._operator.group.id}({botMute._operator.group.name}) 禁言时长:{botMute.durationSeconds} 秒 操作人:{botMute._operator.id}({botMute._operator.memberName})", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.Event_OnGroupBan(2, Helper.TimeStamp, botMute._operator.group.id, botMute._operator.id, QQ, botMute.durationSeconds);
                    break;

                case MiraiEvents.BotUnmuteEvent:
                    var botUnmute = msg.ToObject<BotUnmuteEvent>();
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "Bot解除禁言", $"群:{botUnmute._operator.group.id}({botUnmute._operator.group.name}) 操作人:{botUnmute._operator.id}({botUnmute._operator.memberName})", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.Event_OnGroupBan(1, Helper.TimeStamp, botUnmute._operator.group.id, botUnmute._operator.id, QQ, 0);
                    break;

                case MiraiEvents.BotJoinGroupEvent:
                    var botJoinGroup = msg.ToObject<BotJoinGroupEvent>();
                    string botJoinGroupInvitrosMsg = "";
                    if (botJoinGroup.invitor != null)
                    {
                        botJoinGroupInvitrosMsg = $" 邀请人:{botJoinGroup.invitor.id}({botJoinGroup.invitor.memberName})";
                    }
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "Bot加入群聊", $"群:{botJoinGroup.group.id}({botJoinGroup.group.name}){botJoinGroupInvitrosMsg}", "处理中...");
                    break;

                case MiraiEvents.BotLeaveEventActive:
                    var botLeaveEventActive = msg.ToObject<BotLeaveEventActive>();
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "Bot主动退出群聊", $"群:{botLeaveEventActive.group.id}({botLeaveEventActive.group.name})", "处理中...");
                    break;

                case MiraiEvents.BotLeaveEventKick:
                    var botLeaveEventKick = msg.ToObject<BotLeaveEventKick>();
                    string botLeaveEventOperatorMsg = "";
                    if (botLeaveEventKick._operator != null)
                    {
                        botLeaveEventOperatorMsg = $" 操作人:{botLeaveEventKick._operator.id}({botLeaveEventKick._operator.memberName})";
                    }
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "Bot被踢出群聊", $"群:{botLeaveEventKick.group.id}({botLeaveEventKick.group.name}){botLeaveEventOperatorMsg}", "处理中...");
                    break;

                case MiraiEvents.BotLeaveEventDisband:
                    var botLeaveEventDisband = msg.ToObject<BotLeaveEventDisband>();
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群解散", $"群:{botLeaveEventDisband.group.id}({botLeaveEventDisband.group.name})", "处理中...");
                    break;

                case MiraiEvents.GroupRecallEvent:
                    var groupRecall = msg.ToObject<GroupRecallEvent>();
                    string groupRecallMsg = GetMessageByMsgId(groupRecall.messageId, groupRecall.group.id);
                    if (string.IsNullOrEmpty(groupRecallMsg))
                    {
                        groupRecallMsg = "消息拉取失败";
                    }

                    PluginManagerProxy.Instance.Event_OnGroupMsgRecall(groupRecall.messageId, groupRecall.group.id, groupRecallMsg);
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群撤回", $"群:{groupRecall.group.id}({groupRecall.group.name}) QQ:{groupRecall.authorId} 内容:{groupRecallMsg}", "处理中...");
                    break;

                case MiraiEvents.FriendRecallEvent:
                    var friendRecall = msg.ToObject<FriendRecallEvent>();
                    string friendRecallMsg = GetMessageByMsgId(friendRecall.messageId, friendRecall.authorId);
                    if (string.IsNullOrEmpty(friendRecallMsg))
                    {
                        friendRecallMsg = "消息拉取失败";
                    }

                    PluginManagerProxy.Instance.Event_OnGroupMsgRecall(friendRecall.messageId, friendRecall.authorId, friendRecallMsg);
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "私聊撤回", $"QQ:{friendRecall.authorId} 内容:{friendRecallMsg}", "处理中...");
                    break;

                case MiraiEvents.NudgeEvent:
                    var nudge = msg.ToObject<NudgeEvent>();
                    string nudgeMsg = "";
                    nudgeMsg = nudge.subject.kind == "Group" ? $"群:{nudge.subject.id} QQ:{nudge.fromId}" : $"QQ:{nudge.fromId}";
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "戳一戳", $"{nudgeMsg} 内容:{nudge.action} {nudge.target} {nudge.suffix}", "处理中...");
                    break;

                case MiraiEvents.GroupNameChangeEvent:
                    var groupNameChange = msg.ToObject<GroupNameChangeEvent>();
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群名变更", $"群:{groupNameChange.group.id}({groupNameChange.origin}) 变更为:{groupNameChange.current} 操作人:{groupNameChange._operator.id}({groupNameChange._operator.memberName})", "处理中...");
                    PluginManagerProxy.Instance.Event_OnGroupNameChanged(groupNameChange.group.id, groupNameChange.current);
                    break;

                case MiraiEvents.GroupEntranceAnnouncementChangeEvent:
                    var groupEntranceAnnouncementChange = msg.ToObject<GroupEntranceAnnouncementChangeEvent>();
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群入群公告改变", $"群:{groupEntranceAnnouncementChange.group.id}({groupEntranceAnnouncementChange.group.name}) 内容:{groupEntranceAnnouncementChange.current}", "处理中...");
                    break;

                case MiraiEvents.GroupMuteAllEvent:
                    var groupMuteAllRecall = msg.ToObject<GroupMuteAllEvent>();
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", $"全体{(groupMuteAllRecall.current ? "" : "解除")}禁言", $"群:{groupMuteAllRecall._operator.group.id}({groupMuteAllRecall._operator.group.name}) 操作人:{groupMuteAllRecall._operator.id}({groupMuteAllRecall._operator.memberName})", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.Event_OnGroupBan(groupMuteAllRecall.current ? 1 : 2, Helper.TimeStamp, groupMuteAllRecall._operator.group.id, groupMuteAllRecall._operator.id, 0, 0);
                    break;

                case MiraiEvents.GroupAllowAnonymousChatEvent:
                    var groupAllowAnonymousChat = msg.ToObject<GroupAllowAnonymousChatEvent>();
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群允许匿名聊天", $"群:{groupAllowAnonymousChat._operator.group.id}({groupAllowAnonymousChat._operator.group.name}) 操作人:{groupAllowAnonymousChat._operator.id}({groupAllowAnonymousChat._operator.memberName})", "处理中...");
                    break;

                case MiraiEvents.GroupAllowConfessTalkEvent:
                    var groupAllowConfessTalk = msg.ToObject<GroupAllowConfessTalkEvent>();
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群允许坦白说", $"群:{groupAllowConfessTalk.group.id}({groupAllowConfessTalk.group.name})", "处理中...");
                    break;

                case MiraiEvents.GroupAllowMemberInviteEvent:
                    var groupAllowMemberInvite = msg.ToObject<GroupAllowMemberInviteEvent>();
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群允许邀请入群", $"群:{groupAllowMemberInvite._operator.group.id}({groupAllowMemberInvite._operator.group.name}) 操作人:{groupAllowMemberInvite._operator.id}({groupAllowMemberInvite._operator.memberName})", "处理中...");
                    break;

                case MiraiEvents.MemberJoinEvent:
                    var memberJoin = msg.ToObject<MemberJoinEvent>();
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群成员增加", $"群:{memberJoin.member.group.id}({memberJoin.member.group.name}) QQ:{memberJoin.member.id}({memberJoin.member.memberName})", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.Event_OnGroupMemberIncrease(memberJoin.invitor == null ? 1 : 2, Helper.TimeStamp, memberJoin.member.group.id, 10001, memberJoin.member.id);
                    break;

                case MiraiEvents.MemberLeaveEventKick:
                    var memberLeaveEventKick = msg.ToObject<MemberLeaveEventKick>();
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群成员被踢出", $"群:{memberLeaveEventKick.member.group.id}({memberLeaveEventKick.member.group.name}) QQ:{memberLeaveEventKick.member.id}({memberLeaveEventKick.member.memberName}) 操作者:{memberLeaveEventKick._operator.group.id}({memberLeaveEventKick._operator.group.name})", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.Event_OnGroupMemberDecrease(2, Helper.TimeStamp, memberLeaveEventKick.member.group.id, memberLeaveEventKick.member.group.id, memberLeaveEventKick.member.id);
                    break;

                case MiraiEvents.MemberLeaveEventQuit:
                    var memberLeaveEventQuit = msg.ToObject<MemberLeaveEventQuit>();
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群成员离开", $"群:{memberLeaveEventQuit.member.group.id}({memberLeaveEventQuit.member.group.name}) QQ:{memberLeaveEventQuit.member.id}({memberLeaveEventQuit.member.memberName})", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.Event_OnGroupMemberDecrease(1, Helper.TimeStamp, memberLeaveEventQuit.member.group.id, 0, memberLeaveEventQuit.member.id);
                    break;

                case MiraiEvents.MemberCardChangeEvent:
                    var memberCardChangeEvent = msg.ToObject<MemberCardChangeEvent>();
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群成员名片改变", $"群:{memberCardChangeEvent.member.group.id}({memberCardChangeEvent.member.group.name}) QQ:{memberCardChangeEvent.member.id}({memberCardChangeEvent.member.memberName}) 名片:{memberCardChangeEvent.current}", "处理中...");
                    PluginManagerProxy.Instance.Event_OnGroupMemberCardChanged(memberCardChangeEvent.member.group.id, memberCardChangeEvent.member.id, memberCardChangeEvent.current);
                    break;

                case MiraiEvents.MemberSpecialTitleChangeEvent:
                    var memberSpecialTitleChangeEvent = msg.ToObject<MemberSpecialTitleChangeEvent>();
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群成员头衔改变", $"群:{memberSpecialTitleChangeEvent.member.group.id}({memberSpecialTitleChangeEvent.member.group.name}) QQ:{memberSpecialTitleChangeEvent.member.id}({memberSpecialTitleChangeEvent.member.memberName}) 称号:{memberSpecialTitleChangeEvent.current}", "处理中...");
                    break;

                case MiraiEvents.MemberPermissionChangeEvent:
                    var memberPermissionChangeEvent = msg.ToObject<MemberPermissionChangeEvent>();
                    int memberPermissionChangeStatus = 1;
                    if (memberPermissionChangeEvent.origin == "MEMBER")
                    {
                        memberPermissionChangeStatus = 2;
                    }

                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群员权限变更", $"群:{memberPermissionChangeEvent.member.group.id}({memberPermissionChangeEvent.member.group.name}) QQ:{memberPermissionChangeEvent.member.id}({memberPermissionChangeEvent.member.memberName}) 新权限为:{memberPermissionChangeEvent.current}", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.Event_OnAdminChange(memberPermissionChangeStatus, Helper.TimeStamp, memberPermissionChangeEvent.member.group.id, memberPermissionChangeEvent.member.id);
                    break;

                case MiraiEvents.MemberMuteEvent:
                    var memberMuteEvent = msg.ToObject<MemberMuteEvent>();
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群员被禁言", $"群:{memberMuteEvent.member.group.id}({memberMuteEvent.member.group.name}) QQ:{memberMuteEvent.member.id}({memberMuteEvent.member.memberName}) 禁言时长:{memberMuteEvent.durationSeconds} 秒 操作人:{memberMuteEvent._operator?.id}({memberMuteEvent._operator?.memberName})", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.Event_OnGroupBan(2, Helper.TimeStamp, memberMuteEvent.member.group.id, memberMuteEvent._operator?.id ?? 10001, memberMuteEvent.member.id, memberMuteEvent.durationSeconds);
                    break;

                case MiraiEvents.MemberUnmuteEvent:
                    var memberUnmuteEvent = msg.ToObject<MemberUnmuteEvent>();
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群员被解除禁言", $"群:{memberUnmuteEvent.member.group.id}({memberUnmuteEvent.member.group.name}) QQ:{memberUnmuteEvent.member.id}({memberUnmuteEvent.member.memberName}) 操作人:{memberUnmuteEvent._operator?.id}({memberUnmuteEvent._operator?.memberName})", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.Event_OnGroupBan(1, Helper.TimeStamp, memberUnmuteEvent.member.group.id, memberUnmuteEvent._operator?.id ?? 10001, memberUnmuteEvent.member.id, 0);
                    break;

                case MiraiEvents.MemberHonorChangeEvent:
                    var memberHonorChangeEvent = msg.ToObject<MemberHonorChangeEvent>();
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "群员称号改变", $"群:{memberHonorChangeEvent.member.group.id}({memberHonorChangeEvent.member.group.name}) QQ:{memberHonorChangeEvent.member.id}({memberHonorChangeEvent.member.memberName}) 变更为:{memberHonorChangeEvent.honor}", "处理中...");
                    break;

                case MiraiEvents.NewFriendRequestEvent:
                    var newFriendRequestEvent = msg.ToObject<NewFriendRequestEvent>();
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "添加好友请求", $"QQ:{newFriendRequestEvent.fromId}({newFriendRequestEvent.nick}) 备注:{newFriendRequestEvent.message} 来源群:{newFriendRequestEvent.groupId}", "处理中...");
                    RequestCache.FriendRequest.Add(newFriendRequestEvent.eventId.ToString(), (newFriendRequestEvent.fromId, newFriendRequestEvent.nick));
                    handledPlugin = PluginManagerProxy.Instance.Event_OnFriendAddRequest(1, Helper.TimeStamp, newFriendRequestEvent.fromId, newFriendRequestEvent.message, newFriendRequestEvent.eventId.ToString());
                    break;

                case MiraiEvents.MemberJoinRequestEvent:
                    var memberJoinRequestEvent = msg.ToObject<MemberJoinRequestEvent>();
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "添加群请求", $"群:{memberJoinRequestEvent.groupId}({memberJoinRequestEvent.groupName}) QQ:{memberJoinRequestEvent.fromId}({memberJoinRequestEvent.nick}) 备注:{memberJoinRequestEvent.message}", "处理中...");
                    RequestCache.GroupRequest.Add(memberJoinRequestEvent.eventId.ToString(), (memberJoinRequestEvent.fromId, memberJoinRequestEvent.nick, memberJoinRequestEvent.groupId, memberJoinRequestEvent.groupName));
                    handledPlugin = PluginManagerProxy.Instance.Event_OnGroupAddRequest(1, Helper.TimeStamp, memberJoinRequestEvent.groupId, memberJoinRequestEvent.fromId, memberJoinRequestEvent.message, memberJoinRequestEvent.eventId.ToString());
                    break;

                case MiraiEvents.BotInvitedJoinGroupRequestEvent:
                    var botInvitedJoinGroupRequestEvent = msg.ToObject<BotInvitedJoinGroupRequestEvent>();
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "收到入群邀请", $"群:{botInvitedJoinGroupRequestEvent.groupId}({botInvitedJoinGroupRequestEvent.groupName}) QQ:{botInvitedJoinGroupRequestEvent.fromId}({botInvitedJoinGroupRequestEvent.nick}) 备注:{botInvitedJoinGroupRequestEvent.message}", "处理中...");
                    break;

                case MiraiEvents.OtherClientOnlineEvent:
                    var otherClientOnlineEvent = msg.ToObject<OtherClientOnlineEvent>();
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "其他设备上线", $"{otherClientOnlineEvent.client.platform}", "处理中...");
                    break;

                case MiraiEvents.OtherClientOfflineEvent:
                    var otherClientOfflineEvent = msg.ToObject<OtherClientOfflineEvent>();
                    logId = LogHelper.WriteLog(LogLevel.Info, "AMN框架", "其他设备离线", $"{otherClientOfflineEvent.client.platform}", "处理中...");
                    break;

                case MiraiEvents.CommandExecutedEvent:
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

        private void ParseAndDispatchMessage(JObject msg)
        {
            if (PluginManagerProxy.Instance == null)
            {
                return;
            }
            Stopwatch sw = new();
            sw.Start();
            MiraiMessageEvents events = Helper.String2Enum<MiraiMessageEvents>(msg["type"].ToString());

            MiraiMessageTypeDetail.Source source = null;
            var chainMsg = CQCodeBuilder.ParseJArray2MiraiMessageBaseList(msg["messageChain"] as JArray);
            source = (MiraiMessageTypeDetail.Source)chainMsg.First(x => x.messageType == MiraiMessageType.Source);
            string parsedMsg = CQCodeBuilder.Parse(chainMsg);
            if (source == null)
            {
                LogHelper.WriteLog(LogLevel.Info, "AMN框架", "参数缺失", $"", "");
                return;
            }

            // 文件上传优先处理
            if (chainMsg.Any(x => x.messageType == MiraiMessageType.File))
            {
                var group = msg.ToObject<GroupMessage>();
                var file = (MiraiMessageTypeDetail.File)chainMsg.First(x => x.messageType == MiraiMessageType.File);
                MemoryStream stream = new();
                BinaryWriter binaryWriter = new(stream);
                BinaryWriterExpand.Write_Ex(binaryWriter, file.id);
                BinaryWriterExpand.Write_Ex(binaryWriter, file.name);
                BinaryWriterExpand.Write_Ex(binaryWriter, file.size);
                BinaryWriterExpand.Write_Ex(binaryWriter, 0);
                PluginManagerProxy.Instance.Event_OnUpload(1, Helper.TimeStamp, group.sender.group.id, group.sender.id, Convert.ToBase64String(stream.ToArray()));
                sw.Stop();
                LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "文件上传", $"来源群:{group.sender.group.id}({group.sender.group.name}) 来源QQ:{group.sender.id}({group.sender.memberName}) " +
                    $"文件名:{file.name} 大小:{file.size / 1000}KB FileID:{file.id}", $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s");
                return;
            }

            int logId = 0;
            RequestCache.AddMessageCache(source.id, parsedMsg);
            CQPluginProxy handledPlugin = null;
            switch (events)
            {
                case MiraiMessageEvents.FriendMessage:
                    var friend = msg.ToObject<FriendMessage>();
                    logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "[↓]收到好友消息", $"QQ:{friend.sender.id}({friend.sender.nickname}) {parsedMsg}", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.Event_OnPrivateMsg(11, source.id, friend.sender.id, parsedMsg, 0, DateTime.Now);
                    break;

                case MiraiMessageEvents.GroupMessage:
                    var group = msg.ToObject<GroupMessage>();
                    logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "[↓]收到消息", $"群:{group.sender.group.id}({group.sender.group.name}) QQ:{group.sender.id}({group.sender.memberName}) 消息: {parsedMsg}", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.Event_OnGroupMsg(1, source.id, group.sender.group.id, group.sender.id, "", parsedMsg, 0, DateTime.Now);
                    break;

                case MiraiMessageEvents.TempMessage:
                    var temp = msg.ToObject<TempMessage>();
                    logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "[↓]收到群临时消息", $"群:{temp.sender.group.id}({temp.sender.group.name}) QQ:{temp.sender.id}({temp.sender.memberName}) 消息: {parsedMsg}", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.Event_OnPrivateMsg(2, source.id, temp.sender.id, parsedMsg, 0, DateTime.Now);
                    break;

                case MiraiMessageEvents.StrangerMessage:
                    var stranger = msg.ToObject<StrangerMessage>();
                    logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "[↓]收到陌生人消息", $"QQ:{stranger.sender.id}({stranger.sender.nickname}) 消息: {parsedMsg}", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.Event_OnPrivateMsg(1, source.id, stranger.sender.id, parsedMsg, 0, DateTime.Now);
                    break;

                case MiraiMessageEvents.OtherClientMessage:
                    var other = msg.ToObject<OtherClientMessage>();
                    logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "[↓]收到其他设备消息", $"QQ:{other.sender.id}({other.sender.platform}) 消息: {parsedMsg}", "x 不处理");
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
    }
}