using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Hardcodet.Wpf.TaskbarNotification;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Another_Mirai_Native.UI.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class LogPageViewModel
    {
        public LogPageViewModel()
        {
            Instance = this;
            LogCollections = [];
            SearchDebounceTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            SearchDebounceTimer.Tick += SearchDebounceTimer_Tick;
        }

        public event Action RequestScrollToBottom;

        public static LogPageViewModel Instance { get; set; }

        public bool AutoScroll { get; set; }

        public int FilterLogLevel { get; set; } = 10;

        public int FilterLogLevelIndex { get; set; } = 1;

        public ObservableCollection<LogModel> LogCollections { get; set; }

        public string SearchText { get; set; } = "";

        public LogModel? SelectedLog { get; set; }

        private bool FormLoaded { get; set; }

        private DispatcherTimer SearchDebounceTimer { get; set; }

        public void Load()
        {
            if (FormLoaded)
            {
                SelectLastLog();
                return;
            }

            AutoScroll = UIConfig.Instance.LogAutoScroll;
            FilterLogLevelIndex = 1; // Default Info

            var ls = LogHelper.GetDisplayLogs(FilterLogLevel, UIConfig.Instance.LogItemsCount);
            UpdateLogCollections(ls);

            LogHelper.LogAdded -= LogHelper_LogAdded;
            LogHelper.LogAdded += LogHelper_LogAdded;
            LogHelper.LogStatusUpdated -= LogHelper_LogStatusUpdated;
            LogHelper.LogStatusUpdated += LogHelper_LogStatusUpdated;

            FormLoaded = true;
            SelectLastLog();
        }

        public void OnAutoScrollChanged()
        {
            UIConfig.Instance.LogAutoScroll = AutoScroll;
            UIConfig.Instance.SetConfig("LogAutoScroll", AutoScroll);
            SelectLastLog();
        }

        public void OnFilterLogLevelIndexChanged()
        {
            FilterLogLevel = FilterLogLevelIndex * 10;
            var ls = LogHelper.GetDisplayLogs(FilterLogLevel, UIConfig.Instance.LogItemsCount);
            UpdateLogCollections(ls);
            RefilterLogCollection();
            SelectLastLog();
        }

        public void OnSearchTextChanged()
        {
            SearchDebounceTimer.Stop();
            SearchDebounceTimer.Start();
        }

        public void RefilterLogCollection()
        {
            var ls = LogHelper.DetailQueryLogs(FilterLogLevel, 1, UIConfig.Instance.LogItemsCount, SearchText, out _, out _, null, null);

            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                UpdateLogCollections(ls);
            });
        }

        public void SelectLastLog()
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                if (AutoScroll && MainWindow.Instance.LogMenuItem.IsSelected && LogCollections.Count > 0)
                {
                    SelectedLog = LogCollections.Last();
                    // ScrollIntoView needs to be handled by View
                    RequestScrollToBottom?.Invoke();
                }
            });
        }

        private void LogHelper_LogAdded(int logId, LogModel log)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                if (log.priority >= FilterLogLevel
                    && (string.IsNullOrEmpty(SearchText) || (log.status.Contains(SearchText) && log.detail.Contains(SearchText) && log.source.Contains(SearchText)
                    && log.name.Contains(SearchText))))
                {
                    if (LogCollections.Count > 0 && LogCollections.Count > UIConfig.Instance.LogItemsCount)
                    {
                        LogCollections.RemoveAt(0);
                    }
                    log.detail = log.detail.Clean();
                    LogCollections.Add(log);
                    SelectLastLog();
                }
            });
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

        }

        private void LogHelper_LogStatusUpdated(int logId, string status)
        {
            LogModel? log = LogCollections.FirstOrDefault(x => x.id == logId);
            if (log != null)
            {
                log.status = status;
            }
        }

        private void SearchDebounceTimer_Tick(object? sender, EventArgs e)
        {
            SearchDebounceTimer.Stop();
            RefilterLogCollection();
            SelectLastLog();
        }

        private void UpdateLogCollections(List<LogModel> list)
        {
            LogCollections.Clear();
            foreach (var item in list)
            {
                item.detail = item.detail.Clean();
                LogCollections.Add(item);
            }
        }
    }
}