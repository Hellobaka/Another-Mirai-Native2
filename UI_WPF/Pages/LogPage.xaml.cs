using Another_Mirai_Native.UI.ViewModel;
using Another_Mirai_Native.UI;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Another_Mirai_Native.UI.Pages
{
    /// <summary>
    /// LogPage.xaml 的交互逻辑
    /// </summary>
    public partial class LogPage : Page
    {
        public LogPage()
        {
            InitializeComponent();

            DataContext = new LogPageViewModel();
            Instance = this;
        }

        public static LogPage Instance { get; set; }

        public LogPageViewModel ViewModel => (LogPageViewModel)DataContext;

        public bool PageLoaded { get; set; }

        private DispatcherTimer ResizeTimer { get; set; }

        public void SelectLastLog()
        {
            ViewModel.SelectLastLog();
        }

        private void ColumnWidthChanged(object? sender, EventArgs e)
        {
            if (ResizeTimer == null)
            {
                ResizeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
                ResizeTimer.Tick += ResizeTimer_Tick;
            }
            ResizeTimer.Stop();
            ResizeTimer.Start();
        }

        private void InitColumnWidth()
        {
            for (int i = 0; i < LogGridView.Columns.Count; i++)
            {
                var column = LogGridView.Columns[i];
                try
                {
                    column.Width = UIConfig.Instance.GetConfig($"LogColumn{i + 1}_Width", 200);
                }
                catch
                {
                }
            }
            foreach (var item in LogGridView.Columns)
            {
                var descriptor = DependencyPropertyDescriptor.FromProperty(GridViewColumn.WidthProperty, typeof(GridViewColumn));
                descriptor.AddValueChanged(item, ColumnWidthChanged);
            }
        }

        private void ListViewItem_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed && ViewModel.SelectedLog != null)
            {
                Clipboard.SetText(ViewModel.SelectedLog.detail.ToString());
            }
        }

        private void LogPage_Loaded(object? sender, RoutedEventArgs? e)
        {
            if (PageLoaded)
            {
                return;
            }
            PageLoaded = true;
            InitColumnWidth();
            ViewModel.Load();
            ViewModel.RequestScrollToBottom += ViewModel_RequestScrollToBottom;
        }

        private void ResizeTimer_Tick(object? sender, EventArgs e)
        {
            ResizeTimer.Stop();
            for (int i = 0; i < LogGridView.Columns.Count; i++)
            {
                var column = LogGridView.Columns[i];
                UIConfig.Instance.SetConfig($"LogColumn{i + 1}_Width", column.Width);
            }
        }

        private void ViewModel_RequestScrollToBottom()
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (LogView.Items.Count > 0)
                {
                    LogView.ScrollIntoView(LogView.Items[LogView.Items.Count - 1]);
                }
            });
        }
    }
}