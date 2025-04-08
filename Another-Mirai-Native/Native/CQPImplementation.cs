using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model.Enums;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Another_Mirai_Native.Native
{
    public class CQPImplementation
    {
        public CQPluginProxy CurrentPlugin { get; set; }

        public Stopwatch Stopwatch { get; set; } = new();

        public bool Testing => CurrentPlugin?.AppInfo.AuthCode == AppConfig.Instance.TestingAuthCode;

        /// <summary>
        /// msgId, groupId, msg
        /// </summary>
        public static event Action<int, long, string, CQPluginProxy> OnGroupMessageSend;

        /// <summary>
        /// msgId, qq, msg
        /// </summary>
        public static event Action<int, long, string, CQPluginProxy> OnPrivateMessageSend;

        public CQPImplementation(CQPluginProxy plugin)
        {
            if (plugin == null)
            {
                throw new ArgumentNullException(nameof(plugin));
            }
            CurrentPlugin = plugin;
        }

        public object? Invoke(string functionName, params object[] args)
        {
            Stopwatch.Restart();
            int logId = -1;
            try
            {
                var methodInfo = typeof(CQPImplementation).GetMethod(functionName, BindingFlags.NonPublic | BindingFlags.Instance);
                if (methodInfo == null)
                {
                    LogHelper.Error("调用前置检查", $"{CurrentPlugin.PluginName} => 调用 {functionName} 未找到对应实现");
                    return null;
                }
                if (!CurrentPlugin.CheckPluginCanInvoke(functionName))
                {
                    LogHelper.Error("调用前置检查", $"{CurrentPlugin.PluginName} => 调用 {functionName} 未定义权限");
                    return null;
                }
                var argumentList = methodInfo.GetParameters();
                if (args.Length != argumentList.Length)
                {
                    LogHelper.Error("调用前置检查", $"{CurrentPlugin.PluginName} => 调用 {functionName} 参数表数量不对应");
                    return null;
                }
                object?[] transformedArgs = new object[argumentList.Length];
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
                LogHelper.Debug("调用前置检查", $"{CurrentPlugin.PluginName} => 调用 {functionName}, 参数: {string.Join(",", transformedArgs)}");
                object? result = methodInfo.Invoke(this, transformedArgs);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error("调用前置检查", $"{CurrentPlugin.PluginName} => {ex.Message}\n{ex.StackTrace}");
                return null;
            }
            finally
            {
                Stopwatch.Stop();
                if (logId > 0)
                {
                    LogHelper.UpdateLogStatus(logId, $"√ {Stopwatch.ElapsedMilliseconds / 1000:f2}s");
                }
            }
        }

        /// <summary>
        /// 发送好友消息
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId">目标帐号</param>
        /// <param name="msg">消息内容</param>
        /// <returns></returns>
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
            OnPrivateMessageSend?.Invoke(ret, qqId, msg, CurrentPlugin);
            stopwatch.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {stopwatch.ElapsedMilliseconds / (double)1000:f2} s");
            return ret;
        }

        /// <summary>
        /// 发送群消息
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId">目标群</param>
        /// <param name="msg">消息内容</param>
        /// <returns></returns>
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
            OnGroupMessageSend?.Invoke(ret, groupId, msg, CurrentPlugin);
            stopwatch.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {stopwatch.ElapsedMilliseconds / (double)1000:f2} s");
            return ret;
        }

        /// <summary>
        /// 发送群消息
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId">目标群</param>
        /// <param name="msgId"></param>
        /// <param name="msg">消息内容</param>
        /// <returns></returns>
        private int CQ_sendGroupQuoteMsg(int authCode, long groupId, int msgId, string msg)
        {
            if (Testing)
            {
                PluginManagerProxy.TriggerTestInvoke("CQ_sendGroupQuoteMsg", new() { { "groupId", groupId }, { "msgId", msgId }, { "msg", msg } });
                return 1;
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
            int logId = LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, "[↑]发送群引用消息", $"群:{groupId} 消息:{msg}", "处理中...");
            int ret = ProtocolManager.Instance.CurrentProtocol.SendGroupMessage(groupId, msg, msgId);
            OnGroupMessageSend?.Invoke(ret, groupId, msg, CurrentPlugin);
            stopwatch.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {stopwatch.ElapsedMilliseconds / (double)1000:f2} s");
            return ret;
        }

        /// <summary>
        /// 发送讨论组消息
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="discussId">目标讨论组</param>
        /// <param name="msg">消息内容</param>
        /// <returns></returns>
        private int CQ_sendDiscussMsg(int authCode, long discussId, string msg)
        {
            if (Testing)
            {
                PluginManagerProxy.TriggerTestInvoke("CQ_sendDiscussMsg", new() { { "discussId", discussId }, { "msg", msg } });
                return 1;
            }
            return ProtocolManager.Instance.CurrentProtocol.SendDiscussMsg(discussId, msg);
        }

        /// <summary>
        /// 撤回消息
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="msgId">消息ID</param>
        /// <returns></returns>
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

        /// <summary>
        /// 发送赞
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="qqId">目标帐号</param>
        /// <param name="count">赞的次数，最多10次</param>
        /// <returns></returns>
        private int CQ_sendLikeV2(int authCode, long qqId, int count)
        {
            return ProtocolManager.Instance.CurrentProtocol.SendLike(qqId, count);
        }

        /// <summary>
        /// 取Cookies(慎用,此接口需要严格授权)
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="domain">目标域名，如 api.example.com</param>
        /// <returns></returns>
        private string CQ_getCookiesV2(int authCode, string domain)
        {
            return ProtocolManager.Instance.CurrentProtocol.GetCookies(domain);
        }

        /// <summary>
        /// 接收图片，并返回图片文件绝对路径
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="file">收到消息中的图片文件名(file)</param>
        /// <returns>下载成功时，返回绝对路径；下载失败时，返回空字符串</returns>
        private string CQ_getImage(int authCode, string file)
        {
            // 检查图片是否是已缓存状态
            string imgDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\image");
            // 检索图片文件夹下文件名为{file}的所有文件
            var arr = Directory.GetFiles(imgDir, $"{Path.GetFileNameWithoutExtension(file)}.*");
            if (arr.Length > 0)
            {
                // 缓存文件存在
                var downloaded = arr.FirstOrDefault(x => Path.GetExtension(x) != ".cqimg");
                if (string.IsNullOrEmpty(downloaded))
                {
                    // 未下载，只有cqimg文件，从中取url并下载
                    string url = Helper.GetPicUrlFromCQImg(file);
                    string imgFileName = file.Contains('.') ? file : Path.ChangeExtension(file, ".jpg");
                    var downloadTask = Helper.DownloadFile(url, imgFileName, imgDir);
                    downloadTask.Wait();
                    if (downloadTask.Result.success is false)
                    {
                        LogHelper.Error("图片下载", $"{file} 下载任务失败");
                        return string.Empty;
                    }
                    return downloadTask.Result.fullPath;
                }
                else
                {
                    // 缓存文件存在，直接返回
                    return downloaded;
                }
            }
            else
            {
                LogHelper.Error("图片下载", $"{file} 缓存文件不存在");
                return string.Empty;
            }
        }

        /// <summary>
        /// 接收语音，并返回语音文件绝对路径
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="file">收到消息中的语音文件名(file)</param>
        /// <param name="format">应用所需的格式</param>
        /// <returns></returns>
        private string CQ_getRecordV2(int authCode, string file, string format)
        {
            // 将不会实现
            return "";
        }

        /// <summary>
        /// 取CsrfToken(慎用,此接口需要严格授权)
        /// </summary>
        /// <param name="authCode"></param>
        /// <returns></returns>
        private string CQ_getCsrfToken(int authCode)
        {
            return ProtocolManager.Instance.CurrentProtocol.GetCsrfToken();
        }

        /// <summary>
        /// 取应用目录
        /// </summary>
        /// <param name="authCode"></param>
        /// <returns></returns>
        private string CQ_getAppDirectory(int authCode)
        {
            string appId = CurrentPlugin.PluginId;
            string path = $@"data\app\{appId}";
            Directory.CreateDirectory(path);
            return new DirectoryInfo(path).FullName + "\\";
        }

        /// <summary>
        /// 取登录帐号
        /// </summary>
        /// <param name="authCode"></param>
        /// <returns></returns>
        private long CQ_getLoginQQ(int authCode)
        {
            return ProtocolManager.Instance.CurrentProtocol.GetLoginQQ();
        }

        /// <summary>
        /// 取登录昵称
        /// </summary>
        /// <param name="authCode"></param>
        /// <returns></returns>
        private string CQ_getLoginNick(int authCode)
        {
            return ProtocolManager.Instance.CurrentProtocol.GetLoginNick();
        }

        /// <summary>
        /// 置群员移除
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId">目标群</param>
        /// <param name="qqId">目标帐号</param>
        /// <param name="refuses">如果为真，则“不再接收此人加群申请”，请慎用</param>
        /// <returns></returns>
        private int CQ_setGroupKick(int authCode, long groupId, long qqId, bool refuses)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            int logId = LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, "踢出群成员", $"移除群{groupId} 成员{qqId}", "处理中...");
            int ret = ProtocolManager.Instance.CurrentProtocol.SetGroupKick(groupId, qqId, refuses);
            stopwatch.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {stopwatch.ElapsedMilliseconds / (double)1000:f2} s");
            return ret;
        }

        /// <summary>
        /// 置群员禁言
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId">目标群</param>
        /// <param name="qqId">目标帐号</param>
        /// <param name="time">禁言的时间，单位为秒。如果要解禁，这里填写0</param>
        /// <returns></returns>
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

        /// <summary>
        /// 置群管理员
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId">目标群</param>
        /// <param name="qqId">被设置的帐号</param>
        /// <param name="isSet">真/设置管理员 假/取消管理员</param>
        /// <returns></returns>
        private int CQ_setGroupAdmin(int authCode, long groupId, long qqId, bool isSet)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            int logId = LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, $"{(isSet ? "设置" : "取消")}群成员管理", $"{(isSet ? "设置" : "取消")}群{groupId} 成员{qqId}", "处理中...");
            int ret = ProtocolManager.Instance.CurrentProtocol.SetGroupAdmin(groupId, qqId, isSet);
            stopwatch.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {stopwatch.ElapsedMilliseconds / (double)1000:f2} s");
            return ret;
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
        private int CQ_setGroupSpecialTitle(int authCode, long groupId, long qqId, string title, long durationTime)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            int logId = LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, "设置群成员头衔", $"设置群{groupId} 成员{qqId} 头衔 {title}", "处理中...");
            int ret = ProtocolManager.Instance.CurrentProtocol.SetGroupSpecialTitle(groupId, qqId, title, durationTime);
            stopwatch.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {stopwatch.ElapsedMilliseconds / (double)1000:f2} s");
            return ret;
        }

        /// <summary>
        /// 置全群禁言
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId">目标群</param>
        /// <param name="isOpen">真/开启 假/关闭</param>
        /// <returns></returns>
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

        /// <summary>
        /// 置匿名群员禁言
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId">目标群</param>
        /// <param name="anonymous">群消息事件收到的“匿名”参数</param>
        /// <param name="banTime">禁言的时间，单位为秒。不支持解禁</param>
        /// <returns></returns>
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

        /// <summary>
        /// 置群匿名设置
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId"></param>
        /// <param name="isOpen"></param>
        /// <returns></returns>
        private int CQ_setGroupAnonymous(int authCode, long groupId, bool isOpen)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            int logId = LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, "设置群匿名状态", $"群{groupId} {(isOpen ? "开启" : "关闭")}", "处理中...");
            int ret = ProtocolManager.Instance.CurrentProtocol.SetGroupAnonymous(groupId, isOpen);
            stopwatch.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {stopwatch.ElapsedMilliseconds / (double)1000:f2} s");
            return ret;
        }

        /// <summary>
        /// 置群成员名片
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId">目标群</param>
        /// <param name="qqId">被设置的帐号</param>
        /// <param name="newCard"></param>
        /// <returns></returns>
        private int CQ_setGroupCard(int authCode, long groupId, long qqId, string newCard)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            int logId = LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, "设置群成员名片", $"设置群{groupId} 成员{qqId} 名片 {newCard}", "处理中...");
            int ret = ProtocolManager.Instance.CurrentProtocol.SetGroupCard(groupId, qqId, newCard);
            stopwatch.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {stopwatch.ElapsedMilliseconds / (double)1000:f2} s");
            return ret;
        }

        /// <summary>
        /// 置群退出
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId">目标群</param>
        /// <param name="isDisband">真/解散本群(群主) 假/退出本群(管理、群成员)</param>
        /// <returns></returns>
        private int CQ_setGroupLeave(int authCode, long groupId, bool isDisband)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            int logId = LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, "退出群", $"退出群{groupId}", "处理中...");
            int ret = ProtocolManager.Instance.CurrentProtocol.SetGroupLeave(groupId, isDisband);
            stopwatch.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {stopwatch.ElapsedMilliseconds / (double)1000:f2} s");
            return ret;
        }

        /// <summary>
        /// 置讨论组退出
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="discussId">目标讨论组</param>
        /// <returns></returns>
        private int CQ_setDiscussLeave(int authCode, long discussId)
        {
            return ProtocolManager.Instance.CurrentProtocol.SetDiscussLeave(discussId);
        }

        /// <summary>
        /// 置好友添加请求
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="identifying">请求事件收到的“反馈标识”参数</param>
        /// <param name="requestType">1通过 2拒绝</param>
        /// <param name="appendMsg">添加后的好友备注</param>
        /// <returns></returns>
        private int CQ_setFriendAddRequest(int authCode, string identifying, int requestType, string appendMsg)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            long fromId = 0; string nick = "";
            if (CurrentPlugin.PluginType == PluginType.XiaoLiZi)
            {
                if (!RequestCache.CachedStrings.TryGetValue(identifying, out identifying!))
                {
                    LogHelper.Error("处理好友添加请求", $"Seq={identifying} 没有被缓存，框架可能未正确处理事件");
                    return 1;
                }
            }
            if (RequestCache.FriendRequest.TryGetValue(identifying, out (long, string) value))
            {
                fromId = value.Item1;
                nick = value.Item2;
            }
            int logId = LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, $"好友添加申请", $"来源: {fromId}({nick}) 操作: {(requestType == 0 ? "同意" : "拒绝")}", "处理中...");
            int ret = ProtocolManager.Instance.CurrentProtocol.SetFriendAddRequest(identifying, requestType, appendMsg);
            stopwatch.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {stopwatch.ElapsedMilliseconds / (double)1000:f2} s");
            return ret;
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
        private int CQ_setGroupAddRequestV2(int authCode, string identifying, int requestType, int responseType, string appendMsg)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            int logId;
            long fromId = 0, groupId = 0;
            string nick = "", groupName = "";
            if (CurrentPlugin.PluginType == PluginType.XiaoLiZi)
            {
                if (RequestCache.CachedStrings.TryGetValue(identifying, out string? seq))
                {
                    identifying = seq;
                }
                else
                {
                    LogHelper.Error("处理群添加请求", $"Seq={identifying} 没有被缓存，框架可能未正确处理事件");
                }
            }
            if (RequestCache.GroupRequest.TryGetValue(identifying, out (long, string, long, string) value))
            {
                fromId = value.Item1;
                nick = value.Item2;
                groupId = value.Item3;
                groupName = value.Item4;
            }
            logId = requestType == 2
                ? LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, $"群邀请添加申请", $"来源群: {groupId}({groupName}) 来源人: {fromId}({nick}) 操作: {(responseType == 1 ? "同意" : "拒绝")}", "处理中...")
                : LogHelper.WriteLog(CurrentPlugin, LogLevel.InfoSend, $"群添加申请", $"来源: {fromId}({nick}) 目标群: {groupId}({groupName}) 操作: {(responseType == 1 ? "同意" : "拒绝")}", "处理中...");
            int ret = ProtocolManager.Instance.CurrentProtocol.SetGroupAddRequest(identifying, requestType, responseType, appendMsg);
            stopwatch.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {stopwatch.ElapsedMilliseconds / (double)1000:f2} s");
            return ret;
        }

        /// <summary>
        /// 增加运行日志
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="priority"></param>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private int CQ_addLog(int authCode, int priority, string type, string msg)
        {
            LogHelper.WriteLog(CurrentPlugin, (LogLevel)priority, type, msg, "");
            return 0;
        }

        /// <summary>
        /// 置致命错误提示
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private int CQ_setFatal(int authCode, string errorMsg)
        {
            LogHelper.WriteLog(CurrentPlugin, LogLevel.Fatal, "致命错误", errorMsg, "");
            return 0;
        }

        /// <summary>
        /// 取群成员信息(支持缓存)
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId">目标帐号所在群</param>
        /// <param name="qqId">目标帐号</param>
        /// <param name="isCache"></param>
        /// <returns></returns>
        private string CQ_getGroupMemberInfoV2(int authCode, long groupId, long qqId, bool isCache)
        {
            return ProtocolManager.Instance.CurrentProtocol.GetGroupMemberInfo(groupId, qqId, isCache);
        }

        /// <summary>
        /// 取群成员列表
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId">目标帐号所在群</param>
        /// <returns></returns>
        private string CQ_getGroupMemberList(int authCode, long groupId)
        {
            return ProtocolManager.Instance.CurrentProtocol.GetGroupMemberList(groupId);
        }

        /// <summary>
        /// 取群列表
        /// </summary>
        /// <param name="authCode"></param>
        /// <returns></returns>
        private string CQ_getGroupList(int authCode)
        {
            return ProtocolManager.Instance.CurrentProtocol.GetGroupList();
        }

        /// <summary>
        /// 取陌生人信息(支持缓存)
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="qqId">目标帐号</param>
        /// <param name="notCache"></param>
        /// <returns></returns>
        private string CQ_getStrangerInfo(int authCode, long qqId, bool notCache)
        {
            return ProtocolManager.Instance.CurrentProtocol.GetStrangerInfo(qqId, notCache);
        }

        /// <summary>
        /// 取群信息(支持缓存)
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="groupId">目标群</param>
        /// <param name="notCache"></param>
        /// <returns></returns>
        private string CQ_getGroupInfo(int authCode, long groupId, bool notCache)
        {
            return ProtocolManager.Instance.CurrentProtocol.GetGroupInfo(groupId, notCache);
        }

        /// <summary>
        /// 取好友列表
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="reserved"></param>
        /// <returns></returns>
        private string CQ_getFriendList(int authCode, bool reserved)
        {
            return ProtocolManager.Instance.CurrentProtocol.GetFriendList(reserved);
        }

        /// <summary>
        /// 是否支持发送图片
        /// </summary>
        /// <param name="authCode"></param>
        /// <returns>返回大于 0 为支持，等于 0 为不支持</returns>
        private int CQ_canSendImage(int authCode)
        {
            return ProtocolManager.Instance.CurrentProtocol.CanSendImage();
        }

        /// <summary>
        /// 是否支持发送语音
        /// </summary>
        /// <param name="authCode"></param>
        /// <returns>返回大于 0 为支持，等于 0 为不支持</returns>
        private int CQ_canSendRecord(int authCode)
        {
            return ProtocolManager.Instance.CurrentProtocol.CanSendRecord();
        }
    }
}