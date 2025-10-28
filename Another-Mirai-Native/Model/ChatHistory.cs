using Another_Mirai_Native.Native;
using SqlSugar;

namespace Another_Mirai_Native.Model
{
    public class ChatHistory
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }

        public DateTime Time { get; set; } = DateTime.Now;

        public ChatHistoryType Type { get; set; } = ChatHistoryType.Private;

        /// <summary>
        /// 群号或QQ
        /// </summary>
        public long ParentID { get; set; }

        public long SenderID { get; set; }

        public string Message { get; set; }

        public int MsgId { get; set; }

        public bool Recalled { get; set; }

        public string PluginName { get; set; } = "";

        public byte[] ToNative()
        {
            MemoryStream stream = new();
            using BinaryWriter binaryWriter = new(stream);
            BinaryWriterExpand.Write_Ex(binaryWriter, Time.ToString("G"));
            BinaryWriterExpand.Write_Ex(binaryWriter, ParentID);
            BinaryWriterExpand.Write_Ex(binaryWriter, SenderID);
            BinaryWriterExpand.Write_Ex(binaryWriter, Message);
            BinaryWriterExpand.Write_Ex(binaryWriter, MsgId);
            BinaryWriterExpand.Write_Ex(binaryWriter, Recalled ? 1 : 0);
            return stream.ToArray();
        }

        public static ChatHistory FromNative(byte[] buffer)
        {
            ChatHistory history = new();

            using BinaryReader binaryReader = new(new MemoryStream(buffer));

            history.Time = DateTime.ParseExact(binaryReader.ReadString_Ex(), "G", null);
            history.ParentID = binaryReader.ReadInt64_Ex();
            history.SenderID = binaryReader.ReadInt64_Ex();
            history.Message = binaryReader.ReadString_Ex();
            history.MsgId = binaryReader.ReadInt32_Ex();
            history.Recalled = binaryReader.ReadInt32_Ex() == 1;

            return history;
        }

        public static string CollectionToList(List<ChatHistory> list)
        {
            MemoryStream stream = new();
            using BinaryWriter binaryWriter = new(stream);
            BinaryWriterExpand.Write_Ex(binaryWriter, list.Count);
            foreach (var item in list)
            {
                var buffer = item.ToNative();
                BinaryWriterExpand.Write_Ex(binaryWriter, (short)buffer.Length);
                binaryWriter.Write(buffer);
            }
            return Convert.ToBase64String(stream.ToArray());
        }

        public static List<ChatHistory> RawToList(string base64)
        {
            byte[] buffer = Convert.FromBase64String(base64);
            List<ChatHistory> list = [];

            using BinaryReader binaryReader = new(new MemoryStream(buffer));
            int count = binaryReader.ReadInt32_Ex();
            for (int i = 0; i < count; i++)
            {
                byte[] tokenBuffer = binaryReader.ReadToken_Ex();
                list.Add(FromNative(tokenBuffer));
            }

            return list;
        }

        public string ToNativeBase64()
        {
            return Convert.ToBase64String(ToNative());
        }
    }

    public enum ChatHistoryType
    {
        Group,
        Private,
        Notice,
        Other
    }
}
