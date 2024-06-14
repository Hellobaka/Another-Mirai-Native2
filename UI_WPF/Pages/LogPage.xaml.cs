using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.UI.ViewModel;
using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Another_Mirai_Native.UI.Pages
{
    /// <summary>
    /// LogPage.xaml 的交互逻辑
    /// </summary>
    public partial class LogPage : Page, INotifyPropertyChanged
    {
        private ObservableCollection<LogModelWrapper> logCollections;

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

            Dispatcher.BeginInvoke(() => LogPage_Loaded(null, null));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public static LogPage Instance { get; set; }

        public bool FormLoaded { get; set; }

        public ObservableCollection<LogModelWrapper> LogCollections
        {
            get => logCollections;

            set
            {
                logCollections = value;
                OnPropertyChanged(nameof(LogCollections));
            }
        }

        private int FilterLogLevel { get; set; } = 0;

        private DispatcherTimer ResizeTimer { get; set; }

        private DispatcherTimer SearchDebounceTimer { get; set; }

        private string SearchText { get; set; } = "";

        private LogModelWrapper SelectedLog => LogView.SelectedItem as LogModelWrapper;

        public void RefilterLogCollection()
        {
            var ls = ConvertListToObservableCollection(
                PackLogModelWrapper(
                    LogHelper.DetailQueryLogs(FilterLogLevel, 1, UIConfig.Instance.LogItemsCount, SearchText, out _, out _, null, null)
                    )
                );
            Dispatcher.BeginInvoke(() =>
            {
                LogCollections = ls;
            });
        }

        public void SelectLastLog()
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (AutoScroll.IsOn && MainWindow.Instance.LogMenuItem.IsSelected && LogCollections.Count > 0)
                {
                    LogView.SelectedItem = LogCollections.Last();
                    LogView.UpdateLayout();
                    LogView.ScrollIntoView(LogView.SelectedItem);
                }
            });
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static ObservableCollection<T> ConvertListToObservableCollection<T>(List<T> list)
        {
            var result = new ObservableCollection<T>();
            foreach (var item in list)
            {
                result.Add(item);
            }
            return result;
        }

        private static List<LogModelWrapper> PackLogModelWrapper(List<LogModel> ls)
        {
            List<LogModelWrapper> logs = new();
            foreach (var log in ls)
            {
                logs.Add(new LogModelWrapper(log));
            }
            return logs;
        }

        private void AutoScroll_Toggled(object sender, RoutedEventArgs e)
        {
            UIConfig.Instance.LogAutoScroll = AutoScroll.IsOn;
            UIConfig.Instance.SetConfig("LogAutoScroll", AutoScroll.IsOn);
            SelectLastLog();
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
            FilterLogLevel = FilterLogLevelSelector.SelectedIndex * 10;
            var ls = LogHelper.GetDisplayLogs(FilterLogLevel, UIConfig.Instance.LogItemsCount);
            LogCollections = ConvertListToObservableCollection(PackLogModelWrapper(ls));
            RefilterLogCollection();
            SelectLastLog();
        }

        private void FilterTextValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchDebounceTimer.Stop();
            SearchDebounceTimer.Start();
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
        }

        private void ListViewItem_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed && SelectedLog != null)
            {
                Clipboard.SetText(SelectedLog.detail.ToString());
            }
        }

        private void LogHelper_LogAdded(int logId, LogModel log)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (log.priority >= FilterLogLevel
                    && log.status.Contains(SearchText) && log.detail.Contains(SearchText) && log.source.Contains(SearchText)
                    && log.name.Contains(SearchText))
                {
                    if (LogCollections.Count > 0 && LogCollections.Count > UIConfig.Instance.LogItemsCount)
                    {
                        LogCollections.RemoveAt(0);
                    }
                    LogCollections.Add(new LogModelWrapper(log));
                    SelectLastLog();
                }
                BalloonIcon tipIcon = BalloonIcon.Warning;
                if (log.priority == (int)LogLevel.Warning)
                {
                    tipIcon = BalloonIcon.Warning;
                }
                else if (log.priority >= (int)LogLevel.Error)
                {
                    tipIcon = BalloonIcon.Error;
                }
                if (log.priority >= (int)LogLevel.Warning && UIConfig.Instance.ShowBalloonTip)
                {
                    MainWindow.Instance.TaskbarIcon?.ShowBalloonTip(log.source, log.detail, tipIcon);
                }
            });
        }

        private void LogHelper_LogStatusUpdated(int logId, string status)
        {
            LogModelWrapper log = LogCollections.FirstOrDefault(x => x.id == logId);
            if (log != null)
            {
                log.status = status;
                log.InvokePropertyChanged("status");
            }
        }

        private void LogPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (FormLoaded)
            {
                SelectLastLog();
                return;
            }
            var ls = LogHelper.GetDisplayLogs(10, UIConfig.Instance.LogItemsCount);
            LogCollections = ConvertListToObservableCollection(PackLogModelWrapper(ls));
            ResizeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
            ResizeTimer.Tick += ResizeTimer_Tick;
            SearchDebounceTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
            SearchDebounceTimer.Tick += SearchDebounceTimer_Tick;
            LogHelper.LogAdded -= LogHelper_LogAdded;
            LogHelper.LogAdded += LogHelper_LogAdded;
            LogHelper.LogStatusUpdated -= LogHelper_LogStatusUpdated;
            LogHelper.LogStatusUpdated += LogHelper_LogStatusUpdated;
            AutoScroll.IsOn = UIConfig.Instance.LogAutoScroll;
            FormLoaded = true;
            SelectLastLog();
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

        private void SearchDebounceTimer_Tick(object? sender, EventArgs e)
        {
            SearchText = FilterTextValue.Text;
            RefilterLogCollection();
            SelectLastLog();
            SearchDebounceTimer.Stop();
        }
    }
}