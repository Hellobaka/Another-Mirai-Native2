namespace Another_Mirai_Native.Protocol.OneBot.Notice
{
    public class AdminChange
    {
        public string post_type { get; set; }

        public string notice_type { get; set; }

        public int time { get; set; }

        public long self_id { get; set; }

        public string sub_type { get; set; }

        public long user_id { get; set; }

        public int group_id { get; set; }
    }
}