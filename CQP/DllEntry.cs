using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.RPC;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Another_Mirai_Native.Export
{
    public class DllMain
    {
        /// <summary>
        /// 发送群消息
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId">目标群</param>
        /// <param name="msg">消息内容</param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_sendGroupMsg", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_sendGroupMsg(int authCode, long groupId, IntPtr msg)
        {
            string text = msg.ToString(Helper.GB18030);
            return CallCore(authCode, groupId, text).ToInt();
        }

        /// <summary>
        /// 发送群消息
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId">目标群</param>
        /// <param name="msgId"></param>
        /// <param name="msg">消息内容</param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_sendGroupQuoteMsg", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_sendGroupQuoteMsg(int authCode, long groupId, int msgId, IntPtr msg)
        {
            string text = msg.ToString(Helper.GB18030);
            return CallCore(authCode, groupId, msgId, text).ToInt();
        }

        /// <summary>
        /// 发送好友消息
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId">目标帐号</param>
        /// <param name="msg">消息内容</param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_sendPrivateMsg", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_sendPrivateMsg(int authCode, long qqId, IntPtr msg)
        {
            string text = msg.ToString(Helper.GB18030);
            return CallCore(authCode, qqId, text).ToInt();
        }

        /// <summary>
        /// 发送讨论组消息
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="discussId">目标讨论组</param>
        /// <param name="msg">消息内容</param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_sendDiscussMsg", CallingConvention = CallingConvention.StdCall)]
        public int CQ_sendDiscussMsg(int authCode, long discussId, IntPtr msg)
        {
            string text = msg.ToString(Helper.GB18030);
            return CallCore(authCode, discussId, text).ToInt();
        }

        /// <summary>
        /// 撤回消息
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="msgId">消息ID</param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_deleteMsg", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_deleteMsg(int authCode, long msgId)
        {
            return CallCore(authCode, msgId).ToInt();
        }

        /// <summary>
        /// 发送赞
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="qqId">目标帐号</param>
        /// <param name="count">赞的次数，最多10次</param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_sendLikeV2", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_sendLikeV2(int authCode, long qqId, int count)
        {
            return CallCore(authCode, qqId, count).ToInt();
        }

        /// <summary>
        /// 取Cookies(慎用,此接口需要严格授权)
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="domain">目标域名，如 api.example.com</param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_getCookiesV2", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getCookiesV2(int authCode, IntPtr domain)
        {
            string text = domain.ToString(Helper.GB18030);
            var r = CallCore(authCode, text);
            return r != null ? r.ToString().ToNative() : "".ToNative();
        }

        /// <summary>
        /// 接收语音，并返回语音文件绝对路径
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="file">收到消息中的语音文件名(file)</param>
        /// <param name="format">应用所需的格式</param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_getRecordV2", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getRecordV2(int authCode, IntPtr file, IntPtr format)
        {
            string path = file.ToString(Helper.GB18030);
            string f = format.ToString(Helper.GB18030);
            var r = CallCore(authCode, path, f);
            return r != null ? r.ToString().ToNative() : "".ToNative();
        }

        /// <summary>
        /// 取CsrfToken(慎用,此接口需要严格授权)
        /// </summary>
        /// <param name="authCode"></param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_getCsrfToken", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getCsrfToken(int authCode)
        {
            var r = CallCore(authCode);
            return r != null ? r.ToString().ToNative() : "".ToNative();
        }

        /// <summary>
        /// 取应用目录
        /// </summary>
        /// <param name="authCode"></param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_getAppDirectory", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getAppDirectory(int authCode)
        {
            var r = CallCore(authCode);
            return r != null ? r.ToString().ToNative() : "".ToNative();
        }

        /// <summary>
        /// 取登录帐号
        /// </summary>
        /// <param name="authCode"></param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_getLoginQQ", CallingConvention = CallingConvention.StdCall)]
        public static long CQ_getLoginQQ(int authCode)
        {
            return CallCore(authCode).ToLong(10001);
        }

        /// <summary>
        /// 取登录昵称
        /// </summary>
        /// <param name="authCode"></param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_getLoginNick", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getLoginNick(int authCode)
        {
            var r = CallCore(authCode);
            return r != null ? r.ToString().ToNative() : "".ToNative();
        }

        /// <summary>
        /// 置群员移除
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId">目标群</param>
        /// <param name="qqId">目标帐号</param>
        /// <param name="refuses">如果为真，则“不再接收此人加群申请”，请慎用</param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_setGroupKick", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupKick(int authCode, long groupId, long qqId, bool refuses)
        {
            return CallCore(authCode, groupId, qqId, refuses).ToInt();
        }

        /// <summary>
        /// 置群员禁言
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId">目标群</param>
        /// <param name="qqId">目标帐号</param>
        /// <param name="time">禁言的时间，单位为秒。如果要解禁，这里填写0</param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_setGroupBan", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupBan(int authCode, long groupId, long qqId, long time)
        {
            return CallCore(authCode, groupId, qqId, time).ToInt();
        }

        /// <summary>
        /// 置群管理员
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId">目标群</param>
        /// <param name="qqId">被设置的帐号</param>
        /// <param name="isSet">真/设置管理员 假/取消管理员</param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_setGroupAdmin", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupAdmin(int authCode, long groupId, long qqId, bool isSet)
        {
            return CallCore(authCode, groupId, qqId, isSet).ToInt();
        }

        /// <summary>
        /// 置群成员专属头衔
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId">目标群</param>
        /// <param name="qqId">目标帐号</param>
        /// <param name="title">如果要删除，这里填空</param>
        /// <param name="durationTime">专属头衔有效期，单位为秒。如果永久有效，这里填写-1</param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_setGroupSpecialTitle", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupSpecialTitle(int authCode, long groupId, long qqId, IntPtr title, long durationTime)
        {
            string text = title.ToString(Helper.GB18030);
            return CallCore(authCode, groupId, qqId, text, durationTime).ToInt();
        }

        /// <summary>
        /// 置全群禁言
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId">目标群</param>
        /// <param name="isOpen">真/开启 假/关闭</param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_setGroupWholeBan", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupWholeBan(int authCode, long groupId, bool isOpen)
        {
            return CallCore(authCode, groupId, isOpen).ToInt();
        }

        /// <summary>
        /// 置匿名群员禁言
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId">目标群</param>
        /// <param name="anonymous">群消息事件收到的“匿名”参数</param>
        /// <param name="banTime">禁言的时间，单位为秒。不支持解禁</param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_setGroupAnonymousBan", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupAnonymousBan(int authCode, long groupId, IntPtr anonymous, long banTime)
        {
            string info = anonymous.ToString(Helper.GB18030);
            return CallCore(authCode, groupId, info, banTime).ToInt();
        }

        /// <summary>
        /// 置群匿名设置
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId"></param>
        /// <param name="isOpen"></param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_setGroupAnonymous", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupAnonymous(int authCode, long groupId, bool isOpen)
        {
            return CallCore(authCode, groupId, isOpen).ToInt();
        }

        /// <summary>
        /// 置群成员名片
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId">目标群</param>
        /// <param name="qqId">被设置的帐号</param>
        /// <param name="newCard"></param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_setGroupCard", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupCard(int authCode, long groupId, long qqId, IntPtr newCard)
        {
            string text = newCard.ToString(Helper.GB18030);
            return CallCore(authCode, groupId, qqId, text).ToInt();
        }

        /// <summary>
        /// 置群退出
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId">目标群</param>
        /// <param name="isDisband">真/解散本群(群主) 假/退出本群(管理、群成员)</param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_setGroupLeave", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupLeave(int authCode, long groupId, bool isDisband)
        {
            return CallCore(authCode, groupId, isDisband).ToInt();
        }

        /// <summary>
        /// 置讨论组退出
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="discussId">目标讨论组</param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_setDiscussLeave", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setDiscussLeave(int authCode, long discussId)
        {
            return CallCore(authCode, discussId).ToInt();
        }

        /// <summary>
        /// 置好友添加请求
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="identifying">请求事件收到的“反馈标识”参数</param>
        /// <param name="requestType">1通过 2拒绝</param>
        /// <param name="appendMsg">添加后的好友备注</param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_setFriendAddRequest", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setFriendAddRequest(int authCode, IntPtr identifying, int requestType, IntPtr appendMsg)
        {
            string id = identifying.ToString(Helper.GB18030);
            string text = appendMsg.ToString(Helper.GB18030);
            return CallCore(authCode, id, requestType, text).ToInt();
        }

        /// <summary>
        /// 置群添加请求
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="identifying">请求事件收到的“反馈标识”参数</param>
        /// <param name="requestType">1申请 2邀请</param>
        /// <param name="responseType">1通过 2拒绝</param>
        /// <param name="appendMsg">操作理由，仅 requestType=1 且 responseType=2 时可用</param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_setGroupAddRequestV2", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupAddRequestV2(int authCode, IntPtr identifying, int requestType, int responseType, IntPtr appendMsg)
        {
            string id = identifying.ToString(Helper.GB18030);
            string text = appendMsg.ToString(Helper.GB18030);
            return CallCore(authCode, id, requestType, responseType, text).ToInt();
        }

        /// <summary>
        /// 增加运行日志
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="priority"></param>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_addLog", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_addLog(int authCode, int priority, IntPtr type, IntPtr msg)
        {
            string id = type.ToString(Helper.GB18030);
            string text = msg.ToString(Helper.GB18030);
            return CallCore(authCode, priority, id, text).ToInt();
        }

        /// <summary>
        /// 置致命错误提示
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_setFatal", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setFatal(int authCode, IntPtr errorMsg)
        {
            string text = errorMsg.ToString(Helper.GB18030);
            return CallCore(authCode, text).ToInt();
        }

        /// <summary>
        /// 取群成员信息(支持缓存)
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId">目标帐号所在群</param>
        /// <param name="qqId">目标帐号</param>
        /// <param name="isCache"></param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_getGroupMemberInfoV2", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getGroupMemberInfoV2(int authCode, long groupId, long qqId, bool isCache)
        {
            var r = CallCore(authCode, groupId, qqId, isCache);
            return r != null ? r.ToString().ToNative() : new GroupMemberInfo().ToNativeBase64().ToNative();
        }

        /// <summary>
        /// 取群成员列表
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId">目标帐号所在群</param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_getGroupMemberList", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getGroupMemberList(int authCode, long groupId)
        {
            var r = CallCore(authCode, groupId);
            return r != null ? r.ToString().ToNative() : GroupMemberInfo.CollectionToList(new()).ToNative();
        }

        /// <summary>
        /// 取群列表
        /// </summary>
        /// <param name="authCode"></param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_getGroupList", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getGroupList(int authCode)
        {
            var r = CallCore(authCode);
            return r != null ? r.ToString().ToNative() : GroupInfo.CollectionToList(new()).ToNative();
        }

        /// <summary>
        /// 取陌生人信息(支持缓存)
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="qqId">目标帐号</param>
        /// <param name="notCache"></param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_getStrangerInfo", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getStrangerInfo(int authCode, long qqId, bool notCache)
        {
            var r = CallCore(authCode, qqId, notCache);
            return r != null ? r.ToString().ToNative() : new StrangerInfo().ToNativeBase64().ToNative();
        }

        /// <summary>
        /// 是否支持发送图片
        /// </summary>
        /// <param name="authCode"></param>
        /// <returns>返回大于 0 为支持，等于 0 为不支持</returns>
        [DllExport(ExportName = "CQ_canSendImage", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_canSendImage(int authCode)
        {
            return CallCore(authCode).ToInt();
        }

        /// <summary>
        /// 是否支持发送语音
        /// </summary>
        /// <param name="authCode"></param>
        /// <returns>返回大于 0 为支持，等于 0 为不支持</returns>
        [DllExport(ExportName = "CQ_canSendRecord", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_canSendRecord(int authCode)
        {
            return CallCore(authCode).ToInt();
        }

        /// <summary>
        /// 接收图片，并返回图片文件绝对路径
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="file">收到消息中的图片文件名(file)</param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_getImage", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getImage(int authCode, IntPtr file)
        {
            string text = file.ToString(Helper.GB18030);
            var r = CallCore(authCode, text);
            return r != null ? r.ToString().ToNative() : "".ToNative();
        }

        /// <summary>
        /// 取群信息(支持缓存)
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId">目标群</param>
        /// <param name="notCache"></param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_getGroupInfo", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getGroupInfo(int authCode, long groupId, bool notCache)
        {
            var r = CallCore(authCode, groupId, notCache);
            return r != null ? r.ToString().ToNative() : new GroupInfo().ToNativeBase64(false).ToNative();
        }

        /// <summary>
        /// 取好友列表
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="reserved"></param>
        /// <returns></returns>
        [DllExport(ExportName = "CQ_getFriendList", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getFriendList(int authCode, bool reserved)
        {
            var r = CallCore(authCode, reserved);
            return r != null ? r.ToString().ToNative() : FriendInfo.CollectionToList(new()).ToNative();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="authCode"></param>
        /// <returns></returns>
        [DllExport(ExportName = "cq_start", CallingConvention = CallingConvention.StdCall)]
        public static bool cq_start(IntPtr path, int authCode)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
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