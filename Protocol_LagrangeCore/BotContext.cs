using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Lagrange.Core;
using Lagrange.Core.Common;
using Lagrange.Core.Common.Interface;
using Lagrange.Core.Common.Interface.Api;
using Lagrange.Core.Message;
using Lagrange.Core.Message.Entity;
using System.Diagnostics;

namespace Another_Mirai_Native.Protocol.LagrangeCore
{
    public partial class LagrangeCoreAPI
    {
        private BotContext BotContext { get; set; }

        private BotSign? BotSign { get; set; }

        private CancellationTokenSource LoginToken { get; set; }

        private bool InitBotContext()
        {
            try
            {
                BotSign ??= new BotSign(LagrangeConfig.SignUrl, LagrangeConfig.SignFallbackPlatform);
                LagrangeConfig.BotConfig.CustomSignProvider = BotSign;
                BotContext = BotFactory.Create(LagrangeConfig.BotConfig,
                    LagrangeConfig.BotDeviceInfo,
                    LagrangeConfig.BotKeystore,
                    BotSign.BotAppInfo);

                BotContext.Invoker.OnBotLogEvent += Invoker_OnBotLogEvent;
                BotContext.Invoker.OnBotCaptchaEvent += Invoker_OnBotCaptchaEvent;
                BotContext.Invoker.OnBotOnlineEvent += Invoker_OnBotOnlineEvent;
                BotContext.Invoker.OnBotOfflineEvent += Invoker_OnBotOfflineEvent;
                BotContext.Invoker.OnPinChangedEvent += Invoker_OnPinChangedEvent;
                BotContext.Invoker.OnBotNewDeviceVerify += Invoker_OnBotNewDeviceVerify;
                BotContext.Invoker.OnDeviceLoginEvent += Invoker_OnDeviceLoginEvent;
                BotContext.Invoker.OnFriendMessageReceived += Invoker_OnFriendMessageReceived;
                BotContext.Invoker.OnFriendPokeEvent += Invoker_OnFriendPokeEvent;
                BotContext.Invoker.OnFriendRecallEvent += Invoker_OnFriendRecallEvent;
                BotContext.Invoker.OnFriendRequestEvent += Invoker_OnFriendRequestEvent;
                BotContext.Invoker.OnGroupAdminChangedEvent += Invoker_OnGroupAdminChangedEvent;
                BotContext.Invoker.OnGroupEssenceEvent += Invoker_OnGroupEssenceEvent;
                BotContext.Invoker.OnGroupInvitationReceived += Invoker_OnGroupInvitationReceived;
                BotContext.Invoker.OnGroupInvitationRequestEvent += Invoker_OnGroupInvitationRequestEvent;
                BotContext.Invoker.OnGroupJoinRequestEvent += Invoker_OnGroupJoinRequestEvent;
                BotContext.Invoker.OnGroupMemberDecreaseEvent += Invoker_OnGroupMemberDecreaseEvent;
                BotContext.Invoker.OnGroupMemberEnterEvent += Invoker_OnGroupMemberEnterEvent;
                BotContext.Invoker.OnGroupMemberIncreaseEvent += Invoker_OnGroupMemberIncreaseEvent;
                BotContext.Invoker.OnGroupMemberMuteEvent += Invoker_OnGroupMemberMuteEvent;
                BotContext.Invoker.OnGroupMessageReceived += Invoker_OnGroupMessageReceived;
                BotContext.Invoker.OnGroupMuteEvent += Invoker_OnGroupMuteEvent;
                BotContext.Invoker.OnGroupNameChangeEvent += Invoker_OnGroupNameChangeEvent;
                BotContext.Invoker.OnGroupPokeEvent += Invoker_OnGroupPokeEvent;
                BotContext.Invoker.OnGroupReactionEvent += Invoker_OnGroupReactionEvent;
                BotContext.Invoker.OnGroupRecallEvent += Invoker_OnGroupRecallEvent;
                BotContext.Invoker.OnGroupTodoEvent += Invoker_OnGroupTodoEvent;
                BotContext.Invoker.OnTempMessageReceived += Invoker_OnTempMessageReceived;

                MessageChainPaser.OnFileUploaded += MessageChainPaser_OnFileUploaded;
                LogHelper.Info("创建Bot实例", $"成功");

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error("创建Bot实例", ex);
                return false;
            }
        }

        private void MessageChainPaser_OnFileUploaded(MessageChain chain, FileEntity file)
        {
            if (string.IsNullOrEmpty(file.FileId))
            {
                return;
            }
            Stopwatch sw = Stopwatch.StartNew();
            MemoryStream stream = new();
            BinaryWriter binaryWriter = new(stream);
            BinaryWriterExpand.Write_Ex(binaryWriter, file.FileId);
            BinaryWriterExpand.Write_Ex(binaryWriter, file.FileName);
            BinaryWriterExpand.Write_Ex(binaryWriter, file.FileSize);
            BinaryWriterExpand.Write_Ex(binaryWriter, 0);
            PluginManagerProxy.Instance.Event_OnUpload(1, Helper.TimeStamp, chain.GroupUin.ToLong(), chain.FriendUin, Convert.ToBase64String(stream.ToArray()));
            sw.Stop();
            LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "文件上传",
                $"来源群:{chain.GroupUin}({ChatHistoryHelper.GetGroupName(chain.GroupUin.ToLong()).Result}) " +
                $"来源QQ:{chain.FriendUin}({ChatHistoryHelper.GetGroupMemberNick(chain.GroupUin.ToLong(), chain.FriendUin).Result}) " +
                $"文件名:{file.FileName} " +
                $"大小:{file.FileSize / 1000}KB " +
                $"FileID:{file.FileId}", $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s");
        }

        private void Invoker_OnTempMessageReceived(BotContext context, Lagrange.Core.Event.EventArg.TempMessageEvent e)
        {
            Stopwatch sw = Stopwatch.StartNew();

            var message = MessageChainPaser.ParseMessageChainToCQCode(e.Chain);
            int messageId = MessageCacher.RecordMessage(e.Chain);

            int logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "[↓]收到群临时消息",
                $"群:{e.Chain.GroupUin}({ChatHistoryHelper.GetGroupName(e.Chain.GroupUin.ToLong()).Result}) " +
                $"QQ:{e.Chain.FriendUin}({ChatHistoryHelper.GetGroupMemberNick(e.Chain.GroupUin.ToLong(), e.Chain.FriendUin).Result}) " +
                $"消息:{message}", "处理中...");
            var handledPlugin = PluginManagerProxy.Instance.Event_OnPrivateMsg(2, messageId, e.Chain.FriendUin, message, 0, DateTime.Now);
            string updateMsg = $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s";
            if (handledPlugin != null)
            {
                updateMsg += $"(由 {handledPlugin.AppInfo.name} 结束消息处理)";
            }
            LogHelper.UpdateLogStatus(logId, updateMsg);
        }

        private void Invoker_OnGroupTodoEvent(BotContext context, Lagrange.Core.Event.EventArg.GroupTodoEvent e)
        {
            LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "群待办",
                $"群:{e.GroupUin}({ChatHistoryHelper.GetGroupName(e.GroupUin).Result}) " +
                $"QQ:{e.OperatorUin}({ChatHistoryHelper.GetGroupMemberNick(e.GroupUin, e.OperatorUin).Result})", "");
        }

        private void Invoker_OnGroupRecallEvent(BotContext context, Lagrange.Core.Event.EventArg.GroupRecallEvent e)
        {
            Stopwatch sw = Stopwatch.StartNew();
            int logId = 0;
            var msgId = MessageCacher.GetMessageIdBySeq(e.Sequence);
            var message = MessageCacher.GetMessageById(msgId);
            if (message == null)
            {
                logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "群撤回",
                    $"群:{e.GroupUin}({ChatHistoryHelper.GetGroupName(e.GroupUin).Result}) " +
                    $"QQ:{e.OperatorUin}({ChatHistoryHelper.GetGroupMemberNick(e.GroupUin, e.OperatorUin).Result}) " +
                    $"消息:消息拉取失败", "处理中...");
            }
            else
            {
                string parsedMessage = MessageChainPaser.ParseMessageChainToCQCode(message);
                PluginManagerProxy.Instance.Event_OnGroupMsgRecall(msgId, e.GroupUin, parsedMessage);
                logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "群撤回",
                    $"群:{e.GroupUin}({ChatHistoryHelper.GetGroupName(e.GroupUin).Result}) " +
                    $"QQ:{e.OperatorUin}({ChatHistoryHelper.GetGroupMemberNick(e.GroupUin, e.OperatorUin).Result}) " +
                    $"消息:{parsedMessage}", "处理中...");
            }
            string updateMsg = $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s";
            LogHelper.UpdateLogStatus(logId, updateMsg);
        }

        private void Invoker_OnGroupReactionEvent(BotContext context, Lagrange.Core.Event.EventArg.GroupReactionEvent e)
        {
            LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "群回应",
                $"群:{e.TargetGroupUin} " +
                $"QQ:{e.OperatorUin}({ChatHistoryHelper.GetGroupMemberNick(e.TargetGroupUin, e.OperatorUin).Result}) " +
                $"表情ID:{e.Code} " +
                $"数量:{e.Count}", "");
        }

        private void Invoker_OnGroupPokeEvent(BotContext context, Lagrange.Core.Event.EventArg.GroupPokeEvent e)
        {
            LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "戳一戳",
                $"群:{e.GroupUin}({ChatHistoryHelper.GetGroupName(e.GroupUin).Result}) " +
                $"QQ:{e.OperatorUin}({ChatHistoryHelper.GetGroupMemberNick(e.GroupUin, e.OperatorUin).Result}) " +
                $"动作:{e.Action}", "");
        }

        private void Invoker_OnGroupNameChangeEvent(BotContext context, Lagrange.Core.Event.EventArg.GroupNameChangeEvent e)
        {
            Stopwatch sw = Stopwatch.StartNew();

            int logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "群名变更",
                $"群:{e.GroupUin}({ChatHistoryHelper.GetGroupName(e.GroupUin).Result}) " +
                $"新名称:{e.Name}", "处理中...");
            PluginManagerProxy.Instance.Event_OnGroupNameChanged(e.GroupUin, e.Name);
            string updateMsg = $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s";
            LogHelper.UpdateLogStatus(logId, updateMsg);
        }

        private void Invoker_OnGroupMuteEvent(BotContext context, Lagrange.Core.Event.EventArg.GroupMuteEvent e)
        {
            Stopwatch sw = Stopwatch.StartNew();

            int logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", $"全体{(e.IsMuted ? "" : "解除")}禁言",
                $"群:{e.GroupUin}({ChatHistoryHelper.GetGroupName(e.GroupUin).Result}) " +
                $"操作者:{e.OperatorUin}({ChatHistoryHelper.GetGroupMemberNick(e.GroupUin, e.OperatorUin.ToLong()).Result})", "处理中...");
            int subType = e.IsMuted ? 2 : 1;
            var handledPlugin = PluginManagerProxy.Instance.Event_OnGroupBan(subType, Helper.DateTime2TimeStamp(e.EventTime), e.GroupUin, e.OperatorUin.ToLong(), 0, 0);
            string updateMsg = $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s";
            if (handledPlugin != null)
            {
                updateMsg += $"(由 {handledPlugin.AppInfo.name} 结束消息处理)";
            }
            LogHelper.UpdateLogStatus(logId, updateMsg);
        }

        private void Invoker_OnGroupMessageReceived(BotContext context, Lagrange.Core.Event.EventArg.GroupMessageEvent e)
        {
            MessageCacher.UpdateChainByRawMessageId(e.Chain);
            if (e.Chain.FriendUin == AppConfig.Instance.CurrentQQ)
            {
                return;
            }
            Stopwatch sw = Stopwatch.StartNew();
            var message = MessageChainPaser.ParseMessageChainToCQCode(e.Chain);
            int messageId = MessageCacher.RecordMessage(e.Chain);

            int logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "[↓]收到消息",
                $"群:{e.Chain.GroupUin}({ChatHistoryHelper.GetGroupName(e.Chain.GroupUin.ToLong()).Result}) " +
                $"QQ:{e.Chain.FriendUin}({ChatHistoryHelper.GetGroupMemberNick(e.Chain.GroupUin.ToLong(), e.Chain.FriendUin).Result}) " +
                $"消息: {message}", "处理中...");
            CQPluginProxy? handledPlugin = PluginManagerProxy.Instance.Event_OnGroupMsg(1, messageId, e.Chain.GroupUin.ToLong(), e.Chain.FriendUin, "", message, 0, DateTime.Now);

            sw.Stop();
            string updateMsg = $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s";
            if (handledPlugin != null)
            {
                updateMsg += $"(由 {handledPlugin.AppInfo.name} 结束消息处理)";
            }
            LogHelper.UpdateLogStatus(logId, updateMsg);
        }

        private void Invoker_OnGroupMemberMuteEvent(BotContext context, Lagrange.Core.Event.EventArg.GroupMemberMuteEvent e)
        {
            Stopwatch sw = Stopwatch.StartNew();

            int logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", $"群员被{(e.Duration == 0 ? "解除" : "")}禁言",
                $"群:{e.GroupUin}({ChatHistoryHelper.GetGroupName(e.GroupUin).Result}) " +
                $"被禁言:{e.TargetUin}({ChatHistoryHelper.GetGroupMemberNick(e.GroupUin, e.TargetUin).Result}) " +
                $"操作者:{e.OperatorUin}({ChatHistoryHelper.GetGroupMemberNick(e.GroupUin, e.OperatorUin.ToLong()).Result}) " +
                $"时长:{e.Duration}", "处理中...");
            int subType = e.Duration != 0 ? 2 : 1;
            var handledPlugin = PluginManagerProxy.Instance.Event_OnGroupBan(subType, Helper.DateTime2TimeStamp(e.EventTime), e.GroupUin, e.OperatorUin.ToLong(), e.TargetUin, e.Duration);
            string updateMsg = $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s";
            if (handledPlugin != null)
            {
                updateMsg += $"(由 {handledPlugin.AppInfo.name} 结束消息处理)";
            }
            LogHelper.UpdateLogStatus(logId, updateMsg);
        }

        private void Invoker_OnGroupMemberIncreaseEvent(BotContext context, Lagrange.Core.Event.EventArg.GroupMemberIncreaseEvent e)
        {
            Stopwatch sw = Stopwatch.StartNew();

            int logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "群成员增加",
                $"群:{e.GroupUin}({ChatHistoryHelper.GetGroupName(e.GroupUin).Result}) " +
                $"新群员:{e.MemberUin}({ChatHistoryHelper.GetGroupMemberNick(e.GroupUin, e.MemberUin).Result}) " +
                $"邀请者:{e.InvitorUin}({ChatHistoryHelper.GetGroupMemberNick(e.GroupUin, e.InvitorUin.ToLong()).Result})", "处理中...");
            var handledPlugin = PluginManagerProxy.Instance.Event_OnGroupMemberIncrease(e.Type == Lagrange.Core.Event.EventArg.GroupMemberIncreaseEvent.EventType.Approve ? 1 : 2, Helper.DateTime2TimeStamp(e.EventTime), e.GroupUin, e.MemberUin, e.InvitorUin.ToLong());
            string updateMsg = $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s";
            if (handledPlugin != null)
            {
                updateMsg += $"(由 {handledPlugin.AppInfo.name} 结束消息处理)";
            }
            LogHelper.UpdateLogStatus(logId, updateMsg);
        }

        private void Invoker_OnGroupMemberEnterEvent(BotContext context, Lagrange.Core.Event.EventArg.GroupMemberEnterEvent e)
        {
            // ignore
        }

        private void Invoker_OnGroupMemberDecreaseEvent(BotContext context, Lagrange.Core.Event.EventArg.GroupMemberDecreaseEvent e)
        {
            Stopwatch sw = Stopwatch.StartNew();

            int logId = 0;
            CQPluginProxy? handledPlugin = null;
            switch (e.Type)
            {
                case Lagrange.Core.Event.EventArg.GroupMemberDecreaseEvent.EventType.KickMe:
                    logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "Bot被踢出群聊",
                        $"群:{e.GroupUin}({ChatHistoryHelper.GetGroupName(e.GroupUin).Result}) " +
                        $"操作人:{e.OperatorUin}({ChatHistoryHelper.GetGroupMemberNick(e.GroupUin, e.OperatorUin.ToLong()).Result})", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.Event_OnGroupMemberDecrease(2, Helper.DateTime2TimeStamp(e.EventTime), e.GroupUin, e.MemberUin, e.OperatorUin.ToLong());
                    break;

                case Lagrange.Core.Event.EventArg.GroupMemberDecreaseEvent.EventType.Disband:
                    logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "群解散",
                        $"群:{e.GroupUin}", "处理中...");
                    break;

                case Lagrange.Core.Event.EventArg.GroupMemberDecreaseEvent.EventType.Leave:
                    logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "群成员离开",
                        $"群:{e.GroupUin}({ChatHistoryHelper.GetGroupName(e.GroupUin).Result}) " +
                        $"群员:{e.MemberUin}({ChatHistoryHelper.GetGroupMemberNick(e.GroupUin, e.MemberUin).Result}) " +
                        $"操作者:{e.OperatorUin}({ChatHistoryHelper.GetGroupMemberNick(e.GroupUin, e.OperatorUin.ToLong()).Result})", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.Event_OnGroupMemberDecrease(1, Helper.DateTime2TimeStamp(e.EventTime), e.GroupUin, e.MemberUin, e.OperatorUin.ToLong());
                    break;

                case Lagrange.Core.Event.EventArg.GroupMemberDecreaseEvent.EventType.Kick:
                    logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "群成员被踢出",
                        $"群:{e.GroupUin}({ChatHistoryHelper.GetGroupName(e.GroupUin).Result}) " +
                        $"群员:{e.MemberUin}({ChatHistoryHelper.GetGroupMemberNick(e.GroupUin, e.MemberUin).Result}) " +
                        $"操作者:{e.OperatorUin}({ChatHistoryHelper.GetGroupMemberNick(e.GroupUin, e.OperatorUin.ToLong()).Result})", "处理中...");
                    handledPlugin = PluginManagerProxy.Instance.Event_OnGroupMemberDecrease(2, Helper.DateTime2TimeStamp(e.EventTime), e.GroupUin, e.MemberUin, e.OperatorUin.ToLong());
                    break;

                default:
                    break;
            }
            string updateMsg = $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s";
            if (handledPlugin != null)
            {
                updateMsg += $"(由 {handledPlugin.AppInfo.name} 结束消息处理)";
            }
            LogHelper.UpdateLogStatus(logId, updateMsg);
        }

        private void Invoker_OnGroupJoinRequestEvent(BotContext context, Lagrange.Core.Event.EventArg.GroupJoinRequestEvent e)
        {
            Stopwatch sw = Stopwatch.StartNew();

            var requests = BotContext.FetchGroupRequests().Result;
            var request = requests?.FirstOrDefault(x => x.GroupUin == e.GroupUin && e.TargetUin == x.TargetMemberUin);
            if (request != null)
            {
                string id = $"{e.GroupUin}_{e.TargetUin}";
                int logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "添加群请求",
                    $"群:{e.GroupUin}({ChatHistoryHelper.GetGroupName(e.GroupUin).Result}) " +
                    $"申请者:{e.TargetUin} " +
                    $"备注:{request.Comment}", "处理中...");
                var handledPlugin = PluginManagerProxy.Instance.Event_OnGroupAddRequest(1, Helper.DateTime2TimeStamp(e.EventTime), e.GroupUin, e.TargetUin, request.Comment ?? "", id.ToString());

                string updateMsg = $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s";
                if (handledPlugin != null)
                {
                    updateMsg += $"(由 {handledPlugin.AppInfo.name} 结束消息处理)";
                }
                LogHelper.UpdateLogStatus(logId, updateMsg);
            }
        }

        private void Invoker_OnGroupInvitationRequestEvent(BotContext context, Lagrange.Core.Event.EventArg.GroupInvitationRequestEvent e)
        {
            Stopwatch sw = Stopwatch.StartNew();

            var requests = BotContext.FetchGroupRequests().Result;
            var request = requests?.FirstOrDefault(x => x.GroupUin == e.GroupUin && e.TargetUin == x.TargetMemberUin);
            if (request != null)
            {
                string id = $"{e.GroupUin}_{e.TargetUin}";
                int logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "邀请入群请求",
                    $"群:{e.GroupUin}({ChatHistoryHelper.GetGroupName(e.GroupUin).Result}) " +
                    $"申请者:{e.TargetUin} " +
                    $"邀请者:{request.InvitorMemberUin}({request.InvitorMemberCard}) " +
                    $"备注:{request.Comment}", "处理中...");
                var handledPlugin = PluginManagerProxy.Instance.Event_OnGroupAddRequest(2, Helper.DateTime2TimeStamp(e.EventTime), e.GroupUin, e.TargetUin, request.Comment ?? "", id.ToString());
                string updateMsg = $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s";
                if (handledPlugin != null)
                {
                    updateMsg += $"(由 {handledPlugin.AppInfo.name} 结束消息处理)";
                }
                LogHelper.UpdateLogStatus(logId, updateMsg);
            }
        }

        private void Invoker_OnGroupInvitationReceived(BotContext context, Lagrange.Core.Event.EventArg.GroupInvitationEvent e)
        {
            LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "收到入群邀请", $"群:{e.GroupUin}({ChatHistoryHelper.GetGroupName(e.GroupUin).Result}) " +
                $"QQ:{e.InvitorUin}({ChatHistoryHelper.GetGroupMemberNick(e.GroupUin, e.InvitorUin).Result})", "");
        }

        private void Invoker_OnGroupEssenceEvent(BotContext context, Lagrange.Core.Event.EventArg.GroupEssenceEvent e)
        {
            // ignore
        }

        private void Invoker_OnGroupAdminChangedEvent(BotContext context, Lagrange.Core.Event.EventArg.GroupAdminChangedEvent e)
        {
            Stopwatch sw = Stopwatch.StartNew();

            int logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "群员权限变更",
                $"群:{e.GroupUin}({ChatHistoryHelper.GetGroupName(e.GroupUin).Result}) " +
                $"QQ:{e.AdminUin}({ChatHistoryHelper.GetGroupMemberNick(e.GroupUin, e.AdminUin).Result}) " +
                $"新权限为:{(e.IsPromote ? "管理层" : "群员")}", "处理中...");
            var handledPlugin = PluginManagerProxy.Instance.Event_OnAdminChange(e.IsPromote ? 2 : 1, Helper.TimeStamp, e.GroupUin, e.AdminUin);
            string updateMsg = $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s";
            if (handledPlugin != null)
            {
                updateMsg += $"(由 {handledPlugin.AppInfo.name} 结束消息处理)";
            }
            LogHelper.UpdateLogStatus(logId, updateMsg);
        }

        private void Invoker_OnFriendRequestEvent(BotContext context, Lagrange.Core.Event.EventArg.FriendRequestEvent e)
        {
            Stopwatch sw = Stopwatch.StartNew();

            string id = $"{e.SourceUin}";
            int logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "添加好友请求",
                $"申请者:{e.SourceUin} " +
                $"备注:{e.Message}", "处理中...");
            var handledPlugin = PluginManagerProxy.Instance.Event_OnFriendAddRequest(1, Helper.DateTime2TimeStamp(e.EventTime), e.SourceUin, e.EventMessage, id.ToString());
            string updateMsg = $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s";
            if (handledPlugin != null)
            {
                updateMsg += $"(由 {handledPlugin.AppInfo.name} 结束消息处理)";
            }
            LogHelper.UpdateLogStatus(logId, updateMsg);
        }

        private void Invoker_OnFriendRecallEvent(BotContext context, Lagrange.Core.Event.EventArg.FriendRecallEvent e)
        {
            Stopwatch sw = Stopwatch.StartNew();
            int logId;
            var msgId = MessageCacher.GetMessageIdBySeq(e.ClientSequence);
            var message = MessageCacher.GetMessageById(msgId);
            if (message == null)
            {
                logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "好友撤回",
                    $"QQ:{e.FriendUin}({ChatHistoryHelper.GetFriendNick(e.FriendUin).Result}) " +
                    $"消息:消息拉取失败", "处理中...");
            }
            else
            {
                string parsedMessage = MessageChainPaser.ParseMessageChainToCQCode(message);
                PluginManagerProxy.Instance.Event_OnPrivateMsgRecall(msgId, e.FriendUin, parsedMessage);
                logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "好友撤回",
                    $"QQ:{e.FriendUin}({ChatHistoryHelper.GetFriendNick(e.FriendUin).Result}) " +
                    $"消息:{parsedMessage}", "处理中...");
            }
            string updateMsg = $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s";
            LogHelper.UpdateLogStatus(logId, updateMsg);
        }

        private void Invoker_OnFriendPokeEvent(BotContext context, Lagrange.Core.Event.EventArg.FriendPokeEvent e)
        {
            LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "戳一戳",
                $"QQ:{e.OperatorUin}({ChatHistoryHelper.GetFriendNick(e.OperatorUin).Result}) " +
                $"动作:{e.Action}", "");
        }

        private void Invoker_OnFriendMessageReceived(BotContext context, Lagrange.Core.Event.EventArg.FriendMessageEvent e)
        {
            Stopwatch sw = Stopwatch.StartNew();

            var message = MessageChainPaser.ParseMessageChainToCQCode(e.Chain);
            int messageId = MessageCacher.RecordMessage(e.Chain);

            int logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "[↓]收到好友消息",
                $"QQ:{e.Chain.FriendUin}({ChatHistoryHelper.GetFriendNick(e.Chain.FriendUin).Result}) " +
                $"消息: {message}", "处理中...");
            var handledPlugin = PluginManagerProxy.Instance.Event_OnPrivateMsg(1, messageId, e.Chain.FriendUin, message, 0, DateTime.Now);
            string updateMsg = $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s";
            if (handledPlugin != null)
            {
                updateMsg += $"(由 {handledPlugin.AppInfo.name} 结束消息处理)";
            }
            LogHelper.UpdateLogStatus(logId, updateMsg);
        }

        private void Invoker_OnDeviceLoginEvent(BotContext context, Lagrange.Core.Event.EventArg.DeviceLoginEvent e)
        {
            LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "其他设备上线", e.Message, "");
        }

        private void Invoker_OnBotNewDeviceVerify(BotContext context, Lagrange.Core.Event.EventArg.BotNewDeviceVerifyEvent e)
        {
            LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "新设备验证", e.Url, "");
        }

        private void Invoker_OnPinChangedEvent(BotContext context, Lagrange.Core.Event.EventArg.PinChangedEvent e)
        {
            // ignore
        }

        private void Invoker_OnBotOfflineEvent(BotContext context, Lagrange.Core.Event.EventArg.BotOfflineEvent e)
        {
            LogHelper.Info("Bot离线", e.EventMessage);
            IsConnected = false;
        }

        private void Invoker_OnBotOnlineEvent(BotContext context, Lagrange.Core.Event.EventArg.BotOnlineEvent e)
        {
            LogHelper.Info("Bot在线", e.EventMessage);
            RequestWaiter.TriggerByKey("LagrangeCoreLogin", true);
            IsConnected = true;
            LagrangeConfig.BotKeystore = BotContext.UpdateKeystore();
            LagrangeConfig.Instance.Save();
            LogHelper.Info("账号登录", "已更新登录状态缓存");
        }

        private void Invoker_OnBotCaptchaEvent(BotContext context, Lagrange.Core.Event.EventArg.BotCaptchaEvent e)
        {
            LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "验证码请求", e.Url, "");
        }

        private void Invoker_OnBotLogEvent(BotContext context, Lagrange.Core.Event.EventArg.BotLogEvent e)
        {
            LogLevel level = e.Level switch
            {
                Lagrange.Core.Event.EventArg.LogLevel.Debug => LogLevel.Debug,
                Lagrange.Core.Event.EventArg.LogLevel.Verbose => LogLevel.Debug,
                Lagrange.Core.Event.EventArg.LogLevel.Information => LogLevel.Info,
                Lagrange.Core.Event.EventArg.LogLevel.Warning => LogLevel.Warning,
                Lagrange.Core.Event.EventArg.LogLevel.Exception => LogLevel.Error,
                Lagrange.Core.Event.EventArg.LogLevel.Fatal => LogLevel.Error,
                _ => LogLevel.Info
            };
            if (!LagrangeConfig.DebugMode && (level == LogLevel.Debug || e.EventMessage.Contains("SSOFrame")))
            {
                return;
            }
            LogHelper.WriteLog(level, e.Tag, e.EventMessage);
        }

        private bool DisposeBotContext()
        {
            try
            {
                BotContext.Invoker.OnBotLogEvent -= Invoker_OnBotLogEvent;
                BotContext.Invoker.OnBotCaptchaEvent -= Invoker_OnBotCaptchaEvent;
                BotContext.Invoker.OnBotOnlineEvent -= Invoker_OnBotOnlineEvent;
                BotContext.Invoker.OnBotOfflineEvent -= Invoker_OnBotOfflineEvent;
                BotContext.Invoker.OnPinChangedEvent -= Invoker_OnPinChangedEvent;
                BotContext.Invoker.OnBotNewDeviceVerify -= Invoker_OnBotNewDeviceVerify;
                BotContext.Invoker.OnDeviceLoginEvent -= Invoker_OnDeviceLoginEvent;
                BotContext.Invoker.OnFriendMessageReceived -= Invoker_OnFriendMessageReceived;
                BotContext.Invoker.OnFriendPokeEvent -= Invoker_OnFriendPokeEvent;
                BotContext.Invoker.OnFriendRecallEvent -= Invoker_OnFriendRecallEvent;
                BotContext.Invoker.OnFriendRequestEvent -= Invoker_OnFriendRequestEvent;
                BotContext.Invoker.OnGroupAdminChangedEvent -= Invoker_OnGroupAdminChangedEvent;
                BotContext.Invoker.OnGroupEssenceEvent -= Invoker_OnGroupEssenceEvent;
                BotContext.Invoker.OnGroupInvitationReceived -= Invoker_OnGroupInvitationReceived;
                BotContext.Invoker.OnGroupInvitationRequestEvent -= Invoker_OnGroupInvitationRequestEvent;
                BotContext.Invoker.OnGroupJoinRequestEvent -= Invoker_OnGroupJoinRequestEvent;
                BotContext.Invoker.OnGroupMemberDecreaseEvent -= Invoker_OnGroupMemberDecreaseEvent;
                BotContext.Invoker.OnGroupMemberEnterEvent -= Invoker_OnGroupMemberEnterEvent;
                BotContext.Invoker.OnGroupMemberIncreaseEvent -= Invoker_OnGroupMemberIncreaseEvent;
                BotContext.Invoker.OnGroupMemberMuteEvent -= Invoker_OnGroupMemberMuteEvent;
                BotContext.Invoker.OnGroupMessageReceived -= Invoker_OnGroupMessageReceived;
                BotContext.Invoker.OnGroupMuteEvent -= Invoker_OnGroupMuteEvent;
                BotContext.Invoker.OnGroupNameChangeEvent -= Invoker_OnGroupNameChangeEvent;
                BotContext.Invoker.OnGroupPokeEvent -= Invoker_OnGroupPokeEvent;
                BotContext.Invoker.OnGroupReactionEvent -= Invoker_OnGroupReactionEvent;
                BotContext.Invoker.OnGroupRecallEvent -= Invoker_OnGroupRecallEvent;
                BotContext.Invoker.OnGroupTodoEvent -= Invoker_OnGroupTodoEvent;
                BotContext.Invoker.OnTempMessageReceived -= Invoker_OnTempMessageReceived;
                MessageChainPaser.OnFileUploaded -= MessageChainPaser_OnFileUploaded;

                LoginToken?.Cancel();
                BotContext?.Dispose();

                LogHelper.Info("销毁Bot实例", $"成功");
                return true;
            }
            catch (Exception e)
            {
                LogHelper.Error("销毁Bot实例", e);
                return false;
            }
        }

        private bool Login()
        {
            try
            {
                LoginToken?.Cancel();
                LoginToken = new();
                // 首先尝试使用缓存的凭据登录
                // 失败后再使用密码登录 (不实现)
                // 若不存在密码，则使用二维码
                var keystore = BotContext.UpdateKeystore();
                bool wait = RequestWaiter.Wait("LagrangeCoreLogin", (int)TimeSpan.FromSeconds(60).TotalMilliseconds, () =>
                {
                    bool success;
                    if (ForceQRLogin)
                    {
                        success = QrCodeLogin();
                    }
                    else
                    {
                        success = SessionLogin(keystore)
                            || PasswordLogin(keystore)
                            || QrCodeLogin();
                    }
                    LogHelper.Info("账号登录", $"登录结果:{success}；等待在线信号...");

                    if (!success)
                    {
                        RequestWaiter.TriggerByKey("LagrangeCoreLogin", false);
                    }
                }, out object? r);
                bool waitResult = r != null && (bool)r;
                if (wait)
                {
                    if (waitResult)
                    {
                        LogHelper.Info("账号登录", $"登录成功，{BotContext.BotName}({BotContext.BotUin})");
                    }
                    else
                    {
                        LogHelper.Error("账号登录", "登录失败");
                    }
                }
                else
                {
                    LogHelper.Error("账号登录", "登录超时");
                    LoginToken?.Cancel();
                    return false;
                }
                return waitResult;
            }
            catch (Exception ex)
            {
                LogHelper.Error("账号登录", ex);
                return false;
            }
        }

        private bool QrCodeLogin()
        {
            LogHelper.Info("二维码登录", "获取二维码...");
            (string url, byte[] qrCode)? captcha = BotContext.FetchQrCode().Result;
            if (captcha == null || !captcha.HasValue)
            {
                LogHelper.Info("二维码登录", "获取二维码失败");
                return false;
            }
            LogHelper.Info("二维码登录", "二维码获取成功");
            QRCodeDisplayAction?.Invoke(captcha.Value.url, captcha.Value.qrCode);
            LogHelper.Info("二维码登录", "开始等待扫码状态...");
            var loginResult = ((Task<bool>)BotContext.LoginByQrCode(LoginToken.Token)).Result;
            if (loginResult)
            {
                LogHelper.Info("二维码登录", "扫码成功");
            }
            QRCodeFinishedAction?.Invoke();

            return loginResult;
        }

        private bool PasswordLogin(BotKeystore keystore)
        {
            return false;
        }

        private bool SessionLogin(BotKeystore keystore)
        {
            if (keystore.Session == null
                || keystore.Session.TempPassword == null
                || keystore.Session.TempPassword.Length == 0)
            {
                return false;
            }
            LogHelper.Info("缓存登录", "尝试使用会话缓存进行登录");

            return BotContext.LoginByPassword(LoginToken.Token).Result;
        }
    }
}