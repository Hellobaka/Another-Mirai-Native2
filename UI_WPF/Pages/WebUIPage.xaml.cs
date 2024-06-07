﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Another_Mirai_Native.UI.Pages
{
    /// <summary>
    /// WebUIPage.xaml 的交互逻辑
    /// </summary>
    public partial class WebUIPage : Page
    {
        public bool PageLoaded { get; private set; }

        public WebUIPage()
        {
            InitializeComponent();
        }

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
            Task.Run(() => BlazorUI.Program.Main([]));
#else
            DialogHelper.ShowSimpleDialog("启动 WebUI", "启动 WebUI 需要 .net8 以上版本");
#endif
        }

        private void Program_OnBlazorServiceStoped()
        {
            Dispatcher.InvokeAsync(() =>
            {
                ProcessStartStatus.Fill = Brushes.Red;
                ProcessStartText.Text = "服务已退出";
            });
        }

        private void Program_OnBlazorServiceStarted()
        {
            Dispatcher.InvokeAsync(() =>
            {
                ProcessStartStatus.Fill = Brushes.Green;
                ProcessStartText.Text = "服务已启动";
            });
        }

        private async void ErrorWriter_OnWrite(string msg)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                Terminal_Error.AppendText(msg);
                ScrollContainer_Error.ScrollToEnd();
            });
        }

        private async void Writer_OnWrite(string msg)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                Terminal_Output.AppendText(msg);
                ScrollContainer_Output.ScrollToEnd();
            });
        }

        private async void WebUIStopButton_Click(object sender, RoutedEventArgs e)
        {
#if NET5_0_OR_GREATER
            if (await DialogHelper.ShowConfirmDialog("终止 WebUI", "确定要终止 WebUI 吗？") is false)
            {
                return;
            }
            await BlazorUI.Program.BlazorHost.StopAsync();
#endif
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (PageLoaded)
            {
                return;
            }
            PageLoaded = true;
#if NET5_0_OR_GREATER
            ObservableTextWriter writer = new();
            writer.OnWrite += Writer_OnWrite;
            ObservableTextWriter errorWriter = new();
            errorWriter.OnWrite += ErrorWriter_OnWrite;
            Console.SetOut(writer);
            Console.SetError(errorWriter);

            BlazorUI.Program.OnBlazorServiceStarted += Program_OnBlazorServiceStarted;
            BlazorUI.Program.OnBlazorServiceStopped += Program_OnBlazorServiceStoped;
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

        private class ObservableTextWriter : TextWriter
        {
            public override Encoding Encoding => Encoding.Default;

            public event Action<string> OnWrite;

            public override void Write(char value)
            {
                OnWrite?.Invoke(value.ToString());
            }

            public override void Write(string value)
            {
                OnWrite?.Invoke(value);
            }

            public override void WriteLine(string value)
            {
                OnWrite?.Invoke(value + Environment.NewLine);
            }
        }
    }
}