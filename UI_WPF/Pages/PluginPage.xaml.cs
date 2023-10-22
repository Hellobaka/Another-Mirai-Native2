using Another_Mirai_Native.Native;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Another_Mirai_Native.UI.Pages
{
    /// <summary>
    /// PluginPage.xaml 的交互逻辑
    /// </summary>
    public partial class PluginPage : Page
    {
        public PluginPage()
        {
            InitializeComponent();
        }

        public ObservableCollection<CQPluginProxy> CQPlugins { get; set; } = new();

        private Dictionary<int, string> AuthChineseName { get; set; } = new() {
            {20, "[敏感]取Cookies"},
            {30, "接收语音"},
            {101, "发送群消息"},
            {103, "发送讨论组消息"},
            {106, "发送私聊消息"},
            {110, "[敏感]发送赞"},
            {120, "置群员移除"},
            {121, "置群员禁言"},
            {122, "置群管理员"},
            {123, "置全群禁言"},
            {124, "置匿名群员禁言"},
            {125, "置群匿名设置"},
            {126, "置群成员名片"},
            {127, "[敏感]置群退出"},
            {128, "置群成员专属头衔"},
            {130, "取群成员信息"},
            {131, "取陌生人信息"},
            {132, "取群信息"},
            {140, "置讨论组退出"},
            {150, "置好友添加请求"},
            {151, "置群添加请求"},
            {160, "取群成员列表"},
            {161, "取群列表"},
            {162, "取好友列表"},
            {180, "撤回消息"},
        };

        private void PluginListContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAuthList();
        }

        private void UpdateAuthList()
        {
            if (PluginListContainer.SelectedItem == null)
            {
                return;
            }
            AuthDisplay.Items.Clear();
            foreach (var item in ((CQPluginProxy)PluginListContainer.SelectedItem).AppInfo.auth.OrderBy(x => x))
            {
                if (AuthChineseName.ContainsKey(item))
                {
                    AuthDisplay.Items.Add(AuthChineseName[item]);
                }
            }
        }

        private void ReloadAllBtn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void OpenPluginPathBtn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void OpenAllDataBtn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ToggleEnableBtn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void OpenMenuBtn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void OpenDataBtn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ReloadBtn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            PluginManagerProxy.OnPluginEnableChanged -= PluginManagerProxy_OnPluginEnableChanged;
            PluginManagerProxy.OnPluginProxyAdded -= PluginManagerProxy_OnPluginProxyAdded;
            PluginManagerProxy.OnPluginProxyRemoved -= PluginManagerProxy_OnPluginProxyRemoved;
            PluginManagerProxy.OnPluginEnableChanged += PluginManagerProxy_OnPluginEnableChanged;
            PluginManagerProxy.OnPluginProxyAdded += PluginManagerProxy_OnPluginProxyAdded;
            PluginManagerProxy.OnPluginProxyRemoved += PluginManagerProxy_OnPluginProxyRemoved;

            DataContext = this;
            foreach (var item in PluginManagerProxy.Proxies)
            {
                CQPlugins.Add(item);
            }
        }

        private void PluginManagerProxy_OnPluginProxyRemoved(CQPluginProxy plugin)
        {
            UpdatePluginList(plugin);
        }

        private void PluginManagerProxy_OnPluginProxyAdded(CQPluginProxy plugin)
        {
            Dispatcher.Invoke(() => CQPlugins.Add(plugin));
        }

        private void PluginManagerProxy_OnPluginEnableChanged(CQPluginProxy plugin)
        {
            UpdatePluginList(plugin);
        }

        private void UpdatePluginList(CQPluginProxy plugin)
        {
            if (plugin == null)
            {
                return;
            }
            Dispatcher.Invoke(() =>
            {
                for (int i = 0; i < CQPlugins.Count; i++)
                {
                    var p = CQPlugins[i];
                    if (p.PluginId == plugin.PluginId)
                    {
                        CQPlugins[i] = plugin;
                        return;
                    }
                }
            });
        }
    }
}