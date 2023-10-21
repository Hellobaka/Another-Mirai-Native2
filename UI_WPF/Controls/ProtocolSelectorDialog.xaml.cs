using Another_Mirai_Native.Config;
using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
                ConnectingStatus.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public ContentDialogResult DialogResult { get; set; }

        private IProtocol CurrentProtocol { get; set; }

        private void ProtocolSelector_Loaded(object sender, RoutedEventArgs e)
        {
            Connecting = false;
            HasProtocolContent = false;
            foreach (var item in ProtocolManager.Protocols)
            {
                ProtocolList.Items.Add(item.Name);
            }
            ProtocolList.Text = AppConfig.AutoProtocol;
            if (ConfigHelper.GetConfig("AutoConnect", MainWindow.DefaultConfigPath, false))
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
            var protocol = ProtocolManager.Protocols.FirstOrDefault(x => x.Name == ProtocolList.SelectedItem.ToString());
            if (protocol != null)
            {
                foreach (var item in protocol.GetConnectionConfig())
                {
                    ProtocolConfigContainer.Children.Add(new TextBlock()
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Text = item.Key
                    });
                    ProtocolConfigContainer.Children.Add(new TextBox()
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Text = item.Value,
                        Tag = item.Key
                    });
                }
            }
            CurrentProtocol = protocol;
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
                    configs.Add(child.Tag.ToString(), child.Text);
                }
            }
            if (!CurrentProtocol.SetConnectionConfig(configs))
            {
                ErrorDisplay.Text = "配置校验失败，请修改后重试";
            }
            ConfigHelper.SetConfig("AutoProtocol", CurrentProtocol.Name);
            Connecting = true;
            Task.Run(() =>
            {
                bool ret = CurrentProtocol.Connect();
                Dispatcher.Invoke(() =>
                {
                    Connecting = false;
                    if (ret)
                    {
                        DialogResult = ContentDialogResult.Primary;
                        Hide();
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
            ConfigHelper.SetConfig("AutoConnect", AutoConnectSelector.IsOn, MainWindow.DefaultConfigPath);
        }
    }
}