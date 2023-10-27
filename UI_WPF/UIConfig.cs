using Another_Mirai_Native.Config;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.UI
{
    public static class UIConfig
    {
        public static string DefaultConfigPath { get; set; } = @"conf/UIConfig.json";

        public static string Theme { get; set; } = "Light";

        public static string AccentColor { get; set; } = "";

        public static List<string> AutoEnablePlugins { get; set; } = new();

        public static bool AutoConnect { get; set; }

        public static void InitConfigs()
        {
            Theme = ConfigHelper.GetConfig("Theme", DefaultConfigPath, "Light");
            AccentColor = ConfigHelper.GetConfig("AccentColor", DefaultConfigPath, "");
            AutoEnablePlugins = ConfigHelper.GetConfig("AutoEnablePlugins", DefaultConfigPath, new List<string>());
            AutoConnect = ConfigHelper.GetConfig("AutoConnect", DefaultConfigPath, false);
        }
    }
}