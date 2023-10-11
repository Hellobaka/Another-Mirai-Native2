using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Config
{
    public static class AppConfig
    {
        public static bool AutoConnect { get; set; }

        public static string AutoProtocol { get; set; } = "MiraiAPIHttp";

        public static bool StartUI { get; set; }

        public static bool PluginExitWhenCoreExit { get; set; }

        public static int ReconnectTime { get; set; }

        public static bool RestartPluginIfDead { get; set; }

        public static string WebSocketURL { get; set; } = "";

        public static int Core_PID { get; set; }

        public static bool Core_AutoExit { get; set; }

        public static string Core_PluginPath { get; set; } = "";

        public static string Core_WSURL { get; set; } = "";

        public static int PluginInvokeTimeout { get; set; } = 120 * 1000;

        public static void LoadConfig()
        {
            PluginExitWhenCoreExit = ConfigHelper.GetConfig("PluginExitWhenCoreExit", true);
            StartUI = ConfigHelper.GetConfig("StartUI", false);
            AutoConnect = ConfigHelper.GetConfig("AutoConnect", false);
            AutoProtocol = ConfigHelper.GetConfig("AutoProtocol", "MiraiAPIHttp");
            WebSocketURL = ConfigHelper.GetConfig("WebSocketURL", "ws://127.0.0.1:30303");
            ReconnectTime = ConfigHelper.GetConfig("ReconnectTime", 5000);
            RestartPluginIfDead = ConfigHelper.GetConfig("RestartPluginIfDead", false);
            PluginInvokeTimeout = ConfigHelper.GetConfig("PluginInvokeTimeout", 120 * 1000);
        }
    }
}