namespace Another_Mirai_Native.Protocol.QQ.Model
{
    public class MessageAttachment
    {
        /// <summary>
        /// 下载地址
        /// </summary>
        public string url { get; set; }

        public string content_type { get; set; }

        public string filename { get; set; }

        public int height { get; set; }

        public string id { get; set; }

        public int size { get; set; }

        public int width { get; set; }
    }
}