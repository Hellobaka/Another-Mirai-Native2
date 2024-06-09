using Another_Mirai_Native.Config;
using System;
using System.Collections.Generic;

namespace Another_Mirai_Native.UI
{
    public class UIConfig : ConfigBase
    {
        public UIConfig()
            : base(@"conf\UIConfig.json")
        {
            LoadConfig();
        }

        public static UIConfig Instance { get; set; } = new UIConfig();

        public string Theme { get; set; } = "System";

        public string AccentColor { get; set; } = "";

        public double Width { get; set; } = 900;

        public double Height { get; set; } = 600;

        public int LogItemsCount { get; set; } = 500;

        public bool LogAutoScroll { get; set; } = true;

        public bool ShowBalloonTip { get; set; } = true;

        public bool PopWindowWhenError { get; set; } = true;

        public string WindowMaterial { get; set; } = "None";

        public bool ChatEnabled { get; set; } = false;

        public bool HardwareRender { get; set; } = false;

        public int MessageContainerMaxCount { get; set; } = 15;

        public List<int> UsedFaceId { get; set; } = new();

        public bool AutoStartup { get; set; }

        public bool AutoCloseWindow { get; set; }

        public bool AutoStartWebUI { get; set; }

        public void LoadConfig()
        {
            Theme = GetConfig("Theme", "System");
            WindowMaterial = GetConfig("WindowMaterial", "None");
            AccentColor = GetConfig("AccentColor", "");
            Width = GetConfig("Window_Width", 900);
            Height = GetConfig("Window_Height", 600);
            LogItemsCount = GetConfig("LogItemsCount", 500);
            MessageContainerMaxCount = Math.Max(GetConfig("MessageContainerMaxCount", 15), 10);
            LogAutoScroll = GetConfig("LogAutoScroll", true);
            ShowBalloonTip = GetConfig("ShowBalloonTip", true);
            PopWindowWhenError = GetConfig("PopWindowWhenError", true);
            ChatEnabled = GetConfig("ChatEnabled", false);
            HardwareRender = GetConfig("HardwareRender", false);
            AutoStartup = GetConfig("AutoStartup", false);
            AutoCloseWindow = GetConfig("AutoCloseWindow", false);
            AutoStartWebUI = GetConfig("AutoStartWebUI", false);
            UsedFaceId = GetConfig("UsedFaceId", new List<int>());
        }
    }
}