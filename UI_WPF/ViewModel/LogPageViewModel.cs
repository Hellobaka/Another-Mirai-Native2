using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.UI.Models;
using Hardcodet.Wpf.TaskbarNotification;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Another_Mirai_Native.UI;

namespace Another_Mirai_Native.UI.ViewModel
{
    public class LogPageViewModel : INotifyPropertyChanged
    {
        public LogPageViewModel()
        {
            LogCollections = [];
            SearchDebounceTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            SearchDebounceTimer.Tick += SearchDebounceTimer_Tick;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public event Action RequestScrollToBottom;

        public bool AutoScroll { get; set; }

        public int FilterLogLevel { get; set; } = 10;

        public int FilterLogLevelIndex { get; set; } = 1;

        public ObservableCollection<LogModelWrapper> LogCollections { get; set; }

        public string SearchText { get; set; } = "";

        public LogModelWrapper? SelectedLog { get; set; }

        private bool FormLoaded { get; set; }

        private DispatcherTimer SearchDebounceTimer { get; set; }

        public void InvokePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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
            UpdateLogCollections(PackLogModelWrapper(ls));

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
            UpdateLogCollections(PackLogModelWrapper(ls));
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
            var ls = PackLogModelWrapper(
                    LogHelper.DetailQueryLogs(FilterLogLevel, 1, UIConfig.Instance.LogItemsCount, SearchText, out _, out _, null, null)
                    );

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

        private static List<LogModelWrapper> PackLogModelWrapper(List<LogModel> ls)
        {
            List<LogModelWrapper> logs = new();
            foreach (var log in ls)
            {
                logs.Add(new LogModelWrapper(log));
            }
            return logs;
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
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                LogModelWrapper? log = LogCollections.FirstOrDefault(x => x.id == logId);
                if (log != null)
                {
                    log.status = status;
                    log.InvokePropertyChanged("status");
                }
            });
        }

        private void SearchDebounceTimer_Tick(object? sender, EventArgs e)
        {
            SearchDebounceTimer.Stop();
            RefilterLogCollection();
            SelectLastLog();
        }

        private void UpdateLogCollections(List<LogModelWrapper> list)
        {
            LogCollections.Clear();
            foreach (var item in list)
            {
                LogCollections.Add(item);
            }
        }
    }
}