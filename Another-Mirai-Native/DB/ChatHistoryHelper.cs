using Another_Mirai_Native.Config;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using SqlSugar;
using System.Collections.Concurrent;

namespace Another_Mirai_Native.DB
{
    public static class ChatHistoryHelper
    {
        /// <summary>
        /// 好友信息列表缓存
        /// </summary>
        public static ConcurrentDictionary<long, FriendInfo> FriendInfoCache { get; set; } = new();

        /// <summary>
        /// 群信息列表缓存
        /// </summary>
        public static ConcurrentDictionary<long, GroupInfo> GroupInfoCache { get; set; } = new();

        /// <summary>
        /// 群成员信息列表缓存
        /// </summary>
        public static ConcurrentDictionary<long, Dictionary<long, GroupMemberInfo>> GroupMemberCache { get; set; } = new();

        private static bool Deleteing { get; set; }

        private static System.Timers.Timer DailyMaintenanceTimer { get; set; }

        /// <summary>
        /// 从数据库加载缓存数据到内存
        /// </summary>
        public static async Task LoadCacheFromDatabaseAsync()
        {
            try
            {
                var db = ChatHistoryDB.GetInstance();

                // 加载好友缓存
                var friends = await db.Queryable<FriendEntity>().ToListAsync();
                foreach (var friend in friends)
                {
                    FriendInfoCache[friend.QQ] = new FriendInfo
                    {
                        QQ = friend.QQ,
                        Nick = friend.Nick,
                        Postscript = friend.Postscript
                    };
                }

                // 加载群缓存
                var groups = await db.Queryable<GroupEntity>().ToListAsync();
                foreach (var group in groups)
                {
                    GroupInfoCache[group.GroupID] = new GroupInfo
                    {
                        Group = group.GroupID,
                        Name = group.Name,
                        CurrentMemberCount = group.CurrentMemberCount,
                        MaxMemberCount = group.MaxMemberCount
                    };
                }

                // 加载群成员缓存
                var groupMembers = await db.Queryable<GroupMemberEntity>().ToListAsync();
                foreach (var member in groupMembers)
                {
                    if (!GroupMemberCache.TryGetValue(member.GroupID, out Dictionary<long, GroupMemberInfo>? value))
                    {
                        value = [];
                        GroupMemberCache[member.GroupID] = value;
                    }

                    value[member.QQ] = new GroupMemberInfo
                    {
                        Group = member.GroupID,
                        QQ = member.QQ,
                        Nick = member.Nick,
                        Card = member.Card,
                        MemberType = member.MemberType,
                        Sex = member.Sex,
                        Age = member.Age,
                        Area = member.Area,
                        JoinGroupDateTime = Helper.TimeStamp2DateTime(member.JoinGroupTime),
                        LastSpeakDateTime = Helper.TimeStamp2DateTime(member.LastSpeakTime),
                        Level = member.Level,
                        ExclusiveTitle = member.ExclusiveTitle,
                        ExclusiveTitleExpirationTime = member.ExclusiveTitleExpirationTime > 0
                            ? Helper.TimeStamp2DateTime(member.ExclusiveTitleExpirationTime) : null,
                        IsBadRecord = member.IsBadRecord,
                        IsAllowEditorCard = member.IsAllowEditorCard
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("聊天记录管理", $"缓存加载失败: {ex}");
            }
        }

        /// <summary>
        /// 保存好友信息到数据库
        /// </summary>
        private static async Task SaveFriendToDBAsync(FriendInfo friend)
        {
            if (friend == null)
            {
                return;
            }
            try
            {
                var db = ChatHistoryDB.GetInstance();
                var entity = new FriendEntity
                {
                    QQ = friend.QQ,
                    Nick = friend.Nick,
                    Postscript = friend.Postscript,
                    LastUpdateTime = Helper.TimeStamp
                };

                var existing = await db.Queryable<FriendEntity>()
                    .Where(x => x.QQ == friend.QQ)
                    .FirstAsync();

                if (existing == null)
                {
                    await db.Insertable(entity).ExecuteCommandAsync();
                }
                else
                {
                    entity.ID = existing.ID;
                    await db.Updateable(entity).ExecuteCommandAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("聊天记录管理", $"保存好友信息失败: {ex}");
            }
        }

        /// <summary>
        /// 保存群信息到数据库
        /// </summary>
        private static async Task SaveGroupToDBAsync(GroupInfo group)
        {
            if (group == null)
            {
                return;
            }
            try
            {
                var db = ChatHistoryDB.GetInstance();
                var entity = new GroupEntity
                {
                    GroupID = group.Group,
                    Name = group.Name,
                    CurrentMemberCount = group.CurrentMemberCount,
                    MaxMemberCount = group.MaxMemberCount,
                    LastUpdateTime = Helper.TimeStamp
                };

                var existing = await db.Queryable<GroupEntity>()
                    .Where(x => x.GroupID == group.Group)
                    .FirstAsync();

                if (existing == null)
                {
                    await db.Insertable(entity).ExecuteCommandAsync();
                }
                else
                {
                    entity.ID = existing.ID;
                    await db.Updateable(entity).ExecuteCommandAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("聊天记录管理", $"保存群信息失败: {ex}");
            }
        }

        /// <summary>
        /// 保存群成员信息到数据库
        /// </summary>
        private static async Task SaveGroupMemberToDBAsync(GroupMemberInfo member)
        {
            if (member == null)
            {
                return;
            }
            try
            {
                var db = ChatHistoryDB.GetInstance();
                var entity = new GroupMemberEntity
                {
                    GroupID = member.Group,
                    QQ = member.QQ,
                    Nick = member.Nick,
                    Card = member.Card,
                    MemberType = member.MemberType,
                    Sex = member.Sex,
                    Age = member.Age,
                    Area = member.Area,
                    JoinGroupTime = member.JoinGroupDateTime.ToTimeStamp(),
                    LastSpeakTime = member.LastSpeakDateTime.ToTimeStamp(),
                    Level = member.Level,
                    ExclusiveTitle = member.ExclusiveTitle,
                    ExclusiveTitleExpirationTime = member.ExclusiveTitleExpirationTime.ToTimeStamp(),
                    IsBadRecord = member.IsBadRecord,
                    IsAllowEditorCard = member.IsAllowEditorCard,
                    LastUpdateTime = Helper.TimeStamp
                };

                var existing = await db.Queryable<GroupMemberEntity>()
                    .Where(x => x.GroupID == member.Group && x.QQ == member.QQ)
                    .FirstAsync();

                if (existing == null)
                {
                    await db.Insertable(entity).ExecuteCommandAsync();
                }
                else
                {
                    entity.ID = existing.ID;
                    await db.Updateable(entity).ExecuteCommandAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("聊天记录管理", $"保存群成员信息失败: {ex}");
            }
        }

        public static int InsertHistory(ChatHistory history)
        {
            if (history == null)
            {
                return -1;
            }

            try
            {
                var db = ChatHistoryDB.GetInstance();
                var entity = new ChatHistoryEntity
                {
                    Time = history.Time.ToTimeStamp(),
                    Type = history.Type,
                    ParentID = history.ParentID,
                    SenderID = history.SenderID,
                    Message = history.Message,
                    MsgId = history.MsgId,
                    Recalled = history.Recalled,
                    PluginName = history.PluginName
                };

                return db.Insertable(entity).ExecuteReturnIdentity();
            }
            catch (Exception ex)
            {
                LogHelper.Error("聊天记录管理", $"插入聊天记录失败: {ex}");
                return -1;
            }
        }

        public static void UpdateHistoryMessageId(long parentId, int id, int msgId)
        {
            try
            {
                var db = ChatHistoryDB.GetInstance();
                db.Updateable<ChatHistoryEntity>()
                    .SetColumns(x => x.MsgId == msgId)
                    .Where(x => x.ID == id && x.ParentID == parentId)
                    .ExecuteCommand();
            }
            catch (Exception ex)
            {
                LogHelper.Error("聊天记录管理", $"更新消息ID失败: {ex}");
            }
        }

        public static void UpdateHistory(ChatHistory history)
        {
            if (history == null)
            {
                return;
            }

            try
            {
                var db = ChatHistoryDB.GetInstance();
                var entity = new ChatHistoryEntity
                {
                    ID = history.ID,
                    Time = history.Time.ToTimeStamp(),
                    Type = history.Type,
                    ParentID = history.ParentID,
                    SenderID = history.SenderID,
                    Message = history.Message,
                    MsgId = history.MsgId,
                    Recalled = history.Recalled,
                    PluginName = history.PluginName
                };

                db.Updateable(entity).ExecuteCommand();
            }
            catch (Exception ex)
            {
                LogHelper.Error("聊天记录管理", $"更新聊天记录失败: {ex}");
            }
        }

        public static void UpdateHistoryRecall(long id, int msgId, ChatHistoryType type, bool recalled)
        {
            try
            {
                var db = ChatHistoryDB.GetInstance();
                var item = db.Queryable<ChatHistoryEntity>()
                    .Where(x => x.ParentID == id && x.MsgId == msgId && x.Type == type)
                    .OrderByDescending(x => x.ID)
                    .First();

                if (item == null)
                {
                    return;
                }

                db.Updateable<ChatHistoryEntity>()
                    .SetColumns(x => x.Recalled == recalled)
                    .Where(x => x.ID == item.ID)
                    .ExecuteCommand();
            }
            catch (Exception ex)
            {
                LogHelper.Error("聊天记录管理", $"更新撤回状态失败: {ex.Message}");
            }
        }

        public static async Task<List<ChatHistory>> GetHistoriesByPageAsync(long id, ChatHistoryType historyType, int pageSize, int pageIndex)
        {
            try
            {
                var db = ChatHistoryDB.GetInstance();
                var entities = await db.Queryable<ChatHistoryEntity>()
                    .Where(x => x.ParentID == id && x.Type == historyType)
                    .OrderByDescending(x => x.Time)
                    .ToPageListAsync(pageIndex, pageSize);

                var result = entities.Select(e => new ChatHistory
                {
                    ID = (int)e.ID,
                    Time = Helper.TimeStamp2DateTime(e.Time),
                    Type = e.Type,
                    ParentID = e.ParentID,
                    SenderID = e.SenderID,
                    Message = e.Message,
                    MsgId = e.MsgId,
                    Recalled = e.Recalled,
                    PluginName = e.PluginName
                }).ToList();

                result.Reverse();
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error("聊天记录管理", $"分页查询失败: {ex.Message}");
                return [];
            }
        }

        public static List<ChatHistory> GetHistoriesByPage(long id, ChatHistoryType historyType, int pageSize, int pageIndex)
        {
            try
            {
                var db = ChatHistoryDB.GetInstance();
                var entities = db.Queryable<ChatHistoryEntity>()
                    .Where(x => x.ParentID == id && x.Type == historyType)
                    .OrderByDescending(x => x.Time)
                    .ToPageList(pageIndex, pageSize);

                var result = entities.Select(e => new ChatHistory
                {
                    ID = (int)e.ID,
                    Time = Helper.TimeStamp2DateTime(e.Time),
                    Type = e.Type,
                    ParentID = e.ParentID,
                    SenderID = e.SenderID,
                    Message = e.Message,
                    MsgId = e.MsgId,
                    Recalled = e.Recalled,
                    PluginName = e.PluginName
                }).ToList();

                result.Reverse();
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error("聊天记录管理", $"分页查询失败: {ex.Message}");
                return [];
            }
        }

        public static List<ChatHistory> GetHistoriesByCount(long groupId, long qq, int count)
        {
            try
            {
                long id = groupId > 0 ? groupId : qq;
                ChatHistoryType historyType = groupId > 0 ? ChatHistoryType.Group : ChatHistoryType.Private;

                var db = ChatHistoryDB.GetInstance();
                var entities = db.Queryable<ChatHistoryEntity>()
                    .Where(x => x.ParentID == id && x.Type == historyType)
                    .WhereIF(qq > 0 && groupId > 0, x => x.SenderID == qq)
                    .OrderByDescending(x => x.Time)
                    .Take(count)
                    .ToList();

                return entities.Select(e => new ChatHistory
                {
                    ID = (int)e.ID,
                    Time = Helper.TimeStamp2DateTime(e.Time),
                    Type = e.Type,
                    ParentID = e.ParentID,
                    SenderID = e.SenderID,
                    Message = e.Message,
                    MsgId = e.MsgId,
                    Recalled = e.Recalled,
                    PluginName = e.PluginName
                }).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.Error("聊天记录管理", $"按数量查询失败: {ex}");
                return [];
            }
        }

        public static ChatHistory? GetHistoriesByMsgId(long id, int msgId, ChatHistoryType historyType)
        {
            try
            {
                var db = ChatHistoryDB.GetInstance();
                var entity = db.Queryable<ChatHistoryEntity>()
                    .Where(x => x.ParentID == id && x.MsgId == msgId && x.Type == historyType)
                    .First();

                if (entity == null)
                {
                    return null;
                }

                return new ChatHistory
                {
                    ID = (int)entity.ID,
                    Time = Helper.TimeStamp2DateTime(entity.Time),
                    Type = entity.Type,
                    ParentID = entity.ParentID,
                    SenderID = entity.SenderID,
                    Message = entity.Message,
                    MsgId = entity.MsgId,
                    Recalled = entity.Recalled,
                    PluginName = entity.PluginName
                };
            }
            catch (Exception ex)
            {
                LogHelper.Error("聊天记录管理", $"按消息ID查询失败: {ex}");
                return null;
            }
        }

        public static List<ChatCategoryEntity> GetHistoryCategories()
        {
            try
            {
                var db = ChatHistoryDB.GetInstance();
                var entities = db.Queryable<ChatCategoryEntity>()
                    .OrderBy(x => x.IsPinned, OrderByType.Desc)
                    .OrderBy(x => x.Time, OrderByType.Desc)
                    .ToList();

                return entities;
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("not an error"))
                {
                    LogHelper.Error("聊天记录管理", $"获取会话列表失败: {ex}");
                }
                return [];
            }
        }

        public static void UpdateHistoryCategory(ChatHistory? chatHistory)
        {
            if (chatHistory == null)
            {
                return;
            }

            try
            {
                var db = ChatHistoryDB.GetInstance();
                var existing = db.Queryable<ChatCategoryEntity>()
                    .Where(x => x.ParentID == chatHistory.ParentID && x.Type == chatHistory.Type)
                    .First();

                if (existing == null)
                {
                    var entity = new ChatCategoryEntity
                    {
                        ParentID = chatHistory.ParentID,
                        SenderID = chatHistory.SenderID,
                        Type = chatHistory.Type,
                        Time = chatHistory.Time.ToTimeStamp(),
                        Message = chatHistory.Message,
                        UnreadCount = 0,
                        IsPinned = false
                    };
                    db.Insertable(entity).ExecuteCommand();
                }
                else
                {
                    db.Updateable<ChatCategoryEntity>()
                        .SetColumns(x => new ChatCategoryEntity
                        {
                            Time = chatHistory.Time.ToTimeStamp(),
                            Message = chatHistory.Message,
                        })
                        .Where(x => x.ID == existing.ID)
                        .ExecuteCommand();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("聊天记录管理", $"更新会话分类失败: {ex}");
            }
        }

        /// <summary>
        /// 清空指定会话的未读消息数
        /// </summary>
        /// <param name="parentId">会话ID(群号或QQ号)</param>
        /// <param name="type">会话类型</param>
        public static void SetUnreadCount(long parentId, ChatHistoryType type, int unreadCount)
        {
            try
            {
                var db = ChatHistoryDB.GetInstance();
                db.Updateable<ChatCategoryEntity>()
                    .SetColumns(x => x.UnreadCount == unreadCount)
                    .Where(x => x.ParentID == parentId && x.Type == type)
                    .ExecuteCommand();
            }
            catch (Exception ex)
            {
                LogHelper.Error("聊天记录管理", $"清空未读数失败: {ex}");
            }
        }

        public static void Initialize()
        {
            if (AppConfig.Instance.EnableChat is false || !AppConfig.Instance.IsCore)
            {
                return;
            }

            // 加载缓存
            Task.Run(LoadCacheFromDatabaseAsync);

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

            ScheduleDailyMaintenance();
        }

        private static ChatHistory InsertHistory(long id, long qq, string msg, ChatHistoryType type, DateTime time, int msgId = 0, CQPluginProxy? plugin = null)
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

                // 异步保存到数据库
                Task.Run(async () => await SaveGroupMemberToDBAsync(memberInfo));
            }
        }

        private static async void PluginManagerProxy_OnFriendAdded(long qq)
        {
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
            UpdateHistoryCategory(history);
        }

        private static void PluginManagerProxy_OnGroupMsgRecall(int msgId, long groupId, string msg)
        {
            UpdateHistoryRecall(groupId, msgId, ChatHistoryType.Group, true);
        }

        private static void PluginManagerProxy_OnPrivateMsg(int msgId, long qq, string msg, DateTime time)
        {
            var history = InsertHistory(qq, qq, msg, ChatHistoryType.Private, time, msgId: msgId);
            UpdateHistoryCategory(history);
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

                // 异步保存到数据库
                Task.Run(async () => await SaveFriendToDBAsync(info));
            }
        }

        private static void PluginManagerProxy_OnGroupNameChanged(long group, string name)
        {
            if (GroupInfoCache.TryGetValue(group, out var info) && info != null)
            {
                info.Name = name;

                // 异步保存到数据库
                Task.Run(async () => await SaveGroupToDBAsync(info));
            }
        }

        private static void PluginManagerProxy_OnGroupMemberCardChanged(long group, long qq, string card)
        {
            if (GroupMemberCache.TryGetValue(group, out var member) && member.TryGetValue(qq, out var info) && info != null)
            {
                info.Card = card;

                // 异步保存到数据库
                Task.Run(async () => await SaveGroupMemberToDBAsync(info));
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

        /// <summary>
        /// 通过url下载图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileName">若提供文件名可填</param>
        /// <returns>图片 Hash</returns>
        public static async Task<string?> CacheMessageImage(string url, string? fileName = null)
        {
            using var db = ChatHistoryDB.GetInstance();

            string baseDirectory = Helper.GetCachePictureDirectory();
            Directory.CreateDirectory(baseDirectory);
            string? absoluteFilePath = await Helper.DownloadImageAsync(url, fileName ?? Helper.GetPicNameFromUrl(url));
            if (string.IsNullOrEmpty(absoluteFilePath)
                || !File.Exists(absoluteFilePath))
            {
                // 下载失败了
                return null;
            }
            var fileBuffer = File.ReadAllBytes(absoluteFilePath);
            string hash = Helper.MD5(fileBuffer);
            CachedImage cachedImage = new()
            {
                FileName = Helper.GetRelativePath(absoluteFilePath!, baseDirectory),
                Hash = hash,
                InsertTime = DateTime.Now,
                Url = url,
                FileSizeInKB = fileBuffer.Length / 1024.0f,
            };
            CachedImage exist = db.Queryable<CachedImage>().Where(x => x.Hash == hash).First();
            if (exist != null)
            {
                cachedImage.ID = exist.ID;
                await db.Updateable(cachedImage).ExecuteCommandAsync();
            }
            else
            {
                await db.Insertable(cachedImage).ExecuteCommandAsync();
            }
            return hash;
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
                await FreeSpaceBySize();
                await FreeSpaceByExpireTime();
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

        private static async Task FreeSpaceByExpireTime()
        {
            if (!AppConfig.Instance.EnableChatImageCacheExpireTimeControl)
            {
                return;
            }

            await Task.Run(() =>
            {
                try
                {
                    var db = ChatHistoryDB.GetInstance();
                    DateTime expireTime = DateTime.Now.AddDays(-AppConfig.Instance.ChatImageCacheExpireTime);
                    
                    // 查询过期的缓存图片
                    var expiredImages = db.Queryable<CachedImage>()
                        .Where(x => x.InsertTime < expireTime && !x.Deleted)
                        .ToList();

                    foreach (var image in expiredImages)
                    {
                        try
                        {
                            // 删除文件
                            if (File.Exists(image.FileName))
                            {
                                File.Delete(image.FileName);
                            }

                            // 标记为已删除
                            db.Updateable<CachedImage>()
                                .SetColumns(x => x.Deleted == true)
                                .Where(x => x.ID == image.ID)
                                .ExecuteCommand();
                        }
                        catch (Exception ex)
                        {
                            LogHelper.WriteLog($"删除过期缓存图片失败：{image.FileName}，错误：{ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog($"清理过期缓存图片失败：{ex.Message}\n{ex.StackTrace}");
                }
            });
        }

        private static async Task FreeSpaceBySize()
        {
            if (!AppConfig.Instance.EnableChatImageCacheMaxSizeControl)
            {
                return;
            }
            await Task.Run(() =>
            {
                try
                {
                    string dir = Helper.GetCachePictureDirectory();
                    double length = 0;
                    var db = ChatHistoryDB.GetInstance();
                    
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
                                    
                                    // 同步更新数据库
                                    var cachedImage = db.Queryable<CachedImage>()
                                        .Where(x => x.FileName == Path.GetFileName(file.FullName))
                                        .First();
                                    
                                    if (cachedImage != null)
                                    {
                                        db.Updateable<CachedImage>()
                                            .SetColumns(x => x.Deleted == true)
                                            .Where(x => x.ID == cachedImage.ID)
                                            .ExecuteCommand();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogHelper.WriteLog($"删除缓存图片失败：{file.FullName}，错误：{ex.Message}");
                                }
                                files.Remove(file);
                            }
                            else
                            {
                                break;
                            }
                        } while (length > maxSize);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog($"按大小清理缓存图片失败：{ex.Message}\n{ex.StackTrace}");
                }
            });
        }

        public static async Task LoadFriendCaches()
        {
            var rawList = ProtocolManager.Instance.CurrentProtocol.GetRawFriendList(false);
            foreach (var item in rawList)
            {
                FriendInfoCache[item.QQ] = item;
                await SaveFriendToDBAsync(item);
            }
        }

        public static async Task LoadGroupInfoCaches(long groupId)
        {
            var rawGroupInfo = ProtocolManager.Instance.CurrentProtocol.GetRawGroupInfo(groupId, false);
            GroupInfoCache[groupId] = rawGroupInfo;
            await SaveGroupToDBAsync(rawGroupInfo);
        }

        public static async Task LoadGroupMemberCaches(long groupId)
        {
            var rawList = ProtocolManager.Instance.CurrentProtocol.GetRawGroupMemberList(groupId);
            GroupMemberCache[groupId] = [];
            foreach (var item in rawList)
            {
                if (item == null)
                {
                    continue;
                }
                GroupMemberCache[groupId][item.QQ] = item;
                await SaveGroupMemberToDBAsync(item);
            }
        }

        /// <summary>
        /// 从 <see cref="FriendInfoCache"/> 中获取好友昵称
        /// 若缓存中不存在则调用协议API
        /// </summary>
        /// <param name="qq">好友ID</param>
        /// <returns>昵称, 失败时返回QQ号</returns>
        public static async Task<string> GetFriendNick(long qq, bool retry = false)
        {
            if (FriendInfoCache.TryGetValue(qq, out var info)
                && info != null)
            {
                if (string.IsNullOrEmpty(info.Postscript))
                {
                    return string.IsNullOrEmpty(info.Nick) ? qq.ToString() : info.Nick;
                }
                else
                {
                    return info.Postscript;
                }
            }
            else if (!retry)
            {
                await LoadFriendCaches();
                return await GetFriendNick(qq, true);
            }
            return qq.ToString();
        }

        /// <summary>
        /// 从 <see cref="GroupMemberCache"/> 中获取群员名片
        /// 若缓存中不存在则调用协议API
        /// </summary>
        /// <param name="group">群来源</param>
        /// <param name="qq">群员QQ</param>
        /// <returns>群员名片, 若不存在则返回昵称, 若调用失败则返回QQ号</returns>
        public static async Task<string> GetGroupMemberNick(long groupId, long qq, bool retry = false)
        {
            if (GroupMemberCache.TryGetValue(groupId, out var member)
                && member != null)
            {
                if (member.TryGetValue(qq, out var info))
                {
                    if (string.IsNullOrEmpty(info.Card))
                    {
                        return string.IsNullOrEmpty(info.Nick) ? qq.ToString() : info.Nick;
                    }
                    else
                    {
                        return info.Card;
                    }
                }
                else
                {
                    return qq.ToString();
                }
            }
            else if (!retry)
            {
                await LoadGroupMemberCaches(groupId);
                return await GetGroupMemberNick(groupId, qq, true);
            }
            return qq.ToString();
        }

        /// <summary>
        /// 从 <see cref="GroupInfoCache"/> 中获取群名称
        /// 若缓存中不存在则调用协议API
        /// </summary>
        /// <param name="groupId">群号</param>
        /// <returns>群名称, 若不存在则返回群号</returns>
        public static async Task<string> GetGroupName(long groupId, bool retry = false)
        {
            if (GroupInfoCache.TryGetValue(groupId, out var info)
                && info != null)
            {
                return string.IsNullOrEmpty(info.Name) ? groupId.ToString() : info.Name;
            }
            else if (!retry)
            {
                await LoadGroupInfoCaches(groupId);
                return await GetGroupName(groupId, true);
            }
            return groupId.ToString();
        }

        private static void ScheduleDailyMaintenance()
        {
            var now = DateTime.Now;
            var nextRun = DateTime.Today.AddHours(4);
            if (now.Hour >= 4)
            {
                nextRun = nextRun.AddDays(1);
            }

            DailyMaintenanceTimer = new System.Timers.Timer((nextRun - now).TotalMilliseconds)
            {
                AutoReset = false
            };
            DailyMaintenanceTimer.Elapsed += async (_, _) =>
            {
                await CheckAndFreeCache();

                // 重置为每24小时执行
                DailyMaintenanceTimer.Interval = TimeSpan.FromHours(24).TotalMilliseconds;
                DailyMaintenanceTimer.AutoReset = true;
                DailyMaintenanceTimer.Start();
            };
            DailyMaintenanceTimer.Start();
        }
    }
}