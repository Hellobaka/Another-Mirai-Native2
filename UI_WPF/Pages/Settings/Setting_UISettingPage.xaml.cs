using ModernWpf;
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
            ThemeSelector.SelectedIndex = UIConfig.Instance.Theme == "Dark" ? 0 : UIConfig.Instance.Theme == "Light" ? 1 : 2;
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
    }
}