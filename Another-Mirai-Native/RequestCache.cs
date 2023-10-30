namespace Another_Mirai_Native
{
    public static class RequestCache
    {
        public static Dictionary<long, (long, string)> FriendRequest { get; set; } = new();

        public static Dictionary<long, (long, string, long, string)> GroupRequest { get; set; } = new();

        public static Dictionary<long, string> Message { get; set; } = new();
    }
}