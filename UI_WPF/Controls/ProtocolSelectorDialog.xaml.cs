﻿using Another_Mirai_Native.Config;
using ModernWpf.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Another_Mirai_Native.UI.Controls
{
    /// <summary>
    /// ProtocolSelectorDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ProtocolSelectorDialog
    {
        public ProtocolSelectorDialog()
        {
            InitializeComponent();
        }

        public bool HasProtocolContent
        {
            set
            {
                HasProtocolContentDisplay.Visibility = value ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public bool Connecting
        {
            set
            {
                ConnectButton.IsEnabled = !value;
                ConnectingStatus.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public ContentDialogResult DialogResult { get; set; }

        private IProtocol? CurrentProtocol { get; set; }

        private void ProtocolSelector_Loaded(object sender, RoutedEventArgs e)
        {
            Connecting = false;
            HasProtocolContent = false;
            AutoConnectSelector.IsOn = AppConfig.Instance.AutoConnect;
            foreach (var item in ProtocolManager.Protocols)
            {
                ProtocolList.Items.Add(item.Name);
            }
            ProtocolList.Text = AppConfig.Instance.AutoProtocol;
            if (AutoConnectSelector.IsOn)
            {
                ConnectButton_Click(sender, e);
            }
        }

        private void ProtocolList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProtocolList.SelectedItem == null || string.IsNullOrEmpty(ProtocolList.SelectedItem.ToString()))
            {
                return;
            }
            ErrorDisplay.Text = "";
            ProtocolConfigContainer.Children.Clear();
            CurrentProtocol = ProtocolManager.Protocols.FirstOrDefault(x => x.Name == ProtocolList.SelectedItem.ToString());
            if (CurrentProtocol != null)
            {
                foreach (var item in CurrentProtocol.GetConnectionConfig())
                {
                    bool isBoolOption = item.Key.StartsWith("bool_");
                    ProtocolConfigContainer.Children.Add(new TextBlock()
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Text = item.Key.Replace("bool_", ""),
                    });
                    if (isBoolOption)
                    {
                        ProtocolConfigContainer.Children.Add(new ToggleSwitch()
                        {
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            IsOn = item.Value == "true",
                            Tag = item.Key,
                            OnContent = "启用",
                            OffContent = "不启用"
                        });
                    }
                    else
                    {
                        ProtocolConfigContainer.Children.Add(new TextBox()
                        {
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            Text = item.Value,
                            Tag = item.Key
                        });
                    }
                }
            }
            HasProtocolContent = ProtocolConfigContainer.Children.Count > 0;
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorDisplay.Text = "";
            if (CurrentProtocol == null)
            {
                ErrorDisplay.Text = "请选择一个连接协议";
                return;
            }
            Dictionary<string, string> configs = new();
            for (int i = 0; i < ProtocolConfigContainer.Children.Count; i += 2)
            {
                if (ProtocolConfigContainer.Children[i + 1] is TextBox child)
                {
                    configs.Add(child.Tag?.ToString() ?? "", child.Text);
                }
                else if(ProtocolConfigContainer.Children[i + 1] is ToggleSwitch toggle)
                {
                    configs.Add(toggle.Tag?.ToString() ?? "", toggle.IsOn.ToString().ToLower());
                }
            }
            if (!CurrentProtocol.SetConnectionConfig(configs))
            {
                ErrorDisplay.Text = "配置校验失败，请修改后重试";
            }
            Connecting = true;
            Task.Run(() =>
            {
                bool ret = ProtocolManager.Instance.Start(CurrentProtocol);
                Dispatcher.Invoke(() =>
                {
                    Connecting = false;
                    if (ret)
                    {
                        AppConfig.Instance.AutoProtocol = CurrentProtocol.Name;
                        AppConfig.Instance.SetConfig("AutoProtocol", CurrentProtocol.Name);

                        DialogResult = ContentDialogResult.Primary;
                        Hide();
                    }
                    else
                    {
                        ErrorDisplay.Text = "连接失败，请检查配置后重试";
                        Connecting = false;
                    }
                });
            });
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = ContentDialogResult.Secondary;
            Hide();
        }

        private void AutoConnectSelector_Toggled(object sender, RoutedEventArgs e)
        {
            AppConfig.Instance.AutoConnect = AutoConnectSelector.IsOn;
            AppConfig.Instance.SetConfig("AutoConnect", AppConfig.Instance.AutoConnect);
        }
    }
}