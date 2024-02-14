using Another_Mirai_Native.Config;

namespace Another_Mirai_Native.Protocol.MiraiAPIHttp
{
    public class MAHConfig : ConfigBase
    {
        public MAHConfig()
            : base(@"conf\MiraiAPIHttp.json")
        {
            LoadConfig();
        }

        public static MAHConfig Instance { get; set; } = new MAHConfig();

        public string WebSocketURL { get; private set; }

        public string AuthKey { get; private set; }

        public long QQ { get; private set; }

        public bool FullMemberInfo { get; private set; } = true;

        public void LoadConfig()
        {
            WebSocketURL = GetConfig("WebSocketURL", "");
            AuthKey = GetConfig("AuthKey", "");
            QQ = GetConfig("QQ", (long)100000);
            FullMemberInfo = GetConfig("FullMemberInfo", true);
        }
    }
}