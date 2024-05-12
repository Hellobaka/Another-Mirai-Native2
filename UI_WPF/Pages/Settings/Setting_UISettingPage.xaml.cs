using ModernWpf;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace Another_Mirai_Native.UI.Pages
{
    /// <summary>
    /// Setting_UISettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class Setting_UISettingPage : Page
    {
        public Setting_UISettingPage()
        {
            InitializeComponent();
            // TODO: 缓存自动清理
        }

        public bool FormLoaded { get; set; }

        private void LogMaxCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!FormLoaded)
            {
                return;
            }
            if (int.TryParse(LogMaxCount.Text, out int count))
            {
                UIConfig.Instance.LogItemsCount = count;
                UIConfig.Instance.SetConfig("LogItemsCount", count);
                LogPage.Instance.RefilterLogCollection();
            }
        }

        private void LogAutoScroll_Toggled(object sender, RoutedEventArgs e)
        {
            if (!FormLoaded)
            {
                return;
            }
            LogPage.Instance.AutoScroll.IsOn = LogAutoScroll.IsOn;
            UIConfig.Instance.SetConfig("LogAutoScroll", LogAutoScroll.IsOn);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (FormLoaded)
            {
                return;
            }
            LogMaxCount.Text = UIConfig.Instance.LogItemsCount.ToString();
            LogAutoScroll.IsOn = UIConfig.Instance.LogAutoScroll;
            HardwareRender.IsOn = UIConfig.Instance.HardwareRender;
            ChatEnableSelector.IsOn = UIConfig.Instance.ChatEnabled;
            ShowBalloonTip.IsOn = UIConfig.Instance.ShowBalloonTip;
            ShowWhenError.IsOn = UIConfig.Instance.PopWindowWhenError;
            ThemeSelector.SelectedIndex = UIConfig.Instance.Theme == "Dark" ? 0 : UIConfig.Instance.Theme == "Light" ? 1 : 2;
            AutoStartup.IsOn = UIConfig.Instance.AutoStartup;
            AutoCloseWindow.IsOn = UIConfig.Instance.AutoCloseWindow;
            MaterialSelector.SelectedIndex = UIConfig.Instance.WindowMaterial switch
            {
                "Mica" => 1,
                "Acrylic" => 2,
                "Tabbed" => 3,
                "None" => 0,
                _ => 0
            };
            FormLoaded = true;
        }

        private void ThemeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!FormLoaded)
            {
                return;
            }
            switch (ThemeSelector.SelectedIndex)
            {
                case 0:
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                    UIConfig.Instance.SetConfig("Theme", "Dark");
                    break;

                case 1:
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                    UIConfig.Instance.SetConfig("Theme", "Light");
                    break;

                case 2:
                    ThemeManager.Current.ApplicationTheme = null;
                    UIConfig.Instance.SetConfig("Theme", "FollowSystem");
                    break;

                default:
                    break;
            }
        }

        private void ShowBalloonTip_Toggled(object sender, RoutedEventArgs e)
        {
            if (!FormLoaded)
            {
                return;
            }
            UIConfig.Instance.ShowBalloonTip = ShowBalloonTip.IsOn;
            UIConfig.Instance.SetConfig("ShowBalloonTip", ShowBalloonTip.IsOn);
        }

        private void ShowWhenError_Toggled(object sender, RoutedEventArgs e)
        {
            if (!FormLoaded)
            {
                return;
            }
            UIConfig.Instance.PopWindowWhenError = ShowWhenError.IsOn;
            UIConfig.Instance.SetConfig("PopWindowWhenError", ShowWhenError.IsOn);
        }

        private void MaterialSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!FormLoaded)
            {
                return;
            }

            var material = MaterialSelector.SelectedIndex switch
            {
                1 => MainWindow.Material.Mica,
                2 => MainWindow.Material.Acrylic,
                3 => MainWindow.Material.Tabbed,
                _ => MainWindow.Material.None,
            };
            MainWindow.Instance.ChangeMaterial(material);

            UIConfig.Instance.WindowMaterial = material.ToString();
            UIConfig.Instance.SetConfig("WindowMaterial", material.ToString());
        }

        private void SoftWareRender_Toggled(object sender, RoutedEventArgs e)
        {
            if (!FormLoaded)
            {
                return;
            }
            if (HardwareRender.IsOn)
            {
                RenderOptions.ProcessRenderMode = RenderMode.Default;
            }
            else
            {
                RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            }
            UIConfig.Instance.HardwareRender = HardwareRender.IsOn;
            UIConfig.Instance.SetConfig("HardwareRender", HardwareRender.IsOn);
        }

        private void ChatEnableSelector_Toggled(object sender, RoutedEventArgs e)
        {
            if (!FormLoaded)
            {
                return;
            }
            UIConfig.Instance.SetConfig("ChatEnabled", ChatEnableSelector.IsOn);
        }

        private void AutoStartup_Toggled(object sender, RoutedEventArgs e)
        {
            if (!FormLoaded)
            {
                return;
            }
            bool current = UIConfig.Instance.AutoStartup;
            if (!SetApplicationAutoStartup(!current))
            {
                AutoStartup.IsOn = current;
                return;
            }
            UIConfig.Instance.AutoStartup = AutoStartup.IsOn;
            UIConfig.Instance.SetConfig("AutoStartup", !current);
        }

        private bool SetApplicationAutoStartup(bool enable)
        {
            string keyPath = @"HKCU\Software\Microsoft\Windows\CurrentVersion\Run";
            string valueName = "Another-Mirai-Native2";
            string programPath = Assembly.GetExecutingAssembly().Location;

            string command = enable
                ? $"reg add \"{keyPath}\" /v \"{valueName}\" /d \"{programPath}\" /f"
                : $"reg delete \"{keyPath}\" /v \"{valueName}\" /f";

            try
            {
                // UAC提权运行脚本
                var procStartInfo = new ProcessStartInfo("cmd", "/c " + command)
                {
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas"
                };

                using var proc = new Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                proc.WaitForExit();

                return true;
            }
            catch
            {
                DialogHelper.ShowSimpleDialog("设置开机启动", "无法写注册表，可能与拒绝了UAC提权有关");
                return false;
            }
        }

        private void AutoCloseWindow_Toggled(object sender, RoutedEventArgs e)
        {
            if (!FormLoaded)
            {
                return;
            }
            UIConfig.Instance.AutoCloseWindow = AutoCloseWindow.IsOn;
            UIConfig.Instance.SetConfig("AutoCloseWindow", AutoCloseWindow.IsOn);
        }
    }
}