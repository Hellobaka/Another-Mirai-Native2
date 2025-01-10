using Another_Mirai_Native.Model.Enums;

namespace Another_Mirai_Native.Config
{
    public class AppConfig : ConfigBase
    {
        public AppConfig()
            : base(@"conf\Config.json")
        {
            LoadConfig();
        }

        public static AppConfig Instance { get; set; } = new AppConfig();

        public bool IsCore { get; set; }

        public bool AutoConnect { get; set; }

        public string AutoProtocol { get; set; } = "MiraiAPIHttp";

        public bool PluginExitWhenCoreExit { get; set; }

        public int ReconnectTime { get; set; }

        /// <summary>
        /// 插件自动重启，若启用进程Exit时会自动重启插件，并根据之前的启用状态更新插件的Enable状态
        /// 若不启用，在重载时更新插件的Enable状态
        /// </summary>
        public bool RestartPluginIfDead { get; set; }

        public string WebSocketURL { get; set; } = "";

        public int Core_PID { get; set; }

        public bool Core_AutoExit { get; set; }

        public int Core_AuthCode { get; set; }

        public string Core_PluginPath { get; set; } = "";

        public string Core_WSURL { get; set; } = "";

        public int PluginInvokeTimeout { get; set; } = 120 * 1000;

        public int HeartBeatInterval { get; set; } = 30 * 1000;

        public bool UseDatabase { get; set; } = true;

        public bool DebugMode { get; set; }

        public bool DebugLazyLoad { get; set; }

        public int LoadTimeout { get; set; } = 10 * 1000;

        public int MessageCacheSize { get; set; } = 4096;

        public int TestingAuthCode { get; set; } = 0;

        public long TestQQ { get; set; } = 10001;

        public string CurrentNickName { get; set; } = "";

        public long CurrentQQ { get; set; } = 10001;

        public ServerType ServerType { get; set; } = ServerType.Pipe;

        public List<string> AutoEnablePlugin { get; set; } = new();

        public bool ShowTaskBar { get; set; } = true;

        public DateTime StartTime { get; set; }

        public bool EnableChat { get; set; }

        public bool EnableChatImageCache { get; set; }

        public int MaxChatImageCacheFolderSize { get; set; }

        public bool QRCodeCompatibilityMode { get; set; }

        public void LoadConfig()
        {
            PluginExitWhenCoreExit = GetConfig("PluginExitWhenCoreExit", true);
            AutoConnect = GetConfig("AutoConnect", false);
            QRCodeCompatibilityMode = GetConfig("QRCodeCompatibilityMode", false);
#if NET5_0_OR_GREATER
            AutoProtocol = GetConfig("AutoProtocol", "Lagrange.Core");
#else
            AutoProtocol = GetConfig("AutoProtocol", "NoConnection");
#endif
            if (string.IsNullOrEmpty(AutoProtocol))
            {
                AutoProtocol = "NoConnection";
            }
            WebSocketURL = GetConfig("WebSocketURL", "ws://127.0.0.1:30303");
            ReconnectTime = GetConfig("ReconnectTime", 5000);
            RestartPluginIfDead = GetConfig("RestartPluginIfDead", false);
            PluginInvokeTimeout = GetConfig("PluginInvokeTimeout", 120 * 1000);
            HeartBeatInterval = GetConfig("HeartBeatInterval", 30 * 1000);
            UseDatabase = GetConfig("UseDatabase", true);
            AutoEnablePlugin = GetConfig("AutoEnablePlugins", new List<string>());
            DebugMode = GetConfig("DebugMode", false);
            DebugLazyLoad = GetConfig("DebugLazyLoad", false);
            LoadTimeout = GetConfig("LoadTimeout", 10 * 1000);
            MessageCacheSize = GetConfig("MessageCacheSize", 4096);
            ServerType = (ServerType)GetConfig("ServerType", 1);
            ShowTaskBar = GetConfig("ShowTaskBar", true);
            EnableChat = GetConfig("EnableChat", true);
            EnableChatImageCache = GetConfig("EnableChatImageCache", true);
            MaxChatImageCacheFolderSize = GetConfig("MaxChatImageCacheFolderSize", 1024);
        }
    }
}