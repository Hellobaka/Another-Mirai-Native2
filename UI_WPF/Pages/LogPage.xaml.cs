using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Hardcodet.Wpf.TaskbarNotification;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        private object syncLock = new();

        public LogPage()
        {
            InitializeComponent();
            LogCollections = new();
            DataContext = this;
            InitColumnWidth();
            foreach (var item in LogGridView.Columns)
            {
                var descriptor = DependencyPropertyDescriptor.FromProperty(GridViewColumn.WidthProperty, typeof(GridViewColumn));
                descriptor.AddValueChanged(item, ColumnWidthChanged);
            }
            Instance = this;
        }

        public static LogPage Instance { get; private set; }

        public ObservableCollection<LogModel> LogCollections { get; set; }

        public List<LogModel> RawLogCollections { get; set; } = new();

        public bool FormLoaded { get; private set; }

        private DispatcherTimer ResizeTimer { get; set; }

        private LogModel SelectedLog => LogView.SelectedItem as LogModel;

        public void RefilterLogCollection()
        {
            if (LogCollections == null)
            {
                return;
            }
            Dispatcher.Invoke(() =>
            {
                int targetPriority = FilterLogLevelSelector.SelectedIndex * 10;
                string search = FilterTextValue?.Text;
                var ls = RawLogCollections.Where(x => x.priority >= targetPriority).Where(x =>
                {
                    if (string.IsNullOrEmpty(search))
                    {
                        return true;
                    }
                    string dateTime = Helper.TimeStamp2DateTime(x.time).ToString("G");
                    return dateTime.Contains(search) || x.detail.Contains(search) || x.name.Contains(search) || x.source.Contains(search) || x.status.Contains(search);
                }).OrderBy(x => x.time).ToList();
                LogCollections.Clear();
                foreach (var item in ls.Skip(Math.Max(0, ls.Count - UIConfig.LogItemsCount)))
                {
                    LogCollections.Add(item);
                }
            });
        }

        private void AutoScroll_Toggled(object sender, RoutedEventArgs e)
        {
            UIConfig.LogAutoScroll = AutoScroll.IsOn;
            ConfigHelper.SetConfig("LogAutoScroll", AutoScroll.IsOn, UIConfig.DefaultConfigPath);
            if (AutoScroll.IsOn && LogCollections != null && LogCollections.Count > 0)
            {
                LogView.SelectedItem = LogCollections.Last();
                LogView.ScrollIntoView(SelectedLog);
            }
        }

        private void ColumnWidthChanged(object? sender, EventArgs e)
        {
            if (ResizeTimer == null)
            {
                return;
            }
            ResizeTimer.Stop();
            ResizeTimer.Start();
        }

        private void FilterLogLevelSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AppConfig.UseDatabase)
            {
                RawLogCollections = LogHelper.GetDisplayLogs(FilterLogLevelSelector.SelectedIndex * 10, UIConfig.LogItemsCount);
            }
            RefilterLogCollection();
            if (LogCollections != null && LogCollections.Count > 0)
            {
                LogView.SelectedItem = LogCollections.Last();
                LogView.ScrollIntoView(SelectedLog);
            }
        }

        private void FilterTextValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefilterLogCollection();
            if (LogCollections != null && LogCollections.Count > 0)
            {
                LogView.SelectedItem = LogCollections.Last();
                LogView.ScrollIntoView(SelectedLog);
            }
        }

        private void InitColumnWidth()
        {
            for (int i = 0; i < LogGridView.Columns.Count; i++)
            {
                var column = LogGridView.Columns[i];
                try
                {
                    column.Width = ConfigHelper.GetConfig($"LogColumn{i + 1}_Width", UIConfig.DefaultConfigPath, 200);
                }
                catch
                {
                }
            }
        }

        private void ListViewItem_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed && SelectedLog != null)
            {
                var thread = new Thread(() =>
                {
                    Thread.Sleep(100);
                    try
                    {
                        Clipboard.SetText(SelectedLog.detail.ToString());
                    }
                    catch
                    {
                    }
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
        }

        private void LogHelper_LogAdded(int logId, LogModel log)
        {
            lock (syncLock)
            {
                RawLogCollections.Add(log);
                BalloonIcon tipIcon = BalloonIcon.Warning;
                if (log.priority == (int)LogLevel.Warning)
                {
                    tipIcon = BalloonIcon.Warning;
                }
                else if (log.priority >= (int)LogLevel.Error)
                {
                    tipIcon = BalloonIcon.Error;
                }
                if (log.priority >= (int)LogLevel.Warning && UIConfig.ShowBalloonTip)
                {
                    MainWindow.Instance.TaskbarIcon?.ShowBalloonTip(log.source, log.detail, tipIcon);
                }
                RefilterLogCollection();
                SelectLastLog();
            }
        }

        private void SelectLastLog()
        {
            Dispatcher.Invoke(() =>
            {
                if (AutoScroll.IsOn && MainWindow.Instance.LogMenuItem.IsSelected)
                {
                    LogView.SelectedItem = LogCollections.Count > 0 ? LogCollections.Last() : null;
                    LogView.ScrollIntoView(LogView.SelectedItem);
                }
            });
        }

        private void LogHelper_LogStatusUpdated(int logId, string status)
        {
            var log = RawLogCollections.FirstOrDefault(x => x.id == logId);
            if (log != null)
            {
                log.status = status;
            }
            Dispatcher.Invoke(() =>
            {
                RefilterLogCollection();
                LogView.SelectedItem = SelectedLog;
            });
        }

        private void LogPage_Loaded(object sender, RoutedEventArgs e)
        {
            SelectLastLog();
            if (FormLoaded)
            {
                return;
            }
            if (AppConfig.UseDatabase)
            {
                int targetPriority = FilterLogLevelSelector.SelectedIndex * 10;
                RawLogCollections = LogHelper.GetDisplayLogs(targetPriority, UIConfig.LogItemsCount);
            }
            ResizeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
            ResizeTimer.Tick += ResizeTimer_Tick;
            LogHelper.LogAdded -= LogHelper_LogAdded;
            LogHelper.LogAdded += LogHelper_LogAdded;
            LogHelper.LogStatusUpdated -= LogHelper_LogStatusUpdated;
            LogHelper.LogStatusUpdated += LogHelper_LogStatusUpdated;
            AutoScroll.IsOn = UIConfig.LogAutoScroll;
            FormLoaded = true;
            RefilterLogCollection();
        }

        private void LogView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // SelectedLog = LogView.SelectedItem as LogModel;
        }

        private void ResizeTimer_Tick(object? sender, EventArgs e)
        {
            ResizeTimer.Stop();
            for (int i = 0; i < LogGridView.Columns.Count; i++)
            {
                var column = LogGridView.Columns[i];
                ConfigHelper.SetConfig($"LogColumn{i + 1}_Width", column.Width, UIConfig.DefaultConfigPath);
            }
        }
    }
}