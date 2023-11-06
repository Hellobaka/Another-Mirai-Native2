using Another_Mirai_Native.Config;
using Newtonsoft.Json.Linq;
using System;
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
            AutoConnect.IsOn = AppConfig.AutoConnect;
            AutoProtocol.Text = AppConfig.AutoProtocol;
            PluginExitWhenCoreExit.IsOn = AppConfig.PluginExitWhenCoreExit;
            RestartPluginIfDead.IsOn = AppConfig.RestartPluginIfDead;
            ReconnectTime.Text = AppConfig.ReconnectTime.ToString();
            HeartBeatInterval.Text = AppConfig.HeartBeatInterval.ToString();
            PluginInvokeTimeout.Text = AppConfig.PluginInvokeTimeout.ToString();
            LoadTimeout.Text = AppConfig.LoadTimeout.ToString();
            UseDatabase.IsOn = AppConfig.UseDatabase;
            PluginAutoEnable.IsOn = AppConfig.PluginAutoEnable;
            DebugMode.IsOn = AppConfig.DebugMode;
            MessageCacheSize.Text = AppConfig.MessageCacheSize.ToString();
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
                ConfigHelper.SetConfig(comboBox.Name, e.AddedItems[0]?.ToString());
                UpdateAppConfig(comboBox.Name, e.AddedItems[0]?.ToString());
            }
        }

        private void Toggler_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ModernWpf.Controls.ToggleSwitch toggleSwitch)
            {
                ConfigHelper.SetConfig(toggleSwitch.Name, toggleSwitch.IsOn);
                UpdateAppConfig(toggleSwitch.Name, toggleSwitch.IsOn);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox && int.TryParse(textBox.Text, out int value))
            {
                ConfigHelper.SetConfig(textBox.Name, value);
                UpdateAppConfig(textBox.Name, value);
            }
        }

        private void UpdateAppConfig(string key, object value)
        {
            var propertiesInfos = typeof(AppConfig).GetProperties();
            var property = propertiesInfos.FirstOrDefault(x => x.Name == key);
            property?.SetValue(null, value);
        }
    }
}