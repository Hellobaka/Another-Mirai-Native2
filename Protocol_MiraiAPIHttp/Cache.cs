using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Another_Mirai_Native.Protocol.MiraiAPIHttp
{
    public static class Cache
    {
        public static Dictionary<long, (long, string)> FriendRequest { get; set; } = new();

        public static Dictionary<long, (long, string, long, string)> GroupRequest { get; set; } = new();

        public static Dictionary<long, JArray> GroupList { get; set; } = new();

        public static Dictionary<(long, long), string> GroupMemberInfo { get; set; } = new();
    }
}