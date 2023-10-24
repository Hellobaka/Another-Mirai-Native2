using Another_Mirai_Native.Native;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
            // TODO: 失去连接状态
            // TODO: 未启用状态
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

        private CQPluginProxy SelectedPlugin { get; set; }

        private void PluginListContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedPlugin = PluginListContainer.SelectedItem as CQPluginProxy;
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
            // TODO: 确认窗口
            // TODO: 按钮进度显示
            Task.Run(() =>
            {
                PluginManagerProxy.Instance.ReloadAllPlugins();
                MainWindow.Instance.EnablePluginByConfig();
            });
        }

        private void OpenPluginPathBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Path.Combine(Environment.CurrentDirectory, "data", "plugins"));
        }

        private void OpenAllDataBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Path.Combine(Environment.CurrentDirectory, "data", "app"));
        }

        private void ToggleEnableBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPlugin == null)
            {
                return;
            }
            if (SelectedPlugin.HasConnection is false)
            {
                // TODO: 自定义错误窗口
                return;
            }
            // TODO: 进度按钮
            Task.Run(() =>
            {
                PluginManagerProxy.Instance.SetPluginEnabled(SelectedPlugin, !SelectedPlugin.Enabled);
            });
            // TODO: 更新自动启用表
        }

        private void OpenMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            // TODO: ContextMenu
        }

        private void OpenDataBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPlugin == null)
            {
                return;
            }
            string path = Path.Combine(Environment.CurrentDirectory, "data", "app", SelectedPlugin.PluginId);
            if (Directory.Exists(path))
            {
                Process.Start(path);
            }
            else
            {
                // TODO: 自定义错误窗口
            }
        }

        private void ReloadBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPlugin == null)
            {
                return;
            }
            if (SelectedPlugin.HasConnection is false)
            {
                // TODO: 自定义错误窗口
                return;
            }
            // TODO: 确认窗口
            // TODO: 按钮进度显示
            Task.Run(() =>
            {
                bool enable = SelectedPlugin.Enabled;
                string id = SelectedPlugin.PluginId;
                PluginManagerProxy.Instance.ReloadPlugin(SelectedPlugin);
                if (enable)
                {
                    PluginManagerProxy.Instance.SetPluginEnabled(PluginManagerProxy.Proxies.FirstOrDefault(x => x.PluginId == id), true);
                }
            });
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
            CQPlugins.Clear();
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