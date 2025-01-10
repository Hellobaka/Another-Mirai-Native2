using Another_Mirai_Native.Config;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Another_Mirai_Native.UI.Pages
{
    /// <summary>
    /// Setting_CorePage.xaml 的交互逻辑
    /// </summary>
    public partial class Setting_CorePage : Page
    {
        public Setting_CorePage()
        {
            InitializeComponent();
        }

        public bool FormLoaded { get; set; }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (FormLoaded)
            {
                return;
            }
            InitDisplay();
            FormLoaded = true;
        }

        private void InitDisplay()
        {
            AutoProtocol.Items.Clear();
            foreach (var item in ProtocolManager.Protocols)
            {
                AutoProtocol.Items.Add(item.Name);
            }
            AutoConnect.IsOn = AppConfig.Instance.AutoConnect;
            AutoProtocol.Text = AppConfig.Instance.AutoProtocol;
            PluginExitWhenCoreExit.IsOn = AppConfig.Instance.PluginExitWhenCoreExit;
            RestartPluginIfDead.IsOn = AppConfig.Instance.RestartPluginIfDead;
            ReconnectTime.Text = AppConfig.Instance.ReconnectTime.ToString();
            HeartBeatInterval.Text = AppConfig.Instance.HeartBeatInterval.ToString();
            PluginInvokeTimeout.Text = AppConfig.Instance.PluginInvokeTimeout.ToString();
            LoadTimeout.Text = AppConfig.Instance.LoadTimeout.ToString();
            UseDatabase.IsOn = AppConfig.Instance.UseDatabase;
            DebugMode.IsOn = AppConfig.Instance.DebugMode;
            MessageCacheSize.Text = AppConfig.Instance.MessageCacheSize.ToString();
            ChatEnable.IsOn = AppConfig.Instance.EnableChat;
            EnableChatImageCache.IsOn = AppConfig.Instance.EnableChatImageCache;
            MaxChatImageCacheFolderSize.Text = AppConfig.Instance.MaxChatImageCacheFolderSize.ToString();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(Container); i++)
            {
                var child = VisualTreeHelper.GetChild(Container, i);
                if (child == null)
                {
                    continue;
                }
                if (child is TextBox textBox)
                {
                    textBox.TextChanged -= TextBox_TextChanged;
                    textBox.TextChanged += TextBox_TextChanged;
                }
                else if (child is ModernWpf.Controls.ToggleSwitch toggler)
                {
                    toggler.Toggled -= Toggler_Toggled;
                    toggler.Toggled += Toggler_Toggled;
                }
                else if (child is ComboBox comboBox)
                {
                    comboBox.SelectionChanged -= ComboBox_SelectionChanged;
                    comboBox.SelectionChanged += ComboBox_SelectionChanged;
                }
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                AppConfig.Instance.SetConfig(comboBox.Name, e.AddedItems[0]?.ToString());
                UpdateAppConfig(comboBox.Name, e.AddedItems[0]?.ToString());
            }
        }

        private void Toggler_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ModernWpf.Controls.ToggleSwitch toggleSwitch)
            {
                AppConfig.Instance.SetConfig(toggleSwitch.Name, toggleSwitch.IsOn);
                UpdateAppConfig(toggleSwitch.Name, toggleSwitch.IsOn);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox && int.TryParse(textBox.Text, out int value))
            {
                AppConfig.Instance.SetConfig(textBox.Name, value);
                UpdateAppConfig(textBox.Name, value);
            }
        }

        private void UpdateAppConfig(string key, object? value)
        {
            if (value == null)
            {
                return;
            }
            var propertiesInfos = typeof(AppConfig).GetProperties();
            var property = propertiesInfos.FirstOrDefault(x => x.Name == key);
            property?.SetValue(AppConfig.Instance, value);
        }
    }
}