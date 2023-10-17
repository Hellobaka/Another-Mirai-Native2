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
                    LogHelper.Error("CQPImplementation.CheckPluginCanInvoke", $"调用 {functionName} 未找到对应实现");
                    return null;
                }
                if (!CurrentPlugin.CheckPluginCanInvoke(functionName))
                {
                    LogHelper.Error("CQPImplementation.CheckPluginCanInvoke", $"调用 {functionName} 未定义权限");
                    return null;
                }
                var argumentList = methodInfo.GetParameters();
                if (args.Length != argumentList.Length)
                {
                    LogHelper.Error("CQPImplementation.CheckPluginCanInvoke", $"调用 {functionName} 参数表数量不对应");
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
                LogHelper.Info("CQPImplementation.CheckPluginCanInvoke", $"调用 {functionName}, 参数: {string.Join(",", transformedArgs)}");
                object result = methodInfo.Invoke(this, transformedArgs);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error("CQPImplementation.CheckPluginCanInvoke", ex);
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
            return ProtocolManager.Instance.CurrentProtocol.SendPrivateMessage(qqId, msg);
        }

        private int CQ_sendGroupMsg(int authCode, long groupId, string msg)
        {
            return ProtocolManager.Instance.CurrentProtocol.SendGroupMessage(groupId, msg);
        }

        private int CQ_sendGroupQuoteMsg(int authCode, long groupId, int msgId, string msg)
        {
            return ProtocolManager.Instance.CurrentProtocol.SendGroupMessage(groupId, msg, msgId);
        }

        private int CQ_sendDiscussMsg(int authCode, long discussId, string msg)
        {
            return ProtocolManager.Instance.CurrentProtocol.SendDiscussMsg(discussId, msg);
        }

        private int CQ_deleteMsg(int authCode, long msgId)
        {
            return ProtocolManager.Instance.CurrentProtocol.DeleteMsg(msgId);
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
            return ProtocolManager.Instance.CurrentProtocol.SetGroupKick(groupId, qqId, refuses);
        }

        private int CQ_setGroupBan(int authCode, long groupId, long qqId, long time)
        {
            return ProtocolManager.Instance.CurrentProtocol.SetGroupBan(groupId, qqId, time);
        }

        private int CQ_setGroupAdmin(int authCode, long groupId, long qqId, bool isSet)
        {
            return ProtocolManager.Instance.CurrentProtocol.SetGroupAdmin(groupId, qqId, isSet);
        }

        private int CQ_setGroupSpecialTitle(int authCode, long groupId, long qqId, string title, long durationTime)
        {
            return ProtocolManager.Instance.CurrentProtocol.SetGroupSpecialTitle(groupId, qqId, title, durationTime);
        }

        private int CQ_setGroupWholeBan(int authCode, long groupId, bool isOpen)
        {
            return ProtocolManager.Instance.CurrentProtocol.SetGroupWholeBan(groupId, isOpen);
        }

        private int CQ_setGroupAnonymousBan(int authCode, long groupId, string anonymous, long banTime)
        {
            return ProtocolManager.Instance.CurrentProtocol.SetGroupAnonymousBan(groupId, anonymous, banTime);
        }

        private int CQ_setGroupAnonymous(int authCode, long groupId, bool isOpen)
        {
            return ProtocolManager.Instance.CurrentProtocol.SetGroupAnonymous(groupId, isOpen);
        }

        private int CQ_setGroupCard(int authCode, long groupId, long qqId, string newCard)
        {
            return ProtocolManager.Instance.CurrentProtocol.SetGroupCard(groupId, qqId, newCard);
        }

        private int CQ_setGroupLeave(int authCode, long groupId, bool isDisband)
        {
            return ProtocolManager.Instance.CurrentProtocol.SetGroupLeave(groupId, isDisband);
        }

        private int CQ_setDiscussLeave(int authCode, long discussId)
        {
            return ProtocolManager.Instance.CurrentProtocol.SetDiscussLeave(discussId);
        }

        private int CQ_setFriendAddRequest(int authCode, string identifying, int requestType, string appendMsg)
        {
            return ProtocolManager.Instance.CurrentProtocol.SetFriendAddRequest(Convert.ToInt64(identifying), requestType, appendMsg);
        }

        private int CQ_setGroupAddRequestV2(int authCode, string identifying, int requestType, int responseType, string appendMsg)
        {
            return ProtocolManager.Instance.CurrentProtocol.SetGroupAddRequest(Convert.ToInt64(identifying), requestType, responseType, appendMsg);
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