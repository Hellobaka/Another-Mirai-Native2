using Another_Mirai_Native.Config;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Another_Mirai_Native.UI.Pages
{
    /// <summary>
    /// Setting_CustomPage.xaml 的交互逻辑
    /// </summary>
    public partial class Setting_CustomPage : Page
    {
        public string CurrentConfigPath { get; private set; }

        public JObject CurrentConfig { get; private set; }

        public Setting_CustomPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InitDisplay();
        }

        private void InitDisplay()
        {
            string? configName = (SettingPage.Instance.SettingContainer.SelectedItem as ModernWpf.Controls.NavigationViewItem)?.Tag.ToString();
            if (string.IsNullOrEmpty(configName))
            {
                return;
            }
            if (CurrentConfigPath != configName)
            {
                Container.Children.Clear();
                CurrentConfigPath = configName!;
            }
            try
            {
                CurrentConfig = JObject.Parse(File.ReadAllText(CurrentConfigPath));
                foreach (JProperty item in CurrentConfig.Properties())
                {
                    bool breakFlag = false;
                    foreach (var control in Container.Children)
                    {
                        if (control is Control c && c.Name == item.Name)
                        {
                            breakFlag = true;
                            break;
                        }
                    }
                    if (breakFlag)
                    {
                        break;
                    }
                    if (item.Value.Type != JTokenType.Integer && item.Value.Type != JTokenType.Float
                        && item.Value.Type != JTokenType.Date && item.Value.Type != JTokenType.String
                        && item.Value.Type != JTokenType.Boolean)
                    {
                        continue;
                    }
                    TextBlock textBlock = new()
                    {
                        Style = (Style)FindResource("TitleTextBlockStyle"),
                        Text = item.Name
                    };
                    Container.Children.Add(textBlock);

                    switch (item.Value.Type)
                    {
                        case JTokenType.Integer:
                        case JTokenType.Float:
                        case JTokenType.Date:
                        case JTokenType.String:
                            TextBox textBox = new()
                            {
                                HorizontalAlignment = HorizontalAlignment.Stretch,
                                Margin = new Thickness(0, 10, 0, 10),
                                Name = item.Name,
                                Text = item.Value.ToString()
                            };
                            textBox.TextChanged += (_, _) =>
                            {
                                bool success = false;
                                object? saveValue = null;
                                switch (item.Value.Type)
                                {
                                    case JTokenType.Integer:
                                        success = long.TryParse(textBox.Text, out long intValue);
                                        saveValue = intValue;
                                        break;

                                    case JTokenType.Float:
                                        success = float.TryParse(textBox.Text, out float floatValue);
                                        saveValue = floatValue;
                                        break;

                                    case JTokenType.Date:
                                        success = DateTime.TryParse(textBox.Text, out DateTime timeValue);
                                        saveValue = timeValue;
                                        break;

                                    case JTokenType.String:
                                        success = true;
                                        saveValue = textBox.Text;
                                        break;
                                }
                                if (success)
                                {
                                    CommonConfig.SetConfig(item.Name, saveValue, CurrentConfigPath);
                                }
                            };
                            Container.Children.Add(textBox);
                            break;

                        case JTokenType.Boolean:
                            ModernWpf.Controls.ToggleSwitch toggleSwitch = new()
                            {
                                Margin = new Thickness(0, 10, 0, 10),
                                OffContent = "关闭",
                                OnContent = "开启",
                                IsOn = item.Value.Value<bool>()
                            };
                            toggleSwitch.Toggled += (_, _) =>
                            {
                                CommonConfig.SetConfig(item.Name, toggleSwitch.IsOn, CurrentConfigPath);
                            };
                            Container.Children.Add(toggleSwitch);
                            break;

                        default:
                            break;
                    }
                }
            }
            catch
            {
                return;
            }
            ErrorDisplay.Visibility = Visibility.Collapsed;
        }
    }
}