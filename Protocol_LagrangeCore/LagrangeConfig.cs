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

        public static BotConfig BotConfig { get; set; }

        public static BotDeviceInfo BotDeviceInfo { get; set; }

        public static BotKeystore BotKeystore { get; set; }

        public static bool DebugMode { get; set; }

        public void Load()
        {
            SignUrl = GetConfig("SignUrl", "https://sign.lagrangecore.org/api/sign/30366");
            SignFallbackPlatform = GetConfig("SignFallbackPlatform", Protocols.Linux);
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
            DebugMode = GetConfig("DebugMode", false);
        }

        public void Save()
        {
            SetConfig("SignUrl", SignUrl);
            SetConfig("SignFallbackPlatform", SignFallbackPlatform.ToString());
            // 重建对象 防止将 Sign 对象存入
            SetConfig("BotConfig", new BotConfig()
            {
                AutoReconnect = BotConfig.AutoReconnect,
                AutoReLogin = BotConfig.AutoReLogin,
                GetOptimumServer = BotConfig.GetOptimumServer,
                Protocol = BotConfig.Protocol,
                UseIPv6Network = BotConfig.UseIPv6Network,
            });
            SetConfig("BotDeviceInfo", BotDeviceInfo);
            SetConfig("BotKeystore", BotKeystore);
        }
    }
}
