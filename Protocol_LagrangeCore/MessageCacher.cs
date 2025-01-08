using Lagrange.Core.Message;
using SqlSugar;

namespace Another_Mirai_Native.Protocol.LagrangeCore
{
    [SugarTable]
    public class MessageCacher
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }

        public int MessageId { get; set; }

        [SugarColumn(IsJson = true)]
        public MessageChain Record { get; set; }

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

        public static void CreateDB(string path)
        {
            if (File.Exists(path))
            {
                return;
            }
            using var db = GetInstance(path);
            db.DbMaintenance.CreateDatabase();
            db.CodeFirst.InitTables(typeof(MessageCacher));
        }

        private static string GetDBPath()
        {
            var path = Path.Combine("logs", "Lagrange", $"{DateTime.Now:yyyyMM}" + ".db");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            if (File.Exists(path) is false)
            {
                CreateDB(path);
            }
            return path;
        }

        public static void RecordMessage(int messageId, MessageChain message)
        {
            using var db = GetInstance(GetDBPath());
            db.Insertable(new MessageCacher()
            {
                MessageId = messageId,
                Record = message,
            }).ExecuteCommand();
        }

        public static MessageChain? GetMessageById(uint messageId)
        {
            using var db = GetInstance(GetDBPath());
            return db.Queryable<MessageCacher>().Where(x => x.MessageId == messageId).ToList().Last()?.Record;
        }

        public static int CalcMessageHash(ulong msgId, uint seq)
        {
            var messageId = BitConverter.GetBytes(msgId);
            var sequence = BitConverter.GetBytes(seq);

            byte[] id = [messageId[0], messageId[1], sequence[0], sequence[1]];
            return BitConverter.ToInt32(id.AsSpan());
        }
    }
}
