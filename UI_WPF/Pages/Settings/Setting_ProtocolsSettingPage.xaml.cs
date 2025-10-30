using Another_Mirai_Native.Config;
using Another_Mirai_Native.UI.Controls;
using Another_Mirai_Native.UI.Models;
using ModernWpf.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Another_Mirai_Native.UI.Pages
{
    /// <summary>
    /// Setting_UISettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class Setting_ProtocolsSettingPage : System.Windows.Controls.Page
    {
        public Setting_ProtocolsSettingPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        public bool FormLoaded { get; set; }

        private ProtocolConfigItem[] LagrangeConfigs { get; set; } = 
        [
            new(){ Key = "SignUrl", ValueType = typeof(string), DescriptionTitle = "签名服务器 Url", DescriptionSubtitle = "仅限 Lagrange 签名 与其他的签名服务器不通用", DisplayControl = typeof(SettingItem_TextBox) },
            new(){ Key = "SignFallbackPlatform", ValueType = typeof(LagrangeCorePlatform), DescriptionTitle = "登录操作系统", DescriptionSubtitle = "", DisplayControl = typeof(SettingItem_ComboBox), DisplayValues = Enum.GetValues(typeof(LagrangeCorePlatform)) },
            new(){ Key = "DebugMode", ValueType = typeof(bool), DescriptionTitle = "调试模式", DescriptionSubtitle = "会输出更多的调试日志", DisplayControl = typeof(SettingItem_ToggleButton) },
        ];

        private ProtocolConfigItem[] OneBotConfigs { get; set; } = 
        [
            new(){ Key = "WebSocketURL", ValueType = typeof(string), DescriptionTitle = "正向 WebSocket 服务器 Url", DescriptionSubtitle = "", DisplayControl = typeof(SettingItem_TextBox) },
            new(){ Key = "AuthKey", ValueType = typeof(string), DescriptionTitle = "鉴权 Token", DescriptionSubtitle = "", DisplayControl = typeof(SettingItem_TextBox) },
            new(){ Key = "MessageType", ValueType = typeof(string), DescriptionTitle = "消息类型", DescriptionSubtitle = "大部分情况下请用 Array 类型", DisplayControl = typeof(SettingItem_ComboBox), DisplayValues = new string[]{ "Array", "CQCode" } },
            new(){ Key = "DiscardOfflineMessage", ValueType = typeof(bool), DescriptionTitle = "抛弃离线消息", DescriptionSubtitle = "只有在收到 Online 元数据时才处理消息", DisplayControl = typeof(SettingItem_ToggleButton) },
        ];

        private ProtocolConfigItem[] MiraiAPIHttpConfigs { get; set; } = 
        [
            new(){ Key = "WebSocketURL", ValueType = typeof(string), DescriptionTitle = "正向 WebSocket 服务器 Url", DescriptionSubtitle = "", DisplayControl = typeof(SettingItem_TextBox) },
            new(){ Key = "AuthKey", ValueType = typeof(string), DescriptionTitle = "鉴权 Token", DescriptionSubtitle = "", DisplayControl = typeof(SettingItem_TextBox) },
            new(){ Key = "QQ", ValueType = typeof(long), DescriptionTitle = "目标 QQ", DescriptionSubtitle = "", DisplayControl = typeof(SettingItem_TextBox)},
            new(){ Key = "FullMemberInfo", ValueType = typeof(bool), DescriptionTitle = "详细群成员信息", DescriptionSubtitle = "调用更详细的群成员信息接口 但是可能大幅度加长调用时长", DisplayControl = typeof(SettingItem_ToggleButton) },
        ];

        private ProtocolConfigItem[] SatoriConfigs { get; set; } = 
        [
            new(){ Key = "WebSocketURL", ValueType = typeof(string), DescriptionTitle = "正向 WebSocket 服务器 Url", DescriptionSubtitle = "", DisplayControl = typeof(SettingItem_TextBox) },
            new(){ Key = "Token", ValueType = typeof(string), DescriptionTitle = "鉴权 Token", DescriptionSubtitle = "", DisplayControl = typeof(SettingItem_TextBox) },
        ];

        private ProtocolConfigItem[] NoConnectionConfigs { get; set; } = 
        [
            new(){ Key = "PicServerListenIP", ValueType = typeof(string), DescriptionTitle = "图片服务器监听 IP", DescriptionSubtitle = "", DisplayControl = typeof(SettingItem_TextBox) },
            new(){ Key = "PicServerListenPort", ValueType = typeof(int), DescriptionTitle = "图片服务器监听端口", DescriptionSubtitle = "", DisplayControl = typeof(SettingItem_TextBox) },
            new(){ Key = "NoConnection_Nick", ValueType = typeof(string), DescriptionTitle = "仿真账号昵称", DescriptionSubtitle = "", DisplayControl = typeof(SettingItem_TextBox) },
            new(){ Key = "NoConnection_QQ", ValueType = typeof(long), DescriptionTitle = "仿真账号 QQ", DescriptionSubtitle = "", DisplayControl = typeof(SettingItem_TextBox) },
            new(){ Key = "ShowTestDialog", ValueType = typeof(bool), DescriptionTitle = "是否显示仿真窗口", DescriptionSubtitle = "", DisplayControl = typeof(SettingItem_ToggleButton) },
            new(){ Key = "BuildTestPicServer", ValueType = typeof(bool), DescriptionTitle = "是否使用本地图片服务器", DescriptionSubtitle = "用于仿真 cqimg 文件", DisplayControl = typeof(SettingItem_ToggleButton) },
        ];

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (FormLoaded)
            {
                return;
            }
            FormLoaded = true;
            foreach (var item in Directory.GetFiles("conf", "*.json"))
            {
                string fileName = Path.GetFileNameWithoutExtension(item);
                if (string.IsNullOrEmpty(fileName)
                    || (fileName != "LagrangeCore"
                        && fileName != "MiraiAPIHttp"
                        && fileName != "NoConnection_ProtocolConfig"
                        && fileName != "Satori_v1"
                        && fileName != "OneBot_v11"))
                {
                    continue;
                }
                string fullPath = Path.GetFullPath(item);
                (string panelName, ProtocolConfigItem[] configItems)  = fileName switch
                {
                    "LagrangeCore" => ("🛠️ Lagrange.Core", LagrangeConfigs),
                    "MiraiAPIHttp" => ("🛠️ Mirai API Http", MiraiAPIHttpConfigs),
                    "NoConnection_ProtocolConfig" => ("🛠️ 仿真协议", NoConnectionConfigs),
                    "OneBot_v11" => ("🛠️ OneBot v11", OneBotConfigs),
                    "Satori_v1" => ("🛠️ Satori", SatoriConfigs),
                    _ => (string.Empty, [])
                };
                if (string.IsNullOrEmpty(panelName))
                {
                    continue;
                }
                var panel = BuildConfigPanel(panelName, configItems);
                SetConfigToControl(panel, fullPath);
            }
        }

        private SimpleStackPanel BuildConfigPanel(string panelName, ProtocolConfigItem[] configItems)
        {
            Border container = new()
            {
                Margin = new Thickness(0, 0, 0, 10),
                Padding = new Thickness(10),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8)
            };
            container.SetResourceReference(Border.BackgroundProperty, "SystemControlBackgroundChromeMediumBrush");
            container.SetResourceReference(Border.BorderBrushProperty, "SystemControlBackgroundChromeMediumLowBrush");
            SimpleStackPanel panel = new()
            {
                Spacing = 10,
                Tag = configItems
            };
            container.Child = panel;
            ProtocolConfigContainer.Children.Add(container);

            TextBlock textBlock = new()
            {
                Text = panelName,
                Margin = new Thickness(0, 0, 0, 5),
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                Style = (Style)Application.Current.FindResource("TitleTextBlockStyle")
            };
            panel.Children.Add(textBlock);

            return panel;
        }

        private void SetConfigToControl(SimpleStackPanel panel, string filePath)
        {
            if (panel.Tag is not ProtocolConfigItem[] configItems)
            {
                return;
            }
            JObject json = JObject.Parse(File.ReadAllText(filePath));
            foreach (var config in configItems)
            {
                if (!json.ContainsKey(config.Key))
                {
                    continue;
                }
                if (config.DisplayControl == typeof(SettingItem_ComboBox))
                {
                    SettingItem_ComboBox comboBox = new()
                    {
                        Name = config.Key,
                        Title = config.DescriptionTitle,
                        Subtitle = config.DescriptionSubtitle,
                        ItemSources = config.DisplayValues,
                        SelectedItem = json[config.Key]?.ToObject(config.ValueType),
                        Tag = filePath
                    };
                    comboBox.SelectedItemChanged += ComboBox_SelectedItemChanged;
                    panel.Children.Add(comboBox);
                }
                else if (config.DisplayControl == typeof(SettingItem_TextBox))
                {
                    SettingItem_TextBox textBox = new()
                    {
                        Name = config.Key,
                        Title = config.DescriptionTitle,
                        Subtitle = config.DescriptionSubtitle,
                        Data = json[config.Key]?.ToObject(config.ValueType),
                        Tag = filePath
                    };
                    textBox.DataChanged += TextBox_DataChanged;
                    panel.Children.Add(textBox);
                }
                else if (config.DisplayControl == typeof(SettingItem_ToggleButton))
                {
                    SettingItem_ToggleButton toggleButton = new()
                    {
                        Name = config.Key,
                        Title = config.DescriptionTitle,
                        Subtitle = config.DescriptionSubtitle,
                        Toggled = json[config.Key]!.ToObject<bool>(),
                        Tag = filePath
                    };
                    toggleButton.OnToggled += ToggleButton_OnToggled;
                    panel.Children.Add(toggleButton);
                }
            }
        }

        private void ToggleButton_OnToggled(object sender, bool value)
        {
            if (!FormLoaded
                || sender is not SettingItem_ToggleButton toggleButton
                || toggleButton.Name is not string name
                || toggleButton.Tag is not string filePath)
            {
                return;
            }
            CommonConfig.SetConfig(name, value, filePath);
        }

        private void TextBox_DataChanged(object sender, object value)
        {
            if (!FormLoaded
                || sender is not SettingItem_TextBox textBox
                || textBox.Name is not string name
                || textBox.Tag is not string filePath)
            {
                return;
            }
            CommonConfig.SetConfig(name, value, filePath);
        }

        private void ComboBox_SelectedItemChanged(object sender, object value)
        {
            if (!FormLoaded
                || sender is not SettingItem_ComboBox comboBox
                || comboBox.Name is not string name
                || comboBox.Tag is not string filePath)
            {
                return;
            }
            CommonConfig.SetConfig(name, value, filePath);
        }
    }
}