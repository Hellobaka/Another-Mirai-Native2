namespace Another_Mirai_Native.Protocol.MiraiAPIHttp.MiraiAPIResponse
{
    public class GroupMemberInfoResponse
    {
        public long id { get; set; }

        public string memberName { get; set; }

        public string permission { get; set; }

        public string specialTitle { get; set; }

        public int joinTimestamp { get; set; }

        public int lastSpeakTimestamp { get; set; }

        public int muteTimeRemaining { get; set; }

        public Group group { get; set; }

        public class Group
        {
            public long id { get; set; }

            public string name { get; set; }

            public string permission { get; set; }
        }
    }
}