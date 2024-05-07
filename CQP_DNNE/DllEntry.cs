using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.RPC;
using Another_Mirai_Native.WebSocket;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Another_Mirai_Native.Export
{
    public class DllMain
    {
        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static int CQ_sendGroupMsg(int authCode, long groupId, IntPtr msg)
        {
            string text = msg.ToString(Helper.GB18030);
            return CallCore(authCode, groupId, text).ToInt();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static int CQ_sendGroupQuoteMsg(int authCode, long groupId, int msgId, IntPtr msg)
        {
            string text = msg.ToString(Helper.GB18030);
            return CallCore(authCode, groupId, msgId, text).ToInt();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static int CQ_sendPrivateMsg(int authCode, long qqId, IntPtr msg)
        {
            string text = msg.ToString(Helper.GB18030);
            return CallCore(authCode, qqId, text).ToInt();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static int CQ_deleteMsg(int authCode, long msgId)
        {
            return CallCore(authCode, msgId).ToInt();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static int CQ_sendLikeV2(int authCode, long qqId, int count)
        {
            return CallCore(authCode, qqId, count).ToInt();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static IntPtr CQ_getCookiesV2(int authCode, IntPtr domain)
        {
            string text = domain.ToString(Helper.GB18030);
            var r = CallCore(authCode, text);
            return r != null ? r.ToString().ToNative() : "".ToNative();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static IntPtr CQ_getRecordV2(int authCode, IntPtr file, IntPtr format)
        {
            string path = file.ToString(Helper.GB18030);
            string f = format.ToString(Helper.GB18030);
            var r = CallCore(authCode, path, f);
            return r != null ? r.ToString().ToNative() : "".ToNative();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static IntPtr CQ_getCsrfToken(int authCode)
        {
            var r = CallCore(authCode);
            return r != null ? r.ToString().ToNative() : "".ToNative();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static IntPtr CQ_getAppDirectory(int authCode)
        {
            var r = CallCore(authCode);
            return r != null ? r.ToString().ToNative() : "".ToNative();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static long CQ_getLoginQQ(int authCode)
        {
            return CallCore(authCode).ToLong(10001);
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static IntPtr CQ_getLoginNick(int authCode)
        {
            var r = CallCore(authCode);
            return r != null ? r.ToString().ToNative() : "".ToNative();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static int CQ_setGroupKick(int authCode, long groupId, long qqId, uint refuses)
        {
            return CallCore(authCode, groupId, qqId, refuses).ToInt();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static int CQ_setGroupBan(int authCode, long groupId, long qqId, long time)
        {
            return CallCore(authCode, groupId, qqId, time).ToInt();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static int CQ_setGroupAdmin(int authCode, long groupId, long qqId, uint isSet)
        {
            return CallCore(authCode, groupId, qqId, isSet).ToInt();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static int CQ_setGroupSpecialTitle(int authCode, long groupId, long qqId, IntPtr title, long durationTime)
        {
            string text = title.ToString(Helper.GB18030);
            return CallCore(authCode, groupId, qqId, text, durationTime).ToInt();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static int CQ_setGroupWholeBan(int authCode, long groupId, uint isOpen)
        {
            return CallCore(authCode, groupId, isOpen).ToInt();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static int CQ_setGroupAnonymousBan(int authCode, long groupId, IntPtr anonymous, long banTime)
        {
            string info = anonymous.ToString(Helper.GB18030);
            return CallCore(authCode, groupId, info, banTime).ToInt();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static int CQ_setGroupAnonymous(int authCode, long groupId, uint isOpen)
        {
            return CallCore(authCode, groupId, isOpen).ToInt();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static int CQ_setGroupCard(int authCode, long groupId, long qqId, IntPtr newCard)
        {
            string text = newCard.ToString(Helper.GB18030);
            return CallCore(authCode, groupId, qqId, text).ToInt();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static int CQ_setGroupLeave(int authCode, long groupId, uint isDisband)
        {
            return CallCore(authCode, groupId, isDisband).ToInt();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static int CQ_setDiscussLeave(int authCode, long discussId)
        {
            return CallCore(authCode, discussId).ToInt();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static int CQ_setFriendAddRequest(int authCode, IntPtr identifying, int requestType, IntPtr appendMsg)
        {
            string id = identifying.ToString(Helper.GB18030);
            string text = appendMsg.ToString(Helper.GB18030);
            return CallCore(authCode, id, requestType, text).ToInt();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static int CQ_setGroupAddRequestV2(int authCode, IntPtr identifying, int requestType, int responseType, IntPtr appendMsg)
        {
            string id = identifying.ToString(Helper.GB18030);
            string text = appendMsg.ToString(Helper.GB18030);
            return CallCore(authCode, id, requestType, responseType, text).ToInt();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static int CQ_addLog(int authCode, int priority, IntPtr type, IntPtr msg)
        {
            Console.WriteLine("log");
            string id = type.ToString(Helper.GB18030);
            string text = msg.ToString(Helper.GB18030);
            return CallCore(authCode, priority, id, text).ToInt();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static int CQ_setFatal(int authCode, IntPtr errorMsg)
        {
            string text = errorMsg.ToString(Helper.GB18030);
            return CallCore(authCode, text).ToInt();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static IntPtr CQ_getGroupMemberInfoV2(int authCode, long groupId, long qqId, uint isCache)
        {
            var r = CallCore(authCode, groupId, qqId, isCache);
            return r != null ? r.ToString().ToNative() : new GroupMemberInfo().ToNativeBase64().ToNative();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static IntPtr CQ_getGroupMemberList(int authCode, long groupId)
        {
            var r = CallCore(authCode, groupId);
            return r != null ? r.ToString().ToNative() : GroupMemberInfo.CollectionToList(new()).ToNative();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static IntPtr CQ_getGroupList(int authCode)
        {
            var r = CallCore(authCode);
            return r != null ? r.ToString().ToNative() : GroupInfo.CollectionToList(new()).ToNative();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static IntPtr CQ_getStrangerInfo(int authCode, long qqId, uint notCache)
        {
            var r = CallCore(authCode, qqId, notCache);
            return r != null ? r.ToString().ToNative() : new StrangerInfo().ToNativeBase64().ToNative();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static int CQ_canSendImage(int authCode)
        {
            return CallCore(authCode).ToInt();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static int CQ_canSendRecord(int authCode)
        {
            return CallCore(authCode).ToInt();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static IntPtr CQ_getImage(int authCode, IntPtr file)
        {
            string text = file.ToString(Helper.GB18030);
            var r = CallCore(authCode, text);
            return r != null ? r.ToString().ToNative() : "".ToNative();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static IntPtr CQ_getGroupInfo(int authCode, long groupId, uint notCache)
        {
            var r = CallCore(authCode, groupId, notCache);
            return r != null ? r.ToString().ToNative() : new GroupInfo().ToNativeBase64(false).ToNative();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static IntPtr CQ_getFriendList(int authCode, uint reserved)
        {
            var r = CallCore(authCode, reserved);
            return r != null ? r.ToString().ToNative() : FriendInfo.CollectionToList(new()).ToNative();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        public static uint cq_start(IntPtr path, int authCode)
        {
            return 1;
        }

        private static object CallCore(params object[] args)
        {
            StackFrame[] stackFrames = new StackTrace().GetFrames();

            if (stackFrames.Length > 1)
            {
                string functionName = stackFrames[1].GetMethod().Name;
                var r = ClientManager.Client.InvokeCQPFuntcion(functionName, true, args);
                if (r != null)
                {
                    return r;
                }
                LogHelper.Error("请求服务端", $"调用 {functionName} 错误，参数: {string.Join(",", args)}");
                return null;
            }
            else
            {
                LogHelper.Error("请求服务端", "无法获取方法名称");
                return null;
            }
        }
    }
}