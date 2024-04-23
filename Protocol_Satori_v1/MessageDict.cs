using SqlSugar;

namespace Another_Mirai_Native.Protocol.Satori
{
    public class MessageDict
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int MessageId { get; set; }

        public string ParentId { get; set; }

        public string RawMessageId { get; set; }

        private static string DBPath => Path.Combine("logs", "Satori_v1_MessageDict.db");

        public static void CreateDB()
        {
            if (File.Exists(DBPath))
            {
                return;
            }
            using var db = GetInstance(DBPath);
            db.DbMaintenance.CreateDatabase();
            db.CodeFirst.InitTables(typeof(MessageDict));
        }

        public static MessageDict GetMessageByDictId(int msgId)
        {
            using var db = GetInstance(DBPath);
            return db.Queryable<MessageDict>().First(x => x.MessageId == msgId);
        }

        public static MessageDict GetMessageByRawId(string rawId)
        {
            using var db = GetInstance(DBPath);
            return db.Queryable<MessageDict>().First(x => x.RawMessageId == rawId);
        }

        public static int InsertMessage(MessageDict message)
        {
            using var db = GetInstance(DBPath);
            return db.Insertable(message).ExecuteReturnIdentity();
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
