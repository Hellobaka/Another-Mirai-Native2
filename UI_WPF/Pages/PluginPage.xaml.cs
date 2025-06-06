using Another_Mirai_Native.Config;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.UI.Controls;
using Another_Mirai_Native.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Another_Mirai_Native.UI.Pages
{
    /// <summary>
    /// PluginPage.xaml 的交互逻辑
    /// </summary>
    public partial class PluginPage : Page, INotifyPropertyChanged
    {
        public PluginPage()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<CQPluginProxyWrapper> CQPlugins { get; set; } = new();

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

        private bool FormLoaded { get; set; }

        private bool ReloadAllRunningStatus { set => Dispatcher.Invoke(() => ReloadAllStatus.IsActive = value); }

        private bool ReloadRunningStatus { set => Dispatcher.Invoke(() => ReloadStatus.IsActive = value); }

        private CQPluginProxy? SelectedPlugin { get; set; }

        private bool ToggleEnableRunningStatus { set => Dispatcher.Invoke(() => EnableStatus.IsActive = value); }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadPluginList()
        {
            Dispatcher.BeginInvoke(() =>
            {
                CQPlugins.Clear();
                foreach (var item in PluginManagerProxy.Proxies)
                {
                    CQPlugins.Add(new CQPluginProxyWrapper(item));
                }
                CQPlugins = CQPlugins.OrderBy(x => x.TargetPlugin.PluginName).ToObservableCollection();
                OnPropertyChanged(nameof(CQPlugins));
                MainWindow.Instance.BuildTaskbarIconMenu();
            });
        }

        private void OpenAllDataBtn_Click(object sender, RoutedEventArgs e)
        {
            Helper.OpenFolder(Path.Combine(Environment.CurrentDirectory, "data", "app"));
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
                Helper.OpenFolder(path);
            }
            else
            {
                DialogHelper.ShowSimpleDialog("嗯哼", "插件数据目录不存在");
            }
        }

        private void OpenMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPlugin == null)
            {
                return;
            }
            var menu = new ContextMenu();
            foreach (var item in SelectedPlugin.AppInfo.menu)
            {
                MenuItem menuItem = new();
                menuItem.Header = item.name;
                menuItem.Click += (a, b) =>
                {
                    if (SelectedPlugin.Enabled is false)
                    {
                        DialogHelper.ShowSimpleDialog("嗯哼", "当前插件未启用，无法调用窗口事件");
                        return;
                    }
                    Task.Run(() =>
                    {
                        PluginManagerProxy.Instance.InvokeEvent(SelectedPlugin, PluginEventType.Menu, item.function);
                    });
                };
                menu.Items.Add(menuItem);
            }
            menu.PlacementTarget = (UIElement)sender;
            menu.Placement = PlacementMode.MousePoint;
            menu.IsOpen = true;
        }

        private void OpenPluginPathBtn_Click(object sender, RoutedEventArgs e)
        {
            Helper.OpenFolder(Path.Combine(Environment.CurrentDirectory, "data", "plugins"));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (FormLoaded)
            {
                return;
            }
            ReloadAllRunningStatus = false;
            ReloadRunningStatus = false;
            ToggleEnableRunningStatus = false;
            TestBtn.IsEnabled = AppConfig.Instance.DebugMode;
            PluginManagerProxy.OnPluginEnableChanged -= PluginManagerProxy_OnPluginEnableChanged;
            PluginManagerProxy.OnPluginProxyAdded -= PluginManagerProxy_OnPluginProxyAdded;
            PluginManagerProxy.OnPluginProxyConnectStatusChanged -= PluginManagerProxy_OnPluginProxyConnectStatusChanged;
            PluginManagerProxy.OnPluginEnableChanged += PluginManagerProxy_OnPluginEnableChanged;
            PluginManagerProxy.OnPluginProxyAdded += PluginManagerProxy_OnPluginProxyAdded;
            PluginManagerProxy.OnPluginProxyConnectStatusChanged += PluginManagerProxy_OnPluginProxyConnectStatusChanged;

            DataContext = this;
            LoadPluginList();
            FormLoaded = true;
        }

        private void PluginListContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedPlugin = (PluginListContainer.SelectedItem as CQPluginProxyWrapper)?.TargetPlugin;
            UpdateAuthList();

            RemoveSelectEffect();
        }

        private async void RemoveSelectEffect()
        {
            await Task.Delay(100);
            ChatDetailListItem_Common.SetElementNoSelectEffect(AppIdDisplayer);
            ChatDetailListItem_Common.SetElementNoSelectEffect(AuthorDisplayer);
            ChatDetailListItem_Common.SetElementNoSelectEffect(VersionDisplayer);
            ChatDetailListItem_Common.SetElementNoSelectEffect(DescriptionDisplay);
        }

        private void PluginManagerProxy_OnPluginEnableChanged(CQPluginProxy plugin)
        {
            var target = CQPlugins.FirstOrDefault(x => x.TargetPlugin == plugin);
            target?.InvokePropertyChanged(nameof(target.TargetPlugin.Enabled));
            MainWindow.Instance.UpdateTrayToolTip();
        }

        private void PluginManagerProxy_OnPluginProxyAdded(CQPluginProxy plugin)
        {
            Dispatcher.Invoke(() =>
            {
                CQPlugins.Add(new CQPluginProxyWrapper(plugin));
                CQPlugins = CQPlugins.OrderBy(x => x.TargetPlugin.PluginName).ToObservableCollection();
                MainWindow.Instance.BuildTaskbarIconMenu();
            });
        }

        private void PluginManagerProxy_OnPluginProxyConnectStatusChanged(CQPluginProxy plugin)
        {
            var target = CQPlugins.FirstOrDefault(x => x.TargetPlugin == plugin);
            target?.InvokePropertyChanged(nameof(target.TargetPlugin.HasConnection));
        }

        private async void ReloadAllBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!await DialogHelper.ShowConfirmDialog("重载插件", "确定要重载所有插件吗？这可能会需要一些时间"))
            {
                return;
            }
            ReloadAllRunningStatus = true;
            await Task.Run(() =>
            {
                PluginManagerProxy.Instance.ReloadAllPlugins();
                LoadPluginList();
                ReloadAllRunningStatus = false;
            });
        }

        private async void ReloadBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPlugin == null)
            {
                return;
            }
            if (SelectedPlugin.Enabled == false)
            {
                DialogHelper.ShowSimpleDialog("嗯？", $"{SelectedPlugin.PluginName} 处于禁用状态，无法重启");
                return;
            }
            if (!await DialogHelper.ShowConfirmDialog("重载插件", $"确定要重载 {SelectedPlugin.PluginName} 吗？"))
            {
                return;
            }
            ReloadStatus.IsActive = true;
            await Task.Run(() =>
            {
                PluginManagerProxy.Instance.ReloadPlugin(SelectedPlugin);
                ReloadRunningStatus = false;
            });
        }

        private async void TestBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPlugin == null)
            {
                return;
            }
            if (SelectedPlugin.AppInfo.AuthCode != AppConfig.Instance.TestingAuthCode && !await DialogHelper.ShowConfirmDialog("测试插件", $"确定要测试 {SelectedPlugin.PluginName} 吗？此操作会导致插件无法接收事件"))
            {
                return;
            }
            if (SelectedPlugin.AppInfo.AuthCode != AppConfig.Instance.TestingAuthCode)
            {
                AppConfig.Instance.TestingAuthCode = SelectedPlugin.AppInfo.AuthCode;
                MainWindow.Instance.TestMenuItem.IsSelected = true;
            }
            else
            {
                AppConfig.Instance.TestingAuthCode = 0;
                DialogHelper.ShowSimpleDialog("测试已停止", "");
            }
            var target = CQPlugins.FirstOrDefault(x => x.TargetPlugin == SelectedPlugin);
            target?.InvokePropertyChanged(nameof(target.TargetPlugin.Enabled));
        }

        private void ToggleEnableBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPlugin == null)
            {
                return;
            }
            ToggleEnableRunningStatus = true;
            Task.Run(() =>
            {
                bool success = SelectedPlugin.Enabled ? true : (SelectedPlugin.MovePluginToTmpDir() && SelectedPlugin.LoadAppInfo());
                success = success && PluginManagerProxy.Instance.SetPluginEnabled(SelectedPlugin, !SelectedPlugin.Enabled);
                ToggleEnableRunningStatus = false;
                if (!success)
                {
                    DialogHelper.ShowSimpleDialog("哎呀", "切换插件状态失败了");
                    return;
                }
                if (SelectedPlugin.Enabled)
                {
                    AppConfig.Instance.AutoEnablePlugin.Add(SelectedPlugin.PluginName);
                }
                else
                {
                    AppConfig.Instance.AutoEnablePlugin.Remove(SelectedPlugin.PluginName);
                }
                AppConfig.Instance.AutoEnablePlugin = AppConfig.Instance.AutoEnablePlugin.Distinct().ToList();
                AppConfig.Instance.SetConfig("AutoEnablePlugins", AppConfig.Instance.AutoEnablePlugin);
            });
        }

        private void UpdateAuthList()
        {
            if (PluginListContainer.SelectedItem == null)
            {
                return;
            }

            AuthDisplay.Items.Clear();
            foreach (var item in ((CQPluginProxyWrapper)PluginListContainer.SelectedItem).TargetPlugin.AppInfo.auth.OrderBy(x => x))
            {
                if (AuthChineseName.ContainsKey(item))
                {
                    AuthDisplay.Items.Add(AuthChineseName[item]);
                }
            }
        }
    }
}