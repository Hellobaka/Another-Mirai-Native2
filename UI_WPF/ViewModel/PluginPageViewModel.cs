using Another_Mirai_Native.Config;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.UI.Models;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace Another_Mirai_Native.UI.ViewModel
{
    public class PluginPageViewModel : INotifyPropertyChanged
    {
        public PluginPageViewModel()
        {
            CreateRelayCommands();

            IsDebugMode = AppConfig.Instance.DebugMode;
            LoadPluginList();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<string> AuthList { get; set; } = [];

        public ObservableCollection<CQPluginProxyWrapper> CQPlugins { get; set; } = [];

        public bool IsDebugMode { get; set; }

        public bool ReloadAllRunningStatus { get; set; }

        public bool ReloadRunningStatus { get; set; }

        public CQPluginProxy? SelectedPlugin => SelectedPluginWrapper?.TargetPlugin;

        [AlsoNotifyFor(nameof(SelectedPlugin))]
        public CQPluginProxyWrapper? SelectedPluginWrapper { get; set; }

        public bool ToggleEnableRunningStatus { get; set; }

        private static Dictionary<int, string> AuthChineseName { get; set; } = new() {
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

        #region Command

        public RelayCommand Command_OpenAllDataDirectory { get; set; }

        public RelayCommand Command_OpenDataDirectory { get; set; }

        public RelayCommand Command_OpenPluginPath { get; set; }

        public RelayCommand Command_ReloadAll { get; set; }

        public RelayCommand Command_ReloadPlugin { get; set; }

        public RelayCommand Command_TestPlugin { get; set; }

        public RelayCommand Command_ToggleEnable { get; set; }

        private void OpenAllDataDirectory()
        {
            Helper.OpenFolder(Path.Combine(Environment.CurrentDirectory, "data", "app"));
        }

        private void OpenDataDirectory()
        {
            if (SelectedPlugin == null) return;
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

        private void OpenPluginPath()
        {
            Helper.OpenFolder(Path.Combine(Environment.CurrentDirectory, "data", "plugins"));
        }

        private async Task ReloadAll()
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

        private async Task ReloadPlugin()
        {
            if (SelectedPlugin == null) return;
            if (SelectedPlugin.Enabled == false)
            {
                DialogHelper.ShowSimpleDialog("嗯？", $"{SelectedPlugin.PluginName} 处于禁用状态，无法重启");
                return;
            }
            if (!await DialogHelper.ShowConfirmDialog("重载插件", $"确定要重载 {SelectedPlugin.PluginName} 吗？"))
            {
                return;
            }
            ReloadRunningStatus = true;
            await Task.Run(() =>
            {
                PluginManagerProxy.Instance.ReloadPlugin(SelectedPlugin);
                ReloadRunningStatus = false;
            });
        }

        private async Task TestPlugin()
        {
            if (SelectedPlugin == null) return;
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

        private void ToggleEnable()
        {
            if (SelectedPlugin == null) return;
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

        #endregion Command

        public void InvokePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SubscribePluginProxyEvents()
        {
            PluginManagerProxy.OnPluginEnableChanged += PluginManagerProxy_OnPluginEnableChanged;
            PluginManagerProxy.OnPluginProxyAdded += PluginManagerProxy_OnPluginProxyAdded;
            PluginManagerProxy.OnPluginProxyConnectStatusChanged += PluginManagerProxy_OnPluginProxyConnectStatusChanged;
        }

        public void UnsubscribePluginProxyEvents()
        {
            PluginManagerProxy.OnPluginEnableChanged -= PluginManagerProxy_OnPluginEnableChanged;
            PluginManagerProxy.OnPluginProxyAdded -= PluginManagerProxy_OnPluginProxyAdded;
            PluginManagerProxy.OnPluginProxyConnectStatusChanged -= PluginManagerProxy_OnPluginProxyConnectStatusChanged;
        }

        private void CreateRelayCommands()
        {
            var fields = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(RelayCommand) && p.Name.StartsWith("Command_"));

            foreach (var prop in fields)
            {
                string methodName = prop.Name.Replace("Command_", "");
                var methodInfo = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                if (methodInfo != null)
                {
                    // 创建委托
                    Action<object> action = (obj) => methodInfo.Invoke(this, [obj]);
                    var cmd = new RelayCommand(action);
                    prop.SetValue(this, cmd);
                }
                else
                {
                    // 没有找到方法时
                    throw new Exception($"方法 {methodName} 未定义!");
                }
            }
        }

        private void LoadPluginList()
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                CQPlugins.Clear();
                foreach (var item in PluginManagerProxy.Proxies)
                {
                    CQPlugins.Add(new CQPluginProxyWrapper(item));
                }
                SortPlugins();
                MainWindow.Instance.BuildTaskbarIconMenu();
            });
        }

        private void OnSelectedPluginWrapperChanged()
        {
            UpdateAuthList();
        }

        private void PluginManagerProxy_OnPluginEnableChanged(CQPluginProxy plugin)
        {
            var target = CQPlugins.FirstOrDefault(x => x.TargetPlugin == plugin);
            target?.InvokePropertyChanged(nameof(target.TargetPlugin.Enabled));
            Application.Current.Dispatcher.Invoke(() => MainWindow.Instance.UpdateTrayToolTip());
        }

        private void PluginManagerProxy_OnPluginProxyAdded(CQPluginProxy plugin)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                CQPlugins.Add(new CQPluginProxyWrapper(plugin));
                SortPlugins();
                MainWindow.Instance.BuildTaskbarIconMenu();
            });
        }

        private void PluginManagerProxy_OnPluginProxyConnectStatusChanged(CQPluginProxy plugin)
        {
            var target = CQPlugins.FirstOrDefault(x => x.TargetPlugin == plugin);
            target?.InvokePropertyChanged(nameof(target.TargetPlugin.HasConnection));
        }

        private void SortPlugins()
        {
            var sorted = CQPlugins.OrderBy(x => x.TargetPlugin.PluginName).ToList();
            CQPlugins.Clear();
            foreach (var item in sorted)
            {
                CQPlugins.Add(item);
            }
        }

        private void UpdateAuthList()
        {
            AuthList.Clear();
            if (SelectedPlugin == null) return;

            foreach (var item in SelectedPlugin.AppInfo.auth.OrderBy(x => x))
            {
                if (AuthChineseName.ContainsKey(item))
                {
                    AuthList.Add(AuthChineseName[item]);
                }
            }
        }
    }
}