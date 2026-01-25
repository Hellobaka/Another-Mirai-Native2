namespace Another_Mirai_Native.Protocol.OneBot.Notice
{
    public class FileUpload
    {
        public string post_type { get; set; }

        public string notice_type { get; set; }

        public long time { get; set; }

        public long self_id { get; set; }

        public long group_id { get; set; }

        public long user_id { get; set; }

        public File file { get; set; }

        public class File
        {
            public int busid { get; set; }

            public string id { get; set; }

            public string name { get; set; }

            public int size { get; set; }

            public string url { get; set; }
        }
    }
}