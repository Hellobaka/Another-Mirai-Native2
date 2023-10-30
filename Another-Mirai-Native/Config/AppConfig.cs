using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Config
{
    public static class AppConfig
    {
        public static bool IsCore { get; set; }

        public static bool AutoConnect { get; set; }

        public static string AutoProtocol { get; set; } = "MiraiAPIHttp";

        public static bool PluginExitWhenCoreExit { get; set; }

        public static int ReconnectTime { get; set; }

        public static bool RestartPluginIfDead { get; set; }

        public static string WebSocketURL { get; set; } = "";

        public static int Core_PID { get; set; }

        public static bool Core_AutoExit { get; set; }

        public static string Core_PluginPath { get; set; } = "";

        public static string Core_WSURL { get; set; } = "";

        public static int PluginInvokeTimeout { get; set; } = 120 * 1000;

        public static int HeartBeatInterval { get; set; } = 30 * 1000;

        public static bool UseDatabase { get; set; } = true;

        public static bool PluginAutoEnable { get; set; } = false;

        public static bool DebugMode { get; set; }

        public static int LoadTimeout { get; set; } = 5 * 1000;

        public static void LoadConfig()
        {
            PluginExitWhenCoreExit = ConfigHelper.GetConfig("PluginExitWhenCoreExit", defaultValue: true);
            AutoConnect = ConfigHelper.GetConfig("AutoConnect", defaultValue: false);
            AutoProtocol = ConfigHelper.GetConfig("AutoProtocol", defaultValue: "NoConnection");
            if (string.IsNullOrEmpty(AutoProtocol))
            {
                AutoProtocol = "NoConnection";
            }
            WebSocketURL = ConfigHelper.GetConfig("WebSocketURL", defaultValue: "ws://127.0.0.1:30303");
            ReconnectTime = ConfigHelper.GetConfig("ReconnectTime", defaultValue: 5000);
            RestartPluginIfDead = ConfigHelper.GetConfig("RestartPluginIfDead", defaultValue: false);
            PluginInvokeTimeout = ConfigHelper.GetConfig("PluginInvokeTimeout", defaultValue: 120 * 1000);
            HeartBeatInterval = ConfigHelper.GetConfig("HeartBeatInterval", defaultValue: 30 * 1000);
            UseDatabase = ConfigHelper.GetConfig("UseDatabase", defaultValue: true);
            PluginAutoEnable = ConfigHelper.GetConfig("PluginAutoEnable", defaultValue: false);
            DebugMode = ConfigHelper.GetConfig("DebugMode", defaultValue: false);
            LoadTimeout = ConfigHelper.GetConfig("LoadTimeout", defaultValue: 5 * 1000);
        }
    }
}