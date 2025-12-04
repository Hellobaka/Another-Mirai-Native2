using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.UI.Controls;
using Another_Mirai_Native.UI.Models;
using Another_Mirai_Native.UI.Pages;
using Another_Mirai_Native.UI.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using ModernWpf;
using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
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
            Instance = this;
            UIConfig.Instance.LoadConfig();
            if (!UIConfig.Instance.HardwareRender)
            {
                RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            }
            switch (UIConfig.Instance.Theme)
            {
                case SystemTheme.Light:
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                    break;

                case SystemTheme.Dark:
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
            // 提前实例化重要页面
            PageCache.Add("LogPage", new LogPage());
            PageCache.Add("WebUIPage", new WebUIPage());
        }

        public static MainWindow Instance { get; set; }

        public TaskbarIcon TaskbarIcon { get; set; } = new()
        {
            Icon = new System.Drawing.Icon(new MemoryStream(Convert.FromBase64String(Another_Mirai_Native.Resources.TaskBarIconResources.IconBase64)))
        };

        private Dictionary<string, object> PageCache { get; set; } = new();

        private DispatcherTimer ResizeTimer { get; set; }

        private PictureViewer? QRCodeViewer { get; set; }

        public void BuildTaskbarIconMenu()
        {
            if (TaskbarIcon == null)
            {
                return;
            }
            UpdateTrayToolTip();
            TaskbarIcon.ContextMenu = DialogHelper.BuildNotifyIconContextMenu(PluginManagerProxy.Proxies,
                   exitAction: () => Environment.Exit(0),
                   webUIAction: () => WebUIMenuItem.IsSelected = true,
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
                       AboutPage.Instance.CheckUpdateButton_Click(null, null);
                   });
        }

        public void InitNotifyIcon()
        {
            Dispatcher.BeginInvoke(BuildTaskbarIconMenu);
        }

        public void UpdateTrayToolTip()
        {
            Dispatcher.BeginInvoke(() =>
            {
                TaskbarIcon.ToolTipText = $"{AppConfig.Instance.CurrentNickName}({AppConfig.Instance.CurrentQQ})\n已启用 {PluginManagerProxy.Proxies.Count(x => x.Enabled)} 个插件";
            });
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            DialogHelper.ShowErrorDialog($"UI异常: {e.Exception.Message}", e.Exception?.ToString() ?? "");
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
                PluginManagerProxy.Instance.OnPluginLoaded();
            });
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TestMenuItem.IsEnabled = AppConfig.Instance.DebugMode;
            ChatMenuItem.IsEnabled = AppConfig.Instance.EnableChat;
#if NET5_0_OR_GREATER
            WebUIMenuItem.IsEnabled = true;
#endif
            UpdateWindowMaterial();
            TaskbarIcon.TrayMouseDoubleClick += (_, _) =>
            {
                WindowState = WindowState.Normal;
                Show();
                SetForegroundWindow();
                LogPage.Instance?.SelectLastLog();
            };
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            ResizeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
            ResizeTimer.Tick += ResizeTimer_Tick;

            await ExecuteStartupLogic();
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (NavigationViewItem)args.SelectedItem;
            if (selectedItem != null)
            {
                string selectedItemTag = (string)selectedItem.Tag;
                if (PageCache.TryGetValue(selectedItemTag, out object? page))
                {
                    MainFrame.Navigate(page);
                }
                else
                {
                    Type? pageType = typeof(MainWindow).Assembly.GetType("Another_Mirai_Native.UI.Pages." + selectedItemTag);
                    if (pageType == null)
                    {
                        return;
                    }
                    var obj = Activator.CreateInstance(pageType);
                    if (obj == null)
                    {
                        return;
                    }
                    PageCache.Add(selectedItemTag, obj);
                    MainFrame.Navigate(obj);
                }
            }
        }

        private void PluginManagerProxy_OnPluginEnableChanged(CQPluginProxy _)
        {
            UpdateTrayToolTip();
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
            UIConfig.Instance.Theme = (SystemTheme)((int)ThemeManager.Current.ActualApplicationTheme + 1);
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

        private void OnQRCodeDisplay(string filePath, byte[] data)
        {
            Dispatcher.BeginInvoke(() =>
            {
                string path = Path.Combine("data", "image", "LoginQRCode");
                Directory.CreateDirectory(path);
                string fileName = Guid.NewGuid().ToString() + ".png";
                File.WriteAllBytes(Path.Combine(path, fileName), data);
                if (Uri.TryCreate(Path.GetFullPath(Path.Combine(path, fileName)), UriKind.Absolute, out var uri))
                {
                    QRCodeViewer ??= new PictureViewer
                    {
                        Title = "二维码登录 关闭后无法再次打开",
                        Owner = this
                    };
                    QRCodeViewer.Image = uri;
                    QRCodeViewer.Show();
                }
                else
                {
                    DialogHelper.ShowSimpleDialog("二维码显示失败", "二维码显示失败，无法保存图片");
                }
            });
        }

        private void OnQRCodeFinish()
        {
            Dispatcher.BeginInvoke(() =>
            {
                QRCodeViewer?.Close();
                QRCodeViewer = null;
            });
        }
        #region Startup Logic
        private void UpdateWindowMaterial()
        {
            MaxHeight = SystemParameters.WorkArea.Height + 10;
            RefreshFrame();
            RefreshDarkMode();
            ThemeManager.Current.ActualApplicationThemeChanged += (_, _) => RefreshDarkMode();
            Material material = UIConfig.Instance.WindowMaterial switch
            {
                WindowMaterial.Mica => Material.Mica,
                WindowMaterial.Acrylic => Material.Acrylic,
                WindowMaterial.Tabbed => Material.Tabbed,
                WindowMaterial.None => Material.None,
                _ => Material.None
            };
            try
            {
                if (material != Material.None)
                {
                    ChangeMaterial(material);
                    SetNavigationViewTransparent(MainDrawer);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("切换窗口材质", ex.Message);
            }
        }

        private async Task ExecuteStartupLogic()
        {
            await Dispatcher.Yield();
            await InitCoreLogic();
            if (await InitProtocolLogic() is false)
            {
                Environment.Exit(0);
            }
            else
            {
                await InitAfterLoginLogic();
            }
        }

        private async Task InitCoreLogic()
        {
            await Task.Run(() => Entry.InitCore());
        }

        private async Task<bool> InitProtocolLogic()
        {
            ProtocolManager protocolManager = new();
            SetQrCodeAction(protocolManager);
            ProtocolSelectorDialog dialog = new();
            await dialog.ShowAsync();
            return dialog.DialogResult != ContentDialogResult.Secondary;
        }

        private async Task InitAfterLoginLogic()
        {
            if (UIConfig.Instance.AutoCloseWindow)
            {
                Close();
            }
            ChatHistoryHelper.Initialize();
            // 登录成功后才有有效的QQ号，此时获取到的历史记录才有效
            PageCache.Add("ChatPage", new ChatPage());
            LoadPlugins();
            DialogHelper.HandleDialogQueue();
            InitEvents();
            await InitWebUI();
        }

        private async Task InitWebUI()
        {
#if NET5_0_OR_GREATER
            if (UIConfig.Instance.AutoStartWebUI)
            {
                await WebUIPage.Instance.StartWebUI();
            }
            else
            {
                await Task.CompletedTask;
            }
#else
            await Task.CompletedTask;
#endif
        }

        private void InitEvents()
        {
            PluginManagerProxy.OnPluginEnableChanged -= PluginManagerProxy_OnPluginEnableChanged;
            PluginManagerProxy.OnPluginEnableChanged += PluginManagerProxy_OnPluginEnableChanged;
        }

        private void SetQrCodeAction(ProtocolManager protocolManager)
        {
            protocolManager.SetQrCodeAction(OnQRCodeDisplay, OnQRCodeFinish);
        }
        #endregion
    }
}