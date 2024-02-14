using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.UI.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using ModernWpf;
using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace Another_Mirai_Native.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UIConfig.Instance.LoadConfig();
            Instance = this;
            switch (UIConfig.Instance.Theme)
            {
                case "Light":
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                    break;

                case "Dark":
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                    break;

                default:
                    break;
            }
            try
            {
                if (string.IsNullOrEmpty(UIConfig.Instance.AccentColor) || UIConfig.Instance.AccentColor.StartsWith("#") is false)
                {
                    ThemeManager.Current.AccentColor = null;
                }
                else
                {
                    ThemeManager.Current.AccentColor = (Color)ColorConverter.ConvertFromString(UIConfig.Instance.AccentColor);
                }
            }
            catch
            {
                ThemeManager.Current.AccentColor = null;
            }
            Width = Math.Max(MinWidth, UIConfig.Instance.Width);
            Height = Math.Max(MinHeight, UIConfig.Instance.Height);
        }

        public static MainWindow Instance { get; set; }

        public TaskbarIcon TaskbarIcon { get; set; } = new()
        {
            Icon = new System.Drawing.Icon(new MemoryStream(Convert.FromBase64String(Another_Mirai_Native.Resources.TaskBarIconResources.IconBase64)))
        };


        private Dictionary<string, object> PageCache { get; set; } = new();

        private DispatcherTimer ResizeTimer { get; set; }

        public void BuildTaskbarIconMenu()
        {
            if (TaskbarIcon == null)
            {
                return;
            }
            UpdateTrayToolTip();
            TaskbarIcon.ContextMenu = DialogHelper.BuildNotifyIconContextMenu(PluginManagerProxy.Proxies,
                   exitAction: () => Environment.Exit(0),
                   reloadAction: PluginManagerProxy.Instance.ReloadAllPlugins,
                   pluginManageAction: () =>
                   {
                       Show();
                       SetForegroundWindow();
                       PluginMenuItem.IsSelected = true;
                   },
                   logAction: () =>
                   {
                       Show();
                       SetForegroundWindow();
                       LogMenuItem.IsSelected = true;
                   },
                   menuAction: (plugin, menu) =>
                   {
                       if (plugin.Enabled is false)
                       {
                           Show();
                           SetForegroundWindow();
                           DialogHelper.ShowSimpleDialog("嗯哼", "当前插件未启用，无法调用窗口事件");
                           return;
                       }
                       Task.Run(() =>
                       {
                           PluginManagerProxy.Instance.InvokeEvent(plugin, PluginEventType.Menu, menu);
                       });
                   },
                   updateAction: () =>
                   {
                       Show();
                       SetForegroundWindow();
                       AboutMenuItem.IsSelected = true;
                   });
        }

        public void UpdateTrayToolTip()
        {
            Dispatcher.BeginInvoke(() =>
            {
                TaskbarIcon.ToolTipText = $"{AppConfig.Instance.CurrentNickName}({AppConfig.Instance.CurrentQQ})\n已启用 {PluginManagerProxy.Proxies.Count(x => x.Enabled)} 个插件";
            });
        }

        public void InitNotifyIcon()
        {
            Dispatcher.BeginInvoke(BuildTaskbarIconMenu);
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            DialogHelper.ShowErrorDialog($"UI异常: {e.Exception.Message}", e.Exception.StackTrace);
        }

        private async void DisconnectProtocol_Click(object sender, RoutedEventArgs e)
        {
            if (await DialogHelper.ShowConfirmDialog("协议切换", "确定要切换协议吗，操作会导致与机器人断开连接，无法处理消息"))
            {
                AppConfig.Instance.AutoConnect = false;
                UIConfig.Instance.SetConfig("AutoConnect", false);
                bool success = await Task.Run(() =>
                {
                    return Task.FromResult(ProtocolManager.Instance.CurrentProtocol.Disconnect());
                });
                if (success)
                {
                    ProtocolSelectorDialog dialog = new();
                    await dialog.ShowAsync();
                    if (dialog.DialogResult == ContentDialogResult.Secondary)
                    {
                        Environment.Exit(0);
                    }
                    BuildTaskbarIconMenu();
                }
                else
                {
                    DialogHelper.ShowSimpleDialog("切换失败力", "协议断开连接失败，建议在设置中更改自己需要的协议并重启框架");
                }

            }
        }        

        private void LoadPlugins()
        {
            Task.Run(() =>
            {
                var manager = new PluginManagerProxy();
                manager.LoadPlugins();
                InitNotifyIcon();
                PluginManagerProxy.Instance.EnablePluginByConfig();
            });
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TaskbarIcon.TrayMouseDoubleClick += (_, _) =>
            {
                Show();
                SetForegroundWindow();
            };
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            ResizeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
            ResizeTimer.Tick += ResizeTimer_Tick;
            _ = new ProtocolManager();
            ProtocolSelectorDialog dialog = new();
            await dialog.ShowAsync();
            if (dialog.DialogResult == ContentDialogResult.Secondary)
            {
                Environment.Exit(0);
            }
            else
            {
                LoadPlugins();
            }

            PluginManagerProxy.OnPluginEnableChanged -= PluginManagerProxy_OnPluginEnableChanged;
            PluginManagerProxy.OnPluginEnableChanged += PluginManagerProxy_OnPluginEnableChanged;
        }

        private void PluginManagerProxy_OnPluginEnableChanged(CQPluginProxy _)
        {
            UpdateTrayToolTip();
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (NavigationViewItem)args.SelectedItem;
            if (selectedItem != null)
            {
                string selectedItemTag = (string)selectedItem.Tag;
                if (PageCache.ContainsKey(selectedItemTag))
                {
                    MainFrame.Navigate(PageCache[selectedItemTag]);
                }
                else
                {
                    Type pageType = typeof(MainWindow).Assembly.GetType("Another_Mirai_Native.UI.Pages." + selectedItemTag);
                    var obj = Activator.CreateInstance(pageType);
                    PageCache.Add(selectedItemTag, obj);
                    MainFrame.Navigate(obj);
                }
            }
        }

        private void ResizeTimer_Tick(object? sender, EventArgs e)
        {
            ResizeTimer.Stop();
            UIConfig.Instance.SetConfig("Window_Width", Width);
            UIConfig.Instance.SetConfig("Window_Height", Height);
        }

        private bool SetForegroundWindow()
        {
            // 通过SetForegroundWindow来激活窗口
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            return hwnd != IntPtr.Zero && SetForegroundWindow(hwnd);
        }

        private void ThemeToggle_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.Current.ApplicationTheme =
                         ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark
                         ? ApplicationTheme.Light
                         : ApplicationTheme.Dark;
            UIConfig.Instance.Theme = ThemeManager.Current.ActualApplicationTheme.ToString();
            UIConfig.Instance.SetConfig("Theme", UIConfig.Instance.Theme);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ResizeTimer == null)
            {
                return;
            }
            ResizeTimer.Stop();
            ResizeTimer.Start();
        }
    }
}