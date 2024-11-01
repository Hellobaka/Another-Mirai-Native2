using Another_Mirai_Native.Config;
using Another_Mirai_Native.Model;
using SqlSugar;
using System.Diagnostics;

namespace Another_Mirai_Native.DB
{
    public static class ChatHistoryHelper
    {
        private static SemaphoreSlim APILock { get; set; } = new(1, 1);

        public static Dictionary<long, FriendInfo> FriendInfoCache { get; set; } = new();

        public static Dictionary<long, GroupInfo> GroupInfoCache { get; set; } = new();

        public static Dictionary<long, Dictionary<long, GroupMemberInfo>> GroupMemberCache { get; set; } = new();

        private static string GetDBPath(long id, ChatHistoryType type)
        {
            var path = Path.Combine("logs", "ChatHistory", type.ToString(), id.ToString() + ".db");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
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

        public static List<ChatHistory> GetHistoriesByPage(long id, ChatHistoryType historyType, int pageSize, int pageIndex)
        {
            using var db = GetInstance(GetDBPath(id, historyType));
            var ls = db.Queryable<ChatHistory>().OrderByDescending(x => x.Time).ToPageList(pageIndex, pageSize);
            ls.Reverse();
            return ls;
        }

        public static ChatHistory GetHistoriesByMsgId(long id, int msgId, ChatHistoryType historyType)
        {
            using var db = GetInstance(GetDBPath(id, historyType));
            var item = db.Queryable<ChatHistory>().First(x => x.MsgId == msgId);
            return item;
        }

        public static List<ChatHistory> GetHistoryCategroies()
        {
            using var db = GetInstance(GetDBPath(AppConfig.Instance.CurrentQQ, ChatHistoryType.Other));
            return db.Queryable<ChatHistory>().ToList();
        }

        public static void UpdateHistoryCategory(ChatHistory chatHistory)
        {
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
                        GroupInfoCache.Add(groupId, ProtocolManager.Instance.CurrentProtocol.GetRawGroupInfo(groupId, false));
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
    }
}