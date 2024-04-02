using Another_Mirai_Native.Config;
using Another_Mirai_Native.Model;
using SqlSugar;
using System.Diagnostics;

namespace Another_Mirai_Native.DB
{
    public static class ChatHistoryHelper
    {
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

        public static void InsertHistory(ChatHistory history)
        {
            if (history == null)
            {
                return;
            }
            using var db = GetInstance(GetDBPath(history.ParentID, history.Type));
            db.Insertable(history).ExecuteCommand();
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
            var item = db.Queryable<ChatHistory>().Where(x => x.ParentID == id && x.MsgId == msgId).OrderByDescending(x=>x.ID).First();
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