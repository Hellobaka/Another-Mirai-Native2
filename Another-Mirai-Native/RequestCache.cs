using Another_Mirai_Native.Config;

namespace Another_Mirai_Native
{
    public static class RequestCache
    {
        public static Dictionary<string, (long, string)> FriendRequest { get; set; } = new();

        /// <summary>
        /// fromQQ nick groupId nick
        /// </summary>
        public static Dictionary<string, (long, string, long, string)> GroupRequest { get; set; } = new();

        public static List<(long, string)> Message { get; set; } = new();

        public static void AddMessageCache(long msgId, string message)
        {
            Message.Add((msgId, message));
            if (Message.Count > 0 && Message.Count > AppConfig.Instance.MessageCacheSize)
            {
                Message.RemoveAt(0);
            }
        }
    }
}