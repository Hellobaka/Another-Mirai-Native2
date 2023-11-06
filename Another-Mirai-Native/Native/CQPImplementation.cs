using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model.Enums;
using System.Diagnostics;
using System.Reflection;

namespace Another_Mirai_Native.Native
{
    public class CQPImplementation
    {
        public CQPluginProxy CurrentPlugin { get; set; } = null;

        public Stopwatch Stopwatch { get; set; } = new();

        public bool Testing => CurrentPlugin?.AppInfo.AuthCode == AppConfig.TestingAuthCode;

        public CQPImplementation(CQPluginProxy plugin)
        {
            CurrentPlugin = plugin;
        }

        public object Invoke(string functionName, object[] args)
        {
            Stopwatch.Restart();
            int logId = 0;
            try
            {
                var methodInfo = typeof(CQPImplementation).GetMethod(functionName, BindingFlags.NonPublic | BindingFlags.Instance);
                if (methodInfo == null)
                {
                    LogHelper.Error("调用前置检查", $"调用 {functionName} 未找到对应实现");
                    return null;
                }
                if (!CurrentPlugin.CheckPluginCanInvoke(functionName))
                {
                    LogHelper.Error("调用前置检查", $"调用 {functionName} 未定义权限");
                    return null;
                }
                var argumentList = methodInfo.GetParameters();
                if (args.Length != argumentList.Length)
                {
                    LogHelper.Error("调用前置检查", $"调用 {functionName} 参数表数量不对应");
                    return null;
                }
                object[] transformedArgs = new object[argumentList.Length];
                for (int i = 0; i < args.Length; i++)
                {
                    switch (argumentList[i].ParameterType.Name)
                    {
                        case "Int64":
                            transformedArgs[i] = Convert.ToInt64(args[i]);
                            break;

                        case "Int32":
                            transformedArgs[i] = Convert.ToInt32(args[i]);
                            break;

                        case "String":
                            transformedArgs[i] = args[i].ToString();
                            break;

                        case "Boolean":
                            transformedArgs[i] = Convert.ToBoolean(args[i]);
                            break;
                    }
                }
                LogHelper.Debug("调用前置检查", $"调用 {functionName}, 参数: {string.Join(",", transformedArgs)}");
                object result = methodInfo.Invoke(this, transformedArgs);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error("调用前置检查", ex);
                return null;
            }
            finally
            {
                Stopwatch.Stop();
                LogHelper.UpdateLogStatus(logId, $"√ {Stopwatch.ElapsedMilliseconds / 1000:f2}s");
            }
        }

        private int CQ_sendPrivateMsg(int authCode, long qqId, string msg)
        {
            if (Testing)
            {
                PluginManagerProxy.TriggerTestInvoke("CQ_sendPrivateMsg", new() { { "qqId", qqId }, { "msg", msg } });
                return 1;
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
            int logId = LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, "[↑]发送私聊消息", $"QQ:{qqId} 消息:{msg}", "处理中...");
            int ret = ProtocolManager.Instance.CurrentProtocol.SendPrivateMessage(qqId, msg);
            stopwatch.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {stopwatch.ElapsedMilliseconds / (double)1000:f2} s");
            return ret;
        }

        private int CQ_sendGroupMsg(int authCode, long groupId, string msg)
        {
            if (Testing)
            {
                PluginManagerProxy.TriggerTestInvoke("CQ_sendGroupMsg", new() { { "groupId", groupId }, { "msg", msg } });
                return 1;
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
            int logId = LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, "[↑]发送群聊消息", $"群:{groupId} 消息:{msg}", "处理中...");
            int ret = ProtocolManager.Instance.CurrentProtocol.SendGroupMessage(groupId, msg);
            stopwatch.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {stopwatch.ElapsedMilliseconds / (double)1000:f2} s");
            return ret;
        }

        private int CQ_sendGroupQuoteMsg(int authCode, long groupId, int msgId, string msg)
        {
            if (Testing)
            {
                PluginManagerProxy.TriggerTestInvoke("CQ_sendGroupQuoteMsg", new() { { "groupId", groupId }, { "msgId", msgId }, { "msg", msg } });
                return 1;
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
            int logId = LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, "[↑]发送群聊消息", $"群:{groupId} 消息:{msg}", "处理中...");
            int ret = ProtocolManager.Instance.CurrentProtocol.SendGroupMessage(groupId, msg, msgId);
            stopwatch.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {stopwatch.ElapsedMilliseconds / (double)1000:f2} s");
            return ret;
        }

        private int CQ_sendDiscussMsg(int authCode, long discussId, string msg)
        {
            if (Testing)
            {
                PluginManagerProxy.TriggerTestInvoke("CQ_sendDiscussMsg", new() { { "discussId", discussId }, { "msg", msg } });
                return 1;
            }
            return ProtocolManager.Instance.CurrentProtocol.SendDiscussMsg(discussId, msg);
        }

        private int CQ_deleteMsg(int authCode, long msgId)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            string msg = "";
            var msgCache = RequestCache.Message.FirstOrDefault(x => x.Item1 == msgId);
            if (string.IsNullOrEmpty(msgCache.Item2))
            {
                msg = msgCache.Item2;
            }
            int logId = LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, "撤回消息", $"消息ID: {msgId} 内容: {(string.IsNullOrEmpty(msg) ? "未捕获到" : msg)}", "处理中...");
            int ret = ProtocolManager.Instance.CurrentProtocol.DeleteMsg(msgId);
            stopwatch.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {stopwatch.ElapsedMilliseconds / (double)1000:f2} s");
            return ret;
        }

        private int CQ_sendLikeV2(int authCode, long qqId, int count)
        {
            return ProtocolManager.Instance.CurrentProtocol.SendLike(qqId, count);
        }

        private string CQ_getCookiesV2(int authCode, string domain)
        {
            return ProtocolManager.Instance.CurrentProtocol.GetCookies(domain);
        }

        private string CQ_getImage(int authCode, string file)
        {
            string url = Helper.GetPicUrlFromCQImg(file);
            string imgFileName = file + ".jpg";
            string imgDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\image");
            var downloadTask = Helper.DownloadFile(url, imgFileName, imgDir);
            downloadTask.Wait();

            return Path.Combine(imgDir, imgFileName);
        }

        private string CQ_getRecordV2(int authCode, string file, string format)
        {
            // 将不会实现
            return "";
        }

        private string CQ_getCsrfToken(int authCode)
        {
            return ProtocolManager.Instance.CurrentProtocol.GetCsrfToken();
        }

        private string CQ_getAppDirectory(int authCode)
        {
            string appId = CurrentPlugin.PluginId;
            string path = $@"data\app\{appId}";
            Directory.CreateDirectory(path);
            return new DirectoryInfo(path).FullName;
        }

        private long CQ_getLoginQQ(int authCode)
        {
            return ProtocolManager.Instance.CurrentProtocol.GetLoginQQ();
        }

        private string CQ_getLoginNick(int authCode)
        {
            return ProtocolManager.Instance.CurrentProtocol.GetLoginNick();
        }

        private int CQ_setGroupKick(int authCode, long groupId, long qqId, bool refuses)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            int logId = LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, "踢出群成员", $"移除群{groupId} 成员{qqId}", "处理中...");
            int ret = ProtocolManager.Instance.CurrentProtocol.SetGroupKick(groupId, qqId, refuses);
            stopwatch.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {stopwatch.ElapsedMilliseconds / (double)1000:f2} s");
            return ret;
        }

        private int CQ_setGroupBan(int authCode, long groupId, long qqId, long time)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            int logId = time > 0
                ? LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, "禁言群成员", $"禁言群{groupId} 成员{qqId} {time}秒", "处理中...")
                : LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, "解除禁言群成员", $"解除禁言群{groupId} 成员{qqId}", "处理中...");
            int ret = ProtocolManager.Instance.CurrentProtocol.SetGroupBan(groupId, qqId, time);
            stopwatch.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {stopwatch.ElapsedMilliseconds / (double)1000:f2} s");
            return ret;
        }

        private int CQ_setGroupAdmin(int authCode, long groupId, long qqId, bool isSet)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            int logId = LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, $"{(isSet ? "设置" : "取消")}群成员管理", $"{(isSet ? "设置" : "取消")}群{groupId} 成员{qqId}", "处理中...");
            int ret = ProtocolManager.Instance.CurrentProtocol.SetGroupAdmin(groupId, qqId, isSet);
            stopwatch.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {stopwatch.ElapsedMilliseconds / (double)1000:f2} s");
            return ret;
        }

        private int CQ_setGroupSpecialTitle(int authCode, long groupId, long qqId, string title, long durationTime)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            int logId = LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, "设置群成员头衔", $"设置群{groupId} 成员{qqId} 头衔 {title}", "处理中...");
            int ret = ProtocolManager.Instance.CurrentProtocol.SetGroupSpecialTitle(groupId, qqId, title, durationTime);
            stopwatch.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {stopwatch.ElapsedMilliseconds / (double)1000:f2} s");
            return ret;
        }

        private int CQ_setGroupWholeBan(int authCode, long groupId, bool isOpen)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            int logId = isOpen
                ? LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, "群全体禁言", $"禁言群{groupId}", "处理中...")
                : LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, "解除群全体禁言", $"解除禁言群{groupId}", "处理中...");
            int ret = ProtocolManager.Instance.CurrentProtocol.SetGroupWholeBan(groupId, isOpen);
            stopwatch.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {stopwatch.ElapsedMilliseconds / (double)1000:f2} s");
            return ret;
        }

        private int CQ_setGroupAnonymousBan(int authCode, long groupId, string anonymous, long banTime)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            int logId = banTime > 0
                ? LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, "禁言匿名", $"群{groupId} 成员{anonymous} {banTime}秒", "处理中...")
                : LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, "解除禁言匿名", $"群{groupId} 成员{anonymous}", "处理中...");
            int ret = ProtocolManager.Instance.CurrentProtocol.SetGroupAnonymousBan(groupId, anonymous, banTime);
            stopwatch.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {stopwatch.ElapsedMilliseconds / (double)1000:f2} s");
            return ret;
        }

        private int CQ_setGroupAnonymous(int authCode, long groupId, bool isOpen)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            int logId = LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, "设置群匿名状态", $"群{groupId} {(isOpen ? "开启" : "关闭")}", "处理中...");
            int ret = ProtocolManager.Instance.CurrentProtocol.SetGroupAnonymous(groupId, isOpen);
            stopwatch.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {stopwatch.ElapsedMilliseconds / (double)1000:f2} s");
            return ret;
        }

        private int CQ_setGroupCard(int authCode, long groupId, long qqId, string newCard)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            int logId = LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, "设置群成员名片", $"设置群{groupId} 成员{qqId} 名片 {newCard}", "处理中...");
            int ret = ProtocolManager.Instance.CurrentProtocol.SetGroupCard(groupId, qqId, newCard);
            stopwatch.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {stopwatch.ElapsedMilliseconds / (double)1000:f2} s");
            return ret;
        }

        private int CQ_setGroupLeave(int authCode, long groupId, bool isDisband)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            int logId = LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, "退出群", $"退出群{groupId}", "处理中...");
            int ret = ProtocolManager.Instance.CurrentProtocol.SetGroupLeave(groupId, isDisband);
            stopwatch.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {stopwatch.ElapsedMilliseconds / (double)1000:f2} s");
            return ret;
        }

        private int CQ_setDiscussLeave(int authCode, long discussId)
        {
            return ProtocolManager.Instance.CurrentProtocol.SetDiscussLeave(discussId);
        }

        private int CQ_setFriendAddRequest(int authCode, string identifying, int requestType, string appendMsg)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            long fromId = 0; string nick = "";
            if (RequestCache.FriendRequest.ContainsKey(identifying))
            {
                fromId = RequestCache.FriendRequest[identifying].Item1;
                nick = RequestCache.FriendRequest[identifying].Item2;
                RequestCache.FriendRequest.Remove(identifying);
            }
            int logId = LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, $"好友添加申请", $"来源: {fromId}({nick}) 操作: {(requestType == 0 ? "同意" : "拒绝")}", "处理中...");
            int ret = ProtocolManager.Instance.CurrentProtocol.SetFriendAddRequest(identifying, requestType, appendMsg);
            stopwatch.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {stopwatch.ElapsedMilliseconds / (double)1000:f2} s");
            return ret;
        }

        private int CQ_setGroupAddRequestV2(int authCode, string identifying, int requestType, int responseType, string appendMsg)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            int logId;
            long fromId = 0, groupId = 0;
            string nick = "", groupName = "";
            if (RequestCache.GroupRequest.ContainsKey(identifying))
            {
                fromId = RequestCache.GroupRequest[identifying].Item1;
                nick = RequestCache.GroupRequest[identifying].Item2;
                groupId = RequestCache.GroupRequest[identifying].Item3;
                groupName = RequestCache.GroupRequest[identifying].Item4;
                RequestCache.GroupRequest.Remove(identifying);
            }
            logId = requestType == 2
                ? LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, $"群邀请添加申请", $"来源群: {groupId}({groupName}) 来源人: {fromId}({nick}) 操作: {appendMsg}", "处理中...")
                : LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, $"群添加申请", $"来源: {fromId}({nick}) 目标群: {groupId}({groupName}) 操作: {appendMsg}", "处理中...");
            int ret = ProtocolManager.Instance.CurrentProtocol.SetGroupAddRequest(identifying, requestType, responseType, appendMsg);
            stopwatch.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {stopwatch.ElapsedMilliseconds / (double)1000:f2} s");
            return ret;
        }

        private int CQ_addLog(int authCode, int priority, string type, string msg)
        {
            LogHelper.WriteLog(CurrentPlugin, (LogLevel)priority, type, msg, "");
            return 1;
        }

        private int CQ_setFatal(int authCode, string errorMsg)
        {
            LogHelper.WriteLog(CurrentPlugin, LogLevel.Fatal, "致命错误", errorMsg, "");
            return 1;
        }

        private string CQ_getGroupMemberInfoV2(int authCode, long groupId, long qqId, bool isCache)
        {
            return ProtocolManager.Instance.CurrentProtocol.GetGroupMemberInfo(groupId, qqId, isCache);
        }

        private string CQ_getGroupMemberList(int authCode, long groupId)
        {
            return ProtocolManager.Instance.CurrentProtocol.GetGroupMemberList(groupId);
        }

        private string CQ_getGroupList(int authCode)
        {
            return ProtocolManager.Instance.CurrentProtocol.GetGroupList();
        }

        private string CQ_getStrangerInfo(int authCode, long qqId, bool notCache)
        {
            return ProtocolManager.Instance.CurrentProtocol.GetStrangerInfo(qqId, notCache);
        }

        private string CQ_getGroupInfo(int authCode, long groupId, bool notCache)
        {
            return ProtocolManager.Instance.CurrentProtocol.GetGroupInfo(groupId, notCache);
        }

        private string CQ_getFriendList(int authCode, bool reserved)
        {
            return ProtocolManager.Instance.CurrentProtocol.GetFriendList(reserved);
        }

        private int CQ_canSendImage(int authCode)
        {
            return ProtocolManager.Instance.CurrentProtocol.CanSendImage();
        }

        private int CQ_canSendRecord(int authCode)
        {
            return ProtocolManager.Instance.CurrentProtocol.CanSendRecord();
        }
    }
}