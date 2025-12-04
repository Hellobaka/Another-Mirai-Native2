using Another_Mirai_Native.Config;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using SqlSugar;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Another_Mirai_Native.DB
{
    public static class ChatHistoryHelper
    {
        private static SemaphoreSlim APILock { get; set; } = new(1, 1);

        public static Dictionary<long, FriendInfo> FriendInfoCache { get; set; } = new();

        public static Dictionary<long, GroupInfo> GroupInfoCache { get; set; } = new();

        public static Dictionary<long, Dictionary<long, GroupMemberInfo>> GroupMemberCache { get; set; } = new();

        private static bool Deleteing { get; set; }

        private static string GetDBPath(long id, ChatHistoryType type)
        {
            var path = Path.Combine("logs", "ChatHistory", type.ToString(), id.ToString() + ".db");
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            if (File.Exists(path) is false)
            {
                CreateDB(path);
            }
            return path;
        }

        public static void CreateDB(string path)
        {
            if (File.Exists(path))
            {
                return;
            }
            using var db = GetInstance(path);
            db.DbMaintenance.CreateDatabase();
            db.CodeFirst.InitTables(typeof(ChatHistory));
        }

        public static int InsertHistory(ChatHistory history)
        {
            if (history == null)
            {
                return -1;
            }
            using var db = GetInstance(GetDBPath(history.ParentID, history.Type));
            return db.Insertable(history).ExecuteReturnIdentity();
        }

        public static void UpdateHistoryMessageId(long parentId, ChatHistoryType chatHistoryType, int id, int msgId)
        {
            using var db = GetInstance(GetDBPath(parentId, chatHistoryType));
            db.Updateable<ChatHistory>().Where(x => x.ID == id).SetColumns(x => x.MsgId == msgId).ExecuteCommand();
        }

        public static void UpdateHistory(ChatHistory history)
        {
            if (history == null)
            {
                return;
            }
            using var db = GetInstance(GetDBPath(history.ParentID, history.Type));
            db.Updateable(history).ExecuteCommand();
        }

        public static void UpdateHistoryRecall(long id, int msgId, ChatHistoryType type, bool recalled)
        {
            using var db = GetInstance(GetDBPath(id, type));
            var item = db.Queryable<ChatHistory>().Where(x => x.ParentID == id && x.MsgId == msgId).OrderByDescending(x => x.ID).First();
            if (item == null)
            {
                return;
            }
            item.Recalled = recalled;
            db.Updateable(item).ExecuteCommand();
        }

        public static async Task<List<ChatHistory>> GetHistoriesByPageAsync(long id, ChatHistoryType historyType, int pageSize, int pageIndex)
        {
            using var db = GetInstance(GetDBPath(id, historyType));
            var ls = await db.Queryable<ChatHistory>().OrderByDescending(x => x.Time).ToPageListAsync(pageIndex, pageSize);
            ls.Reverse();
            return ls;
        }

        public static List<ChatHistory> GetHistoriesByPage(long id, ChatHistoryType historyType, int pageSize, int pageIndex)
        {
            using var db = GetInstance(GetDBPath(id, historyType));
            var ls = db.Queryable<ChatHistory>().OrderByDescending(x => x.Time).ToPageList(pageIndex, pageSize);
            ls.Reverse();
            return ls;
        }

        public static List<ChatHistory> GetHistoriesByCount(long groupId, long qq, int count)
        {
            long id = groupId > 0 ? groupId : qq;
            ChatHistoryType historyType = groupId > 0 ? ChatHistoryType.Group : ChatHistoryType.Private;
            using var db = GetInstance(GetDBPath(id, historyType));
            List<ChatHistory> list = db.Queryable<ChatHistory>()
                .WhereIF(qq > 0, x => x.SenderID == groupId)
                .OrderByDescending(x => x.Time).Take(count)
                .ToList();
            return list;
        }

        public static ChatHistory? GetHistoriesByMsgId(long id, int msgId, ChatHistoryType historyType)
        {
            using var db = GetInstance(GetDBPath(id, historyType));
            var item = db.Queryable<ChatHistory>().First(x => x.MsgId == msgId);
            return item;
        }

        public static List<ChatHistory> GetHistoryCategories()
        {
            using var db = GetInstance(GetDBPath(AppConfig.Instance.CurrentQQ, ChatHistoryType.Other));
            return db.Queryable<ChatHistory>().ToList();
        }

        public static void UpdateHistoryCategory(ChatHistory? chatHistory)
        {
            if (chatHistory == null)
            {
                return;
            }
            using var db = GetInstance(GetDBPath(AppConfig.Instance.CurrentQQ, ChatHistoryType.Other));
            var item = db.Queryable<ChatHistory>().Where(x => x.ParentID == chatHistory.ParentID && x.Type == chatHistory.Type).First();
            if (item == null)
            {
                db.Insertable(chatHistory).ExecuteCommand();
            }
            else
            {
                db.Updateable(chatHistory).ReSetValue(x =>
                {
                    x.Time = chatHistory.Time;
                    x.Message = chatHistory.Message;
                }).Where(x => x.ParentID == chatHistory.ParentID && x.Type == chatHistory.Type).ExecuteCommand();
            }
        }

        /// <summary>
        /// 从 <see cref="FriendInfoCache"/> 中获取好友昵称
        /// 若缓存中不存在则调用协议API
        /// </summary>
        /// <param name="qq">好友ID</param>
        /// <returns>昵称, 失败时返回QQ号</returns>
        public static async Task<string> GetFriendNick(long qq)
        {
            try
            {
                await APILock.WaitAsync();
                if (qq == AppConfig.Instance.CurrentQQ)
                {
                    return AppConfig.Instance.CurrentNickName;
                }
                if (FriendInfoCache.TryGetValue(qq, out var info))
                {
                    if (info == null)
                    {
                        return qq.ToString();
                    }
                    return info.Nick;
                }
                else
                {
                    string r = qq.ToString();
                    await Task.Run(() =>
                    {
                        var ls = ProtocolManager.Instance.CurrentProtocol.GetRawFriendList(false);
                        foreach (var item in ls)
                        {
                            item.Nick = item.Nick.Replace("\r", "").Replace("\n", "");
                            item.Postscript = item.Postscript.Replace("\r", "").Replace("\n", "");
                            if (FriendInfoCache.ContainsKey(item.QQ))
                            {
                                FriendInfoCache[item.QQ] = item;
                            }
                            else
                            {
                                FriendInfoCache.Add(item.QQ, item);
                            }
                            if (item.QQ == qq)
                            {
                                r = item.Nick;
                            }
                        }
                    });
                    return r;
                }
            }
            catch
            {
                return qq.ToString();
            }
            finally
            {
                APILock.Release();
            }
        }

        /// <summary>
        /// 从 <see cref="GroupMemberCache"/> 中获取群员名片
        /// 若缓存中不存在则调用协议API
        /// </summary>
        /// <param name="group">群来源</param>
        /// <param name="qq">群员QQ</param>
        /// <returns>群员名片, 若不存在则返回昵称, 若调用失败则返回QQ号</returns>
        public static async Task<string> GetGroupMemberNick(long group, long qq)
        {
            try
            {
                await APILock.WaitAsync();
                if (qq == AppConfig.Instance.CurrentQQ)
                {
                    return AppConfig.Instance.CurrentNickName;
                }
                if (GroupMemberCache.TryGetValue(group, out var dict) && dict.TryGetValue(qq, out var info))
                {
                    if (info == null)
                    {
                        return qq.ToString();
                    }
                    return string.IsNullOrEmpty(info.Card) ? info.Nick : info.Card;
                }
                else
                {
                    if (GroupMemberCache.ContainsKey(group) is false)
                    {
                        GroupMemberCache.Add(group, new Dictionary<long, GroupMemberInfo>());
                    }
                    if (GroupMemberCache[group].ContainsKey(qq) is false)
                    {
                        await Task.Run(() =>
                        {
                            var memberInfo = ProtocolManager.Instance.CurrentProtocol.GetRawGroupMemberInfo(group, qq, false);
                            memberInfo.Card = memberInfo.Card.Replace("\r", "").Replace("\n", "");
                            memberInfo.Nick = memberInfo.Nick.Replace("\r", "").Replace("\n", "");
                            GroupMemberCache[group].Add(qq, memberInfo);
                        });
                    }
                    if (GroupMemberCache[group][qq] == null)
                    {
                        return qq.ToString();
                    }
                    return string.IsNullOrEmpty(GroupMemberCache[group][qq].Card) ? GroupMemberCache[group][qq].Nick : GroupMemberCache[group][qq].Card;
                }
            }
            catch
            {
                return qq.ToString();
            }
            finally
            {
                APILock.Release();
            }
        }

        /// <summary>
        /// 从 <see cref="GroupInfoCache"/> 中获取群名称
        /// 若缓存中不存在则调用协议API
        /// </summary>
        /// <param name="groupId">群号</param>
        /// <returns>群名称, 若不存在则返回群号</returns>
        public static async Task<string> GetGroupName(long groupId)
        {
            try
            {
                await APILock.WaitAsync();
                if (GroupInfoCache.TryGetValue(groupId, out var info))
                {
                    if (info == null)
                    {
                        return groupId.ToString();
                    }
                    return info.Name;
                }
                else
                {
                    string r = groupId.ToString();
                    await Task.Run(() =>
                    {
                        var info = ProtocolManager.Instance.CurrentProtocol.GetRawGroupInfo(groupId, false);
                        info.Name = info.Name.Replace("\n", "").Replace("\r", "");
                        GroupInfoCache.Add(groupId, info);
                        if (GroupInfoCache[groupId] == null)
                        {
                            r = groupId.ToString();
                        }
                        r = GroupInfoCache[groupId]?.Name ?? groupId.ToString();
                    });
                    return r;
                }
            }
            catch
            {
                return groupId.ToString();
            }
            finally
            {
                APILock.Release();
            }
        }

        public static void Initialize()
        {
            if (AppConfig.Instance.EnableChat is false || !AppConfig.Instance.IsCore)
            {
                return;
            }
            PluginManagerProxy.OnGroupBan += PluginManagerProxy_OnGroupBan;
            PluginManagerProxy.OnGroupAdded += PluginManagerProxy_OnGroupAdded;
            PluginManagerProxy.OnGroupMsg += PluginManagerProxy_OnGroupMsg;
            PluginManagerProxy.OnGroupLeft += PluginManagerProxy_OnGroupLeft;
            PluginManagerProxy.OnAdminChanged += PluginManagerProxy_OnAdminChanged;
            PluginManagerProxy.OnFriendAdded += PluginManagerProxy_OnFriendAdded;
            PluginManagerProxy.OnPrivateMsg += PluginManagerProxy_OnPrivateMsg;
            PluginManagerProxy.OnGroupMsgRecall += PluginManagerProxy_OnGroupMsgRecall;
            PluginManagerProxy.OnPrivateMsgRecall += PluginManagerProxy_OnPrivateMsgRecall;
            PluginManagerProxy.OnGroupMemberCardChanged += PluginManagerProxy_OnGroupMemberCardChanged;
            PluginManagerProxy.OnGroupNameChanged += PluginManagerProxy_OnGroupNameChanged;
            PluginManagerProxy.OnFriendNickChanged += PluginManagerProxy_OnFriendNickChanged;

            CQPImplementation.OnPrivateMessageSend += CQPImplementation_OnPrivateMessageSend;
            CQPImplementation.OnGroupMessageSend += CQPImplementation_OnGroupMessageSend;
        }

        private static ChatHistory InsertHistory(long id, long qq, string msg, ChatHistoryType type, DateTime time, bool sending = false, int msgId = 0, CQPluginProxy? plugin = null)
        {
            var history = new ChatHistory
            {
                Message = msg,
                ParentID = id,
                SenderID = qq,
                Type = type,
                MsgId = msgId,
                PluginName = plugin?.PluginName ?? "",
                Time = time,
            };
            history.ID = InsertHistory(history);
            return history;
        }

        private static void PluginManagerProxy_OnAdminChanged(long group, long qq, QQGroupMemberType type)
        {
            if (GroupMemberCache.TryGetValue(group, out var dict) && dict.TryGetValue(qq, out var memberInfo))
            {
                memberInfo.MemberType = type;
            }
        }

        private static async void PluginManagerProxy_OnFriendAdded(long qq)
        {
            FriendInfoCache.Remove(qq);
            await GetFriendNick(qq);
        }

        private static async void PluginManagerProxy_OnGroupAdded(long group, long qq)
        {
            if (GroupMemberCache.TryGetValue(group, out var dict))
            {
                dict.Remove(qq);
            }
            InsertHistory(group, qq, $"{await GetGroupMemberNick(group, qq)} 加入了本群", ChatHistoryType.Notice, DateTime.Now);
        }

        private static async void PluginManagerProxy_OnGroupBan(long group, long qq, long operatedQQ, long time)
        {
            if (GroupMemberCache.TryGetValue(group, out var dict) && dict.ContainsKey(qq))
            {
                InsertHistory(group, qq, $"{await GetGroupMemberNick(group, qq)} 禁言了 {await GetGroupMemberNick(group, operatedQQ)} {time}秒", ChatHistoryType.Notice, DateTime.Now);
            }
            else
            {
                InsertHistory(group, AppConfig.Instance.CurrentQQ, $"{qq} 禁言了 {operatedQQ} {time}秒", ChatHistoryType.Notice, DateTime.Now);
            }
        }

        private static async void PluginManagerProxy_OnGroupLeft(long group, long qq)
        {
            if (GroupMemberCache.TryGetValue(group, out var dict) && dict.ContainsKey(qq))
            {
                dict.Remove(qq);
                InsertHistory(group, AppConfig.Instance.CurrentQQ, $"{await GetGroupMemberNick(group, qq)} 离开了群", ChatHistoryType.Notice, DateTime.Now);
            }
            else
            {
                InsertHistory(group, AppConfig.Instance.CurrentQQ, $"{qq} 离开了群", ChatHistoryType.Notice, DateTime.Now);
            }
        }

        private static void PluginManagerProxy_OnGroupMsg(int msgId, long group, long qq, string msg, DateTime time)
        {
            var history = InsertHistory(group, qq, msg, ChatHistoryType.Group, time, msgId: msgId);
            if (history.Type != ChatHistoryType.Notice)
            {
                UpdateHistoryCategory(history);
                CacheMessageImage(msg);
            }
        }

        private static void PluginManagerProxy_OnGroupMsgRecall(int msgId, long groupId, string msg)
        {
            UpdateHistoryRecall(groupId, msgId, ChatHistoryType.Group, true);
        }

        private static void PluginManagerProxy_OnPrivateMsg(int msgId, long qq, string msg, DateTime time)
        {
            var history = InsertHistory(qq, qq, msg, ChatHistoryType.Private, time, msgId: msgId);
            if (history.Type != ChatHistoryType.Notice)
            {
                UpdateHistoryCategory(history);
                CacheMessageImage(msg);
            }
        }

        private static void PluginManagerProxy_OnPrivateMsgRecall(int msgId, long qq, string msg)
        {
            UpdateHistoryRecall(qq, msgId, ChatHistoryType.Private, true);
        }

        private static void PluginManagerProxy_OnFriendNickChanged(long qq, string nick)
        {
            if (FriendInfoCache.TryGetValue(qq, out var info) && info != null)
            {
                info.Nick = nick;
            }
        }

        private static void PluginManagerProxy_OnGroupNameChanged(long group, string name)
        {
            if (GroupInfoCache.TryGetValue(group, out var info) && info != null)
            {
                info.Name = name;
            }
        }

        private static void PluginManagerProxy_OnGroupMemberCardChanged(long group, long qq, string card)
        {
            if (GroupMemberCache.TryGetValue(group, out var member) && member.TryGetValue(qq, out var info) && info != null)
            {
                info.Card = card;
            }
        }

        /// <summary>
        /// CQP事件_群消息发送
        /// </summary>
        private static void CQPImplementation_OnGroupMessageSend(int msgId, long group, string msg, CQPluginProxy plugin)
        {
            InsertHistory(group, AppConfig.Instance.CurrentQQ, msg, ChatHistoryType.Group, DateTime.Now, msgId: msgId, plugin: plugin);
        }

        private static void CQPImplementation_OnPrivateMessageSend(int msgId, long qq, string msg, CQPluginProxy plugin)
        {
            InsertHistory(qq, AppConfig.Instance.CurrentQQ, msg, ChatHistoryType.Private, DateTime.Now, msgId: msgId, plugin: plugin);
        }

        private static SqlSugarClient GetInstance(string path)
        {
            SqlSugarClient db = new(new ConnectionConfig()
            {
                ConnectionString = $"data source={path}",
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = false,
                InitKeyType = InitKeyType.Attribute,
            });
            return db;
        }

        private static async void CacheMessageImage(string msg)
        {
            if (AppConfig.Instance.EnableChatImageCache is false)
            {
                return;
            }
            Directory.CreateDirectory(Path.Combine("data", "image", "cached"));
            var imgs = CQCode.Parse(msg).Where(x => x.IsImageCQCode);
            if (!imgs.Any())
            {
                return;
            }
            foreach (var item in imgs)
            {
                string url = Helper.GetImageUrlOrPathFromCQCode(item);
                await Helper.DownloadImageAsync(url, item.GetPicName());
            }
            await CheckAndFreeCache();
        }

        private static async Task CheckAndFreeCache()
        {
            if (Deleteing)
            {
                return;
            }
            try
            {
                Deleteing = true;
                await Task.Run(() =>
                {
                    string dir = Path.Combine("data", "image", "cached");
                    double length = 0;
                    // 统计缓存文件夹总大小
                    List<FileInfo> files = [];
                    foreach (var item in Directory.GetFiles(dir, "*.*", SearchOption.TopDirectoryOnly))
                    {
                        var info = new FileInfo(item);
                        length += info.Length;
                        files.Add(info);
                    }
                    double maxSize = AppConfig.Instance.MaxChatImageCacheFolderSize * 1024 * 1024;
                    if (length > maxSize)
                    {
                        // 按修改时间从旧到新排序
                        files = files.OrderBy(x => x.LastWriteTime).ToList();
                        // 循环删除第一个
                        do
                        {
                            var file = files.FirstOrDefault();
                            if (file != null)
                            {
                                length -= file.Length;

                                try
                                {
                                    file.Delete();
                                }
                                catch
                                { }
                                files.Remove(file);
                            }
                            else
                            {
                                break;
                            }
                        } while (length > maxSize);
                    }
                });                
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog($"缓存图片释放失败：{ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                Deleteing = false;
            }
        }
    }
}