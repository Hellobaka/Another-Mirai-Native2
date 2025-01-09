using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Other.XiaoLiZi;
using System.Runtime.InteropServices;

namespace Another_Mirai_Native.Native.Handler.XiaoLiZi
{
    // HandlerBase => Handler => Proxy => Plugin
    public partial class Loader
    {
        private void CreateProxy()
        {
            AdminChange = new Type_AdminChange(Proxy_AdminChange);
            AppInfoFunction = new Type_AppInfo(Proxy_AppInfoFunction);
            Disable = new Type_Disable(Proxy_Disable);
            Enable = new Type_Enable(Proxy_Enable);
            Exit = new Type_Exit(Proxy_Exit);
            FriendAdded = new Type_FriendAdded(Proxy_FriendAdded);
            FriendAddRequest = new Type_FriendAddRequest(Proxy_FriendAddRequest);
            GroupAddRequest = new Type_GroupAddRequest(Proxy_GroupAddRequest);
            GroupBan = new Type_GroupBan(Proxy_GroupBan);
            GroupMemberDecrease = new Type_GroupMemberDecrease(Proxy_GroupMemberDecrease);
            GroupMemberIncrease = new Type_GroupMemberIncrease(Proxy_GroupMemberIncrease);
            GroupMsg = new Type_GroupMsg(Proxy_GroupMsg);
            Initialize = new Type_Initialize(Proxy_Initialize);
            PrivateMsg = new Type_PrivateMsg(Proxy_PrivateMsg);
            StartUp = new Type_StartUp(Proxy_StartUp);
            Upload = new Type_Upload(Proxy_Upload);
        }

        private int Proxy_AdminChange(int subType, int sendTime, long fromGroup, long beingOperateQQ)
        {
            EventTypeBase e = new()
            {
                EventType = subType == 1 ? Model.Enums.Other.XiaoLiZi.EventTypeEnum.Group_AdministratorTook
                    : Model.Enums.Other.XiaoLiZi.EventTypeEnum.Group_AdministratorGave,
                MessageTimestamp = (int)Helper.TimeStamp,
                SourceGroupName = fromGroup.ToString(),
                SourceGroupQQ = fromGroup,
                ThisQQ = AppConfig.Instance.CurrentQQ,
                TriggerQQ = beingOperateQQ,
                TriggerQQName = beingOperateQQ.ToString(),
                MessageContent = "",
                MessageSeq = 0,
                OperateQQ = 0,
                OperateQQName = 0.ToString(),
                EventSubType = subType,
            };

            return SafeInvokeAndFreeMemory(e, ReceiveEvent);
        }

        private IntPtr Proxy_AppInfoFunction()
        {
            return Marshal.StringToHGlobalAuto($"9,{PluginName}");
        }

        private int Proxy_Disable()
        {
            return SafeInvoke(AppDisabled);
        }

        private int Proxy_Enable()
        {
            return SafeInvoke(AppEnabled);
        }

        private int Proxy_Exit()
        {
            return SafeInvoke(AppExit);
        }

        private int Proxy_FriendAdded(int subType, int sendTime, long fromQQ)
        {
            EventTypeBase e = new()
            {
                EventType = Model.Enums.Other.XiaoLiZi.EventTypeEnum.Friend_NewFriend,
                MessageTimestamp = (int)Helper.TimeStamp,
                ThisQQ = AppConfig.Instance.CurrentQQ,
                TriggerQQ = fromQQ,
                TriggerQQName = fromQQ.ToString(),
                EventSubType = subType,
                MessageContent = "",
                MessageSeq = 0,
                OperateQQ = 0,
                OperateQQName = 0.ToString(),
                SourceGroupName = "",
                SourceGroupQQ = 0,
            };

            return SafeInvokeAndFreeMemory(e, ReceiveEvent);
        }

        private int Proxy_FriendAddRequest(int subType, int sendTime, long fromQQ, IntPtr msg, string responseFlag)
        {
            EventTypeBase e = new()
            {
                EventType = Model.Enums.Other.XiaoLiZi.EventTypeEnum.Friend_FriendRequest,
                MessageContent = msg.ToString(Helper.GB18030),
                MessageSeq = CacheResponse(responseFlag),
                EventSubType = 2,
                ThisQQ = AppConfig.Instance.CurrentQQ,
                TriggerQQ = fromQQ,
                TriggerQQName = fromQQ.ToString(),
                OperateQQ = 0,
                OperateQQName = 0.ToString(),
                MessageTimestamp = sendTime,
                SourceGroupQQ = 0,
                SourceGroupName = "",
            };

            return SafeInvokeAndFreeMemory(e, ReceiveEvent);
        }

        private long CacheResponse(string responseFlag)
        {
            if (RequestCache.CachedStrings.TryGetValue(responseFlag, out string? value))
            {
                return long.Parse(value);
            }
            RequestCache.CachedStrings[responseFlag] = Helper.MakeUniqueID().ToString();
            return long.Parse(RequestCache.CachedStrings[responseFlag]);
        }

        private int Proxy_GroupAddRequest(int subType, int sendTime, long fromGroup, long fromQQ, IntPtr msg, string responseFlag)
        {
            EventTypeBase e = new()
            {
                EventType = Model.Enums.Other.XiaoLiZi.EventTypeEnum.Group_MemberVerifying,
                MessageContent = msg.ToString(Helper.GB18030),
                MessageSeq = CacheResponse(responseFlag),
                MessageTimestamp = sendTime,
                SourceGroupQQ = fromGroup,
                SourceGroupName = fromGroup.ToString(),
                ThisQQ = AppConfig.Instance.CurrentQQ,
                TriggerQQ = fromQQ,
                TriggerQQName = fromQQ.ToString(),
                OperateQQ = 0,
                OperateQQName = "",
                EventSubType = 0,
            };

            return SafeInvokeAndFreeMemory(e, ReceiveEvent);
        }

        private int Proxy_GroupBan(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ, long duration)
        {
            EventTypeBase e = new()
            {
                EventType = duration == 0 ? Model.Enums.Other.XiaoLiZi.EventTypeEnum.Group_MemberNotShutUp
                    : Model.Enums.Other.XiaoLiZi.EventTypeEnum.Group_MemberShutUp,
                MessageSeq = duration,
                MessageTimestamp = sendTime,
                SourceGroupQQ = fromGroup,
                SourceGroupName = fromGroup.ToString(),
                ThisQQ = AppConfig.Instance.CurrentQQ,
                TriggerQQ = beingOperateQQ,
                TriggerQQName = beingOperateQQ.ToString(),
                OperateQQ = fromQQ,
                OperateQQName = fromQQ.ToString(),
                MessageContent = "",
                EventSubType = 0,
            };

            return SafeInvokeAndFreeMemory(e, ReceiveEvent);
        }

        private int Proxy_GroupMemberDecrease(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ)
        {
            EventTypeBase e = new()
            {
                EventType = subType == 1 ? Model.Enums.Other.XiaoLiZi.EventTypeEnum.Group_MemberQuit
                    : Model.Enums.Other.XiaoLiZi.EventTypeEnum.Group_MemberKicked,
                MessageTimestamp = sendTime,
                SourceGroupQQ = fromGroup,
                SourceGroupName = fromGroup.ToString(),
                ThisQQ = AppConfig.Instance.CurrentQQ,
                TriggerQQ = beingOperateQQ,
                TriggerQQName = beingOperateQQ.ToString(),
                OperateQQ = fromQQ,
                OperateQQName = fromQQ.ToString(),
                MessageContent = "",
                MessageSeq = 0,
                EventSubType = 0,
            };

            return SafeInvokeAndFreeMemory(e, ReceiveEvent);
        }

        private int Proxy_GroupMemberIncrease(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ)
        {
            EventTypeBase e = new()
            {
                EventType = subType == 1 ? Model.Enums.Other.XiaoLiZi.EventTypeEnum.Group_MemberJoined
                    : Model.Enums.Other.XiaoLiZi.EventTypeEnum.Group_MemberInvited,
                SourceGroupQQ = fromGroup,
                SourceGroupName = fromGroup.ToString(),
                ThisQQ = AppConfig.Instance.CurrentQQ,
                TriggerQQ = beingOperateQQ,
                TriggerQQName = beingOperateQQ.ToString(),
                OperateQQ = fromQQ,
                OperateQQName = fromQQ.ToString(),
                EventSubType = 0,
                MessageContent = "",
                MessageSeq = 0,
                MessageTimestamp = sendTime,
            };

            return SafeInvokeAndFreeMemory(e, ReceiveEvent);
        }

        private int Proxy_GroupMsg(int subType, int msgId, long fromGroup, long fromQQ, string fromAnonymous, IntPtr msg, int font)
        {
            string message = msg.ToString(Helper.GB18030);
            GroupMessageEvent e = new()
            {
                MessageContent = MessageParser.ParseFromCQCode(message),
                SenderQQ = fromQQ,
                MessageSendTime = (int)Helper.TimeStamp,
                ThisQQ = AppConfig.Instance.CurrentQQ,
                MessageGroupQQ = fromGroup,
                AnonymousNickname = fromAnonymous,
                FontId = font,
                MessageReq = msgId,
                MessageReceiveTime = (int)Helper.TimeStamp,
                MessageType = Model.Enums.Other.XiaoLiZi.MessageTypeEnum.GroupUsualMessage,
                AnonymousFalg = 0,
                SourceGroupName = "",
                AnonymousId = 0,
                BubbleID = 0,
                GroupChatLevel = 0,
                MessageClip = 0,
                MessageClipCount = 0,
                MessageClipID = 0,
                MessageRandom = 0,
                PendantID = 0,
                ReplyMessageContent = "",
                ReservedParameters = "",
                SenderNickname = "",
                SenderTitle = ""
            };
            return SafeInvokeAndFreeMemory(e, ReceiveGroupMsg);
        }

        private int Proxy_Initialize(int authCode)
        {
            return 1;
        }

        private int Proxy_PrivateMsg(int subType, int msgId, long fromQQ, IntPtr msg, int font)
        {
            string message = msg.ToString(Helper.GB18030);
            PrivateMessageEvent e = new()
            {
                MessageContent = MessageParser.ParseFromCQCode(message),
                SenderQQ = fromQQ,
                MessageSendTime = (int)Helper.TimeStamp,
                ThisQQ = AppConfig.Instance.CurrentQQ,
                MessageReq = msgId,
                MessageReceiveTime = (int)Helper.TimeStamp,
                MessageType = Model.Enums.Other.XiaoLiZi.MessageTypeEnum.FriendUsualMessage,
                MessageRandom = 0,
                MessageClip = 0,
                MessageClipID = 0,
                MessageClipCount = 0,
                BubbleID = 0,
                FileID = "",
                FileMD5 = "",
                FileName = "",
                FileSize = 0,
                MessageGroupQQ = 0,
                MessageSeq = 0,
                MessageSubTemporaryType = 0,
                MessageSubType = 0,
                RedEnvelopeType = 0,
                SessionToken = "",
                SourceEventQQ = 0,
                SourceEventQQName = ""
            };

            return SafeInvokeAndFreeMemory(e, ReceivePrivateMsg);
        }

        private int Proxy_StartUp()
        {
            return SafeInvoke(AppEnabled);
        }

        private int Proxy_Upload(int subType, int sendTime, long fromGroup, long fromQQ, string file)
        {
            // 未找到对应事件
            return 0;
        }

        private int SafeInvoke(Delegate? action)
        {
            try
            {
                int? ret = (int?)(action?.DynamicInvoke(null));
                if (ret.HasValue)
                {
                    return ret.Value;
                }
                return 0;
            }
            catch (Exception exc)
            {
                LogHelper.Error($"Proxy_{action?.Method.Name ?? "Unknown"}", exc);
                return 0;
            }
        }

        private int SafeInvokeAndFreeMemory<T>(T e, Delegate? action) where T : struct
        {
            IntPtr p = IntPtr.Zero;
            try
            {
                p = Marshal.AllocHGlobal(Marshal.SizeOf(e));
                Marshal.StructureToPtr(e, p, false);

                int? ret = (int?)(action?.DynamicInvoke(p));
                if (ret.HasValue)
                {
                    return ret.Value;
                }
                return 0;
            }
            catch (Exception exc)
            {
                LogHelper.Error($"Proxy_{action?.Method.Name ?? "Unknown"}", exc);
                return 0;
            }
            finally
            {
                if (p != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(p);
                }
            }
        }
    }
}