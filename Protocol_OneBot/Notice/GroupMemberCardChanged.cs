namespace Another_Mirai_Native.Protocol.OneBot.Notice
{
    public class GroupMemberCardChanged
    {
        public int time { get; set; }

        public long self_id { get; set; }

        public string post_type { get; set; }

        public int group_id { get; set; }

        public int user_id { get; set; }

        public string notice_type { get; set; }

        public string card_new { get; set; }

        public string card_old { get; set; }
    }
}