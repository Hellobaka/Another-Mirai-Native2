using Another_Mirai_Native.DB;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Another_Mirai_Native.UI.Pages
{
    /// <summary>
    /// WebUIPage.xaml 的交互逻辑
    /// </summary>
    public partial class WebUIPage : Page
    {
        public bool PageLoaded { get; private set; }

        public static WebUIPage Instance { get; private set; }

        public bool StartStatus { get; set; }

        public WebUIPage()
        {
            InitializeComponent();
            Instance = this;
            Page_Loaded(null, null);
        }

#if NET5_0_OR_GREATER

        public async Task<bool> StartWebUI()
        {
            if (ProcessStartStatus.Fill == Brushes.Green)
            {
                return false;
            }
            await Task.Run(() => WebAPI.Program.BuildWebAPI([]));
            await WebAPI.Program.StartAsync();
            LogHelper.Info("启动 WebAPI", $"WebAPI 服务已尝试启动");
            return true;
        }

        public async Task<bool> StopWebUI()
        {
            if (WebAPI.Program.WebAPIHost != null && WebAPI.Program.IsRunning)
            {
                await WebAPI.Program.StopAsync();
                LogHelper.Info("停止 WebAPI", $"WebAPI 服务已停止");
                return true;
            }
            return false;
        }
#else
        public bool StartWebUI()
        {
            LogHelper.Error("启动 WebAPI", "启动 WebAPI 需要 .net8 以上版本");
            return false;
        }

        public bool StopWebUI()
        {
            return false;
        }
#endif

        private async void WebUIStartButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessStartStatus.Fill == Brushes.Green)
            {
                DialogHelper.ShowSimpleDialog("启动 WebUI", "服务无法重复启动");
                return;
            }
            if (await DialogHelper.ShowConfirmDialog("启动 WebUI", "确定要启动 WebUI 吗？") is false)
            {
                return;
            }
#if NET5_0_OR_GREATER
            await StartWebUI();
#else
            LogHelper.Error("启动 WebUI", "启动 WebUI 需要 .net8 以上版本");
#endif
        }

        private void Program_OnBlazorServiceStopped()
        {
            Dispatcher.InvokeAsync(() =>
            {
                ProcessStartStatus.Fill = Brushes.Red;
                ProcessStartText.Text = "服务已退出";
                StartStatus = false;

                MainWindow.Instance.BuildTaskbarIconMenu();
            });
        }

        private void Program_OnBlazorServiceStarted()
        {
            Dispatcher.InvokeAsync(() =>
            {
                ProcessStartStatus.Fill = Brushes.Green;
                ProcessStartText.Text = "服务已启动";
                StartStatus = true;

                MainWindow.Instance.BuildTaskbarIconMenu();
            });
        }

#if NET5_0_OR_GREATER
        private async void WebUIStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (await DialogHelper.ShowConfirmDialog("终止 WebUI", "确定要终止 WebUI 吗？") is false)
            {
                return;
            }
            await StopWebUI();
        }
#else
        private void WebUIStopButton_Click(object sender, RoutedEventArgs e)
        {
        }
#endif

        private void Page_Loaded(object? sender, RoutedEventArgs? e)
        {
            if (PageLoaded)
            {
                return;
            }
            PageLoaded = true;
#if NET5_0_OR_GREATER
            WebAPI.LogHelperTarget.OnLogReceived += (log) =>
            {
                Dispatcher.InvokeAsync(() =>
                {
                    var text = $"[{Helper.TimeStamp2DateTime(log.time):G}][{log.name}] {log.detail}{Environment.NewLine}";
                    if (log.priority >= 20)
                    {
                        if (Terminal_Error.Text.Length > 10000)
                        {
                            Terminal_Error.Text = "";
                        }

                        Terminal_Error.AppendText(text);
                        LogHelper.WriteLog(log);
                    }
                    else
                    {
                        if (Terminal_Output.Text.Length > 10000)
                        {
                            Terminal_Output.Text = "";
                        }

                        Terminal_Output.AppendText(text);
                    }
                });
            };
            WebAPI.Program.OnWebAPIServiceStarted += Program_OnBlazorServiceStarted;
            WebAPI.Program.OnWebAPIServiceStopped += Program_OnBlazorServiceStopped;
#else
            WebUIStartButton.IsEnabled = false;
            WebUIStopButton.IsEnabled = false;
#endif
        }

        private void TerminalOutputClearButton_Click(object sender, RoutedEventArgs e)
        {
            Terminal_Output.Text = "";
        }

        private void TerminalErrorClearButton_Click(object sender, RoutedEventArgs e)
        {
            Terminal_Error.Text = "";
        }
    }
}
