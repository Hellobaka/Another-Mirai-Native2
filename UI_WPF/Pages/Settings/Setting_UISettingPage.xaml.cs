using Another_Mirai_Native.Config;
using ModernWpf;
using System.Windows;
using System.Windows.Controls;

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
                UIConfig.LogItemsCount = count;
                ConfigHelper.SetConfig("LogItemsCount", count, UIConfig.DefaultConfigPath);
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
            ConfigHelper.SetConfig("LogAutoScroll", LogAutoScroll.IsOn, UIConfig.DefaultConfigPath);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (FormLoaded)
            {
                return;
            }
            LogMaxCount.Text = UIConfig.LogItemsCount.ToString();
            LogAutoScroll.IsOn = UIConfig.LogAutoScroll;
            ThemeSelector.SelectedIndex = UIConfig.Theme == "Dark" ? 0 : UIConfig.Theme == "Light" ? 1 : 2;
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
                    ConfigHelper.SetConfig("Theme", "Dark", UIConfig.DefaultConfigPath);
                    break;

                case 1:
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                    ConfigHelper.SetConfig("Theme", "Light", UIConfig.DefaultConfigPath);
                    break;

                case 2:
                    ThemeManager.Current.ApplicationTheme = null;
                    ConfigHelper.SetConfig("Theme", "FollowSystem", UIConfig.DefaultConfigPath);
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
            UIConfig.ShowBalloonTip = ShowBalloonTip.IsOn;
            ConfigHelper.SetConfig("ShowBalloonTip", ShowBalloonTip.IsOn, UIConfig.DefaultConfigPath);
        }

        private void ShowWhenError_Toggled(object sender, RoutedEventArgs e)
        {
            if (!FormLoaded)
            {
                return;
            }
            UIConfig.PopWindowWhenError = ShowWhenError.IsOn;
            ConfigHelper.SetConfig("PopWindowWhenError", ShowWhenError.IsOn, UIConfig.DefaultConfigPath);
        }
    }
}