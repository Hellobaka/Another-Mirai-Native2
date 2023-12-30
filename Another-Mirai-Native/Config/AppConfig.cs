using Another_Mirai_Native.Model.Enums;
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

        /// <summary>
        /// 插件自动重启，若启用进程Exit时会自动重启插件，并根据之前的启用状态更新插件的Enable状态
        /// 若不启用，在重载时更新插件的Enable状态
        /// </summary>
        public static bool RestartPluginIfDead { get; set; }

        public static string WebSocketURL { get; set; } = "";

        public static int Core_PID { get; set; }

        public static bool Core_AutoExit { get; set; }

        public static int Core_AuthCode { get; set; }

        public static string Core_PluginPath { get; set; } = "";

        public static string Core_WSURL { get; set; } = "";

        public static int PluginInvokeTimeout { get; set; } = 120 * 1000;

        public static int HeartBeatInterval { get; set; } = 30 * 1000;

        public static bool UseDatabase { get; set; } = true;

        public static bool PluginAutoEnable { get; set; } = false;

        public static bool DebugMode { get; set; }

        public static int LoadTimeout { get; set; } = 10 * 1000;

        public static int MessageCacheSize { get; set; } = 4096;

        public static int TestingAuthCode { get; set; } = 0;

        public static long TestQQ { get; set; } = 10001;

        public static string CurrentNickName { get; set; } = "";

        public static long CurrentQQ { get; set; } = 10001;

        public static ushort gRPCListenPort { get; set; } = 30303;

        public static string gRPCListenIP { get; set; } = "127.0.0.1";

        public static ServerType ServerType { get; set; } = ServerType.WebSocket;

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
            gRPCListenPort = ConfigHelper.GetConfig("gRPCListenPort", defaultValue: (ushort)30303);
            gRPCListenIP = ConfigHelper.GetConfig("gRPCListenIP", defaultValue: "127.0.0.1");
            RestartPluginIfDead = ConfigHelper.GetConfig("RestartPluginIfDead", defaultValue: false);
            PluginInvokeTimeout = ConfigHelper.GetConfig("PluginInvokeTimeout", defaultValue: 120 * 1000);
            HeartBeatInterval = ConfigHelper.GetConfig("HeartBeatInterval", defaultValue: 30 * 1000);
            UseDatabase = ConfigHelper.GetConfig("UseDatabase", defaultValue: true);
            PluginAutoEnable = ConfigHelper.GetConfig("PluginAutoEnable", defaultValue: false);
            DebugMode = ConfigHelper.GetConfig("DebugMode", defaultValue: false);
            LoadTimeout = ConfigHelper.GetConfig("LoadTimeout", defaultValue: 10 * 1000);
            MessageCacheSize = ConfigHelper.GetConfig("MessageCacheSize", defaultValue: 4096);
            ServerType = (ServerType)ConfigHelper.GetConfig("ServerType", defaultValue: 0);
        }
    }
}