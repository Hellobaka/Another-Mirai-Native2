namespace Another_Mirai_Native.Protocol.OneBot.Requests
{
    public class FriendRequest
    {
        public string request_type { get; set; }

        public string post_type { get; set; }

        public string comment { get; set; }

        public string flag { get; set; }

        public int time { get; set; }

        public long self_id { get; set; }

        public long user_id { get; set; }

        public int group_id { get; set; }
    }
}