using Another_Mirai_Native.Config;
using Lagrange.Core.Common;

namespace Another_Mirai_Native.Protocol.LagrangeCore
{
    public class LagrangeConfig : ConfigBase
    {
        public LagrangeConfig()
            : base(@"conf\LagrangeCore.json")
        {
            Load();
            Instance = this;
        }

        public static LagrangeConfig Instance { get; set; }

        public static string SignUrl { get; set; }

        public static Protocols SignFallbackPlatform { get; set; }

        public static long QQ { get; set; }

        public static BotConfig BotConfig { get; set; }

        public static BotDeviceInfo BotDeviceInfo { get; set; }

        public static BotKeystore BotKeystore { get; set; }

        public void Load()
        {
            SignUrl = GetConfig("SignUrl", "https://sign.lagrangecore.org/api/sign/25765");
            SignFallbackPlatform = GetConfig("SignFallbackPlatform", Protocols.Windows);
            QQ = GetConfig("QQ", (long)100000);
            BotConfig = GetConfig("BotConfig", new BotConfig()
            {
                AutoReconnect = true,
                AutoReLogin = true,
                GetOptimumServer = true,
                Protocol = SignFallbackPlatform,
                UseIPv6Network = false,
            });
            BotDeviceInfo = GetConfig("BotDeviceInfo", BotDeviceInfo.GenerateInfo());
            BotKeystore = GetConfig("BotKeystore", new BotKeystore());
        }

        public void Save()
        {
            SetConfig("SignUrl", SignUrl);
            SetConfig("SignFallbackPlatform", SignFallbackPlatform);
            SetConfig("QQ", QQ);
            SetConfig("BotConfig", BotConfig);
            SetConfig("BotDeviceInfo", BotDeviceInfo);
            SetConfig("BotKeystore", BotKeystore);
        }
    }
}
