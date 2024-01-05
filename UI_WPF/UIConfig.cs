using Another_Mirai_Native.Config;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.UI
{
    public static class UIConfig
    {
        public static string DefaultConfigPath { get; set; } = @"conf/UIConfig.json";

        public static string Theme { get; set; } = "System";

        public static string AccentColor { get; set; } = "";

        public static List<string> AutoEnablePlugins { get; set; } = new();

        public static double Width { get; set; } = 900;

        public static double Height { get; set; } = 600;

        public static int LogItemsCount { get; set; } = 500;

        public static bool LogAutoScroll { get; set; } = true;

        public static bool ShowBalloonTip { get; set; } = true;

        public static bool PopWindowWhenError { get; set; } = true;

        public static void InitConfigs()
        {
            Theme = ConfigHelper.GetConfig("Theme", DefaultConfigPath, "System");
            AccentColor = ConfigHelper.GetConfig("AccentColor", DefaultConfigPath, "");
            AutoEnablePlugins = ConfigHelper.GetConfig("AutoEnablePlugins", DefaultConfigPath, new List<string>());
            Width = ConfigHelper.GetConfig("Window_Width", DefaultConfigPath, 900);
            Height = ConfigHelper.GetConfig("Window_Height", DefaultConfigPath, 600);
            LogItemsCount = ConfigHelper.GetConfig("LogItemsCount", DefaultConfigPath, 500);
            LogAutoScroll = ConfigHelper.GetConfig("LogAutoScroll", DefaultConfigPath, true);
            ShowBalloonTip = ConfigHelper.GetConfig("ShowBalloonTip", DefaultConfigPath, true);
            PopWindowWhenError = ConfigHelper.GetConfig("PopWindowWhenError", DefaultConfigPath, true);
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            ObservableCollection<T> list = new();
            foreach (T item in source)
            {
                list.Add(item);
            }
            return list;
        }
    }
}