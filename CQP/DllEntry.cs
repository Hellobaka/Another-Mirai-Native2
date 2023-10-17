using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.WebSocket;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Another_Mirai_Native.Export
{
    public class DllMain
    {
        [DllExport(ExportName = "CQ_sendGroupMsg", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_sendGroupMsg(int authCode, long groupId, IntPtr msg)
        {
            string text = msg.ToString(Helper.GB18030);
            return CallCore(authCode, groupId, text).ToInt();
        }

        [DllExport(ExportName = "CQ_sendGroupQuoteMsg", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_sendGroupQuoteMsg(int authCode, long groupId, int msgId, IntPtr msg)
        {
            string text = msg.ToString(Helper.GB18030);
            return CallCore(authCode, groupId, msgId, text).ToInt();
        }

        [DllExport(ExportName = "CQ_sendPrivateMsg", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_sendPrivateMsg(int authCode, long qqId, IntPtr msg)
        {
            string text = msg.ToString(Helper.GB18030);
            return CallCore(authCode, qqId, text).ToInt();
        }

        [DllExport(ExportName = "CQ_deleteMsg", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_deleteMsg(int authCode, long msgId)
        {
            return CallCore(authCode, msgId).ToInt();
        }

        [DllExport(ExportName = "CQ_sendLikeV2", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_sendLikeV2(int authCode, long qqId, int count)
        {
            return CallCore(authCode, qqId, count).ToInt();
        }

        [DllExport(ExportName = "CQ_getCookiesV2", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getCookiesV2(int authCode, IntPtr domain)
        {
            string text = domain.ToString(Helper.GB18030);
            var r = CallCore(authCode, text);
            return r != null ? r.ToString().ToNative() : "".ToNative();
        }

        [DllExport(ExportName = "CQ_getRecordV2", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getRecordV2(int authCode, IntPtr file, IntPtr format)
        {
            string path = file.ToString(Helper.GB18030);
            string f = format.ToString(Helper.GB18030);
            var r = CallCore(authCode, path, f);
            return r != null ? r.ToString().ToNative() : "".ToNative();
        }

        [DllExport(ExportName = "CQ_getCsrfToken", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getCsrfToken(int authCode)
        {
            var r = CallCore(authCode);
            return r != null ? r.ToString().ToNative() : "".ToNative();
        }

        [DllExport(ExportName = "CQ_getAppDirectory", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getAppDirectory(int authCode)
        {
            var r = CallCore(authCode);
            return r != null ? r.ToString().ToNative() : "".ToNative();
        }

        [DllExport(ExportName = "CQ_getLoginQQ", CallingConvention = CallingConvention.StdCall)]
        public static long CQ_getLoginQQ(int authCode)
        {
            return CallCore(authCode).ToLong(10001);
        }

        [DllExport(ExportName = "CQ_getLoginNick", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getLoginNick(int authCode)
        {
            var r = CallCore(authCode);
            return r != null ? r.ToString().ToNative() : "".ToNative();
        }

        [DllExport(ExportName = "CQ_setGroupKick", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupKick(int authCode, long groupId, long qqId, bool refuses)
        {
            return CallCore(authCode, groupId, qqId, refuses).ToInt();
        }

        [DllExport(ExportName = "CQ_setGroupBan", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupBan(int authCode, long groupId, long qqId, long time)
        {
            return CallCore(authCode, groupId, qqId, time).ToInt();
        }

        [DllExport(ExportName = "CQ_setGroupAdmin", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupAdmin(int authCode, long groupId, long qqId, bool isSet)
        {
            return CallCore(authCode, groupId, qqId, isSet).ToInt();
        }

        [DllExport(ExportName = "CQ_setGroupSpecialTitle", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupSpecialTitle(int authCode, long groupId, long qqId, IntPtr title, long durationTime)
        {
            string text = title.ToString(Helper.GB18030);
            return CallCore(authCode, groupId, qqId, text, durationTime).ToInt();
        }

        [DllExport(ExportName = "CQ_setGroupWholeBan", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupWholeBan(int authCode, long groupId, bool isOpen)
        {
            return CallCore(authCode, groupId, isOpen).ToInt();
        }

        [DllExport(ExportName = "CQ_setGroupAnonymousBan", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupAnonymousBan(int authCode, long groupId, IntPtr anonymous, long banTime)
        {
            string info = anonymous.ToString(Helper.GB18030);
            return CallCore(authCode, groupId, info, banTime).ToInt();
        }

        [DllExport(ExportName = "CQ_setGroupAnonymous", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupAnonymous(int authCode, long groupId, bool isOpen)
        {
            return CallCore(authCode, groupId, isOpen).ToInt();
        }

        [DllExport(ExportName = "CQ_setGroupCard", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupCard(int authCode, long groupId, long qqId, IntPtr newCard)
        {
            string text = newCard.ToString(Helper.GB18030);
            return CallCore(authCode, groupId, qqId, text).ToInt();
        }

        [DllExport(ExportName = "CQ_setGroupLeave", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupLeave(int authCode, long groupId, bool isDisband)
        {
            return CallCore(authCode, groupId, isDisband).ToInt();
        }

        [DllExport(ExportName = "CQ_setDiscussLeave", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setDiscussLeave(int authCode, long discussId)
        {
            return CallCore(authCode, discussId).ToInt();
        }

        [DllExport(ExportName = "CQ_setFriendAddRequest", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setFriendAddRequest(int authCode, IntPtr identifying, int requestType, IntPtr appendMsg)
        {
            string id = identifying.ToString(Helper.GB18030);
            string text = appendMsg.ToString(Helper.GB18030);
            return CallCore(authCode, id, requestType, text).ToInt();
        }

        [DllExport(ExportName = "CQ_setGroupAddRequestV2", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupAddRequestV2(int authCode, IntPtr identifying, int requestType, int responseType, IntPtr appendMsg)
        {
            string id = identifying.ToString(Helper.GB18030);
            string text = appendMsg.ToString(Helper.GB18030);
            return CallCore(authCode, id, requestType, responseType, text).ToInt();
        }

        [DllExport(ExportName = "CQ_addLog", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_addLog(int authCode, int priority, IntPtr type, IntPtr msg)
        {
            string id = type.ToString(Helper.GB18030);
            string text = msg.ToString(Helper.GB18030);
            return CallCore(authCode, priority, id, text).ToInt();
        }

        [DllExport(ExportName = "CQ_setFatal", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setFatal(int authCode, IntPtr errorMsg)
        {
            string text = errorMsg.ToString(Helper.GB18030);
            return CallCore(authCode, text).ToInt();
        }

        [DllExport(ExportName = "CQ_getGroupMemberInfoV2", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getGroupMemberInfoV2(int authCode, long groupId, long qqId, bool isCache)
        {
            var r = CallCore(authCode, groupId, qqId, isCache);
            return r != null ? r.ToString().ToNative() : new GroupMemberInfo().ToNativeBase64().ToNative();
        }

        [DllExport(ExportName = "CQ_getGroupMemberList", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getGroupMemberList(int authCode, long groupId)
        {
            var r = CallCore(authCode, groupId);
            return r != null ? r.ToString().ToNative() : GroupMemberInfo.CollectionToList(new()).ToNative();
        }

        [DllExport(ExportName = "CQ_getGroupList", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getGroupList(int authCode)
        {
            var r = CallCore(authCode);
            return r != null ? r.ToString().ToNative() : GroupInfo.CollectionToList(new()).ToNative();
        }

        [DllExport(ExportName = "CQ_getStrangerInfo", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getStrangerInfo(int authCode, long qqId, bool notCache)
        {
            var r = CallCore(authCode, qqId, notCache);
            return r != null ? r.ToString().ToNative() : new StrangerInfo().ToNativeBase64().ToNative();
        }

        [DllExport(ExportName = "CQ_canSendImage", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_canSendImage(int authCode)
        {
            return CallCore(authCode).ToInt();
        }

        [DllExport(ExportName = "CQ_canSendRecord", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_canSendRecord(int authCode)
        {
            return CallCore(authCode).ToInt();
        }

        [DllExport(ExportName = "CQ_getImage", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getImage(int authCode, IntPtr file)
        {
            string text = file.ToString(Helper.GB18030);
            var r = CallCore(authCode, text);
            return r != null ? r.ToString().ToNative() : "".ToNative();
        }

        [DllExport(ExportName = "CQ_getGroupInfo", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getGroupInfo(int authCode, long groupId, bool notCache)
        {
            var r = CallCore(authCode, groupId, notCache);
            return r != null ? r.ToString().ToNative() : new GroupInfo().ToNativeBase64(false).ToNative();
        }

        [DllExport(ExportName = "CQ_getFriendList", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getFriendList(int authCode, bool reserved)
        {
            var r = CallCore(authCode, reserved);
            return r != null ? r.ToString().ToNative() : FriendInfo.CollectionToList(new()).ToNative();
        }

        [DllExport(ExportName = "cq_start", CallingConvention = CallingConvention.StdCall)]
        public static bool cq_start(IntPtr path, int authCode)
        {
            return true;
        }

        private static object CallCore(params object[] args)
        {
            StackFrame[] stackFrames = new StackTrace().GetFrames();

            if (stackFrames.Length > 1)
            {
                string functionName = stackFrames[1].GetMethod().Name;
                var r = Client.Instance.Invoke("InvokeCQP_" + functionName, args);
                if (r != null && string.IsNullOrEmpty(r.Message))
                {
                    return r.Result;
                }
                LogHelper.Error("CallCore", "调用错误");
                return null;
            }
            else
            {
                LogHelper.Error("CallCore", "无法获取方法名称");
                return null;
            }
        }
    }
}