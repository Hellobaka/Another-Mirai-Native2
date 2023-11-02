namespace Another_Mirai_Native
{
    public static class RequestCache
    {
        public static Dictionary<string, (long, string)> FriendRequest { get; set; } = new();

        /// <summary>
        /// fromQQ nick groupId nick
        /// </summary>
        public static Dictionary<string, (long, string, long, string)> GroupRequest { get; set; } = new();

        public static Dictionary<long, string> Message { get; set; } = new();
    }
}