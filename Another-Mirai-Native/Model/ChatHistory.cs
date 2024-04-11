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
    }

    public enum ChatHistoryType
    {
        Group,
        Private,
        Notice,
        Other
    }
}
