using Another_Mirai_Native.Config;
using Another_Mirai_Native.UI.Models;
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

        public SystemTheme Theme { get; set; } = SystemTheme.System;

        public string AccentColor { get; set; } = "";

        public double Width { get; set; } = 900;

        public double Height { get; set; } = 600;

        public int LogPageSize { get; set; } = 500;

        public bool LogAutoScroll { get; set; } = true;

        public bool ShowBalloonTip { get; set; } = true;

        public bool PopWindowWhenError { get; set; } = true;

        public WindowMaterial WindowMaterial { get; set; } = WindowMaterial.None;

        public bool HardwareRender { get; set; } = false;

        public int MessageContainerMaxCount { get; set; } = 15;

        public List<int> UsedFaceId { get; set; } = new();

        public bool AutoCloseWindow { get; set; }

        public bool AutoStartWebUI { get; set; }

        public void LoadConfig()
        {
            Theme = (SystemTheme)GetConfig("Theme", 0);
            WindowMaterial = (WindowMaterial)GetConfig("WindowMaterial", 0);
            AccentColor = GetConfig("AccentColor", "");
            Width = GetConfig("Window_Width", 900);
            Height = GetConfig("Window_Height", 600);
            LogPageSize = GetConfig("LogPageSize", 500);
            MessageContainerMaxCount = Math.Max(GetConfig("MessageContainerMaxCount", 15), 10);
            LogAutoScroll = GetConfig("LogAutoScroll", true);
            ShowBalloonTip = GetConfig("ShowBalloonTip", true);
            PopWindowWhenError = GetConfig("PopWindowWhenError", true);
            HardwareRender = GetConfig("HardwareRender", false);
            AutoCloseWindow = GetConfig("AutoCloseWindow", false);
            AutoStartWebUI = GetConfig("AutoStartWebUI", false);
            UsedFaceId = GetConfig("UsedFaceId", new List<int>());
        }
    }
}