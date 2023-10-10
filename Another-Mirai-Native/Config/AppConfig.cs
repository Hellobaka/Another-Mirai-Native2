using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native
{
    public static class AppConfig
    {
        public static bool AutoConnect { get; set; }

        public static string AutoProtocol { get; set; } = "MiraiAPIHttp";

        public static short WebSocketPort { get; set; } = 30303;

        public static int Core_PID { get; set; }

        public static bool Core_AutoExit { get; set; }

        public static string Core_PluginPath { get; set; } = "";

        public static string Core_WSURL { get; set; } = "";

        public static void LoadConfig()
        {
            AutoConnect = ConfigHelper.GetConfig("AutoConnect", false);
            AutoProtocol = ConfigHelper.GetConfig("AutoProtocol", "MiraiAPIHttp");
            WebSocketPort = ConfigHelper.GetConfig("WebSocketPort", (short)30303);
        }
    }
}