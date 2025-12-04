using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.UI.Models;
using Hardcodet.Wpf.TaskbarNotification;
using PropertyChanged;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace Another_Mirai_Native.UI.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class LogPageViewModel
    {
        public LogPageViewModel()
        {
            Instance = this;
            LogCollections = new ObservableCollection<LogModel>();
            PageSizes = new ObservableCollection<int> { 100, 200, 500, 1000, 2000, 5000 };
            SearchDebounceTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            SearchDebounceTimer.Tick += SearchDebounceTimer_Tick;
            
            _batchTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
            _batchTimer.Tick += BatchTimer_Tick;
            _batchTimer.Start();

            NextPageCommand = new RelayCommand(NextPage);
            PreviousPageCommand = new RelayCommand(PreviousPage);
            FirstPageCommand = new RelayCommand(FirstPage);
            LastPageCommand = new RelayCommand(LastPage);
        }

        public static LogPageViewModel Instance { get; set; }

        public event Action RequestScrollToBottom;

        public bool AutoScroll { get; set; }

        public int FilterLogLevel { get; set; } = 10;

        public int FilterLogLevelIndex { get; set; } = 1;

        public ObservableCollection<LogModel> LogCollections { get; set; }

        public string SearchText { get; set; } = "";

        public LogModel? SelectedLog { get; set; }
        
        public int CurrentPage { get; set; } = 1;

        public int TotalItems { get; set; }

        public int TotalPages { get; set; }

        public int ItemsPerPage { get; set; } = 500;
        
        public ObservableCollection<int> PageSizes { get; set; }

        public RelayCommand NextPageCommand { get; set; }
        public RelayCommand PreviousPageCommand { get; set; }
        public RelayCommand FirstPageCommand { get; set; }
        public RelayCommand LastPageCommand { get; set; }

        private bool FormLoaded { get; set; }

        private DispatcherTimer SearchDebounceTimer { get; set; }
        
        private DispatcherTimer _batchTimer;
        private ConcurrentQueue<LogModel> _pendingLogs = new();

        private bool _isRefiltering;

        public void Load()
        {
            if (FormLoaded)
            {
                SelectLastLog();
                return;
            }

            AutoScroll = UIConfig.Instance.LogAutoScroll;
            
            int configItemsCount = UIConfig.Instance.LogPageSize;
            if (!PageSizes.Contains(configItemsCount))
            {
                PageSizes.Add(configItemsCount);
                var sorted = PageSizes.OrderBy(x => x).ToList();
                PageSizes.Clear();
                foreach (var item in sorted)
                {
                    PageSizes.Add(item);
                }
            }
            ItemsPerPage = configItemsCount;

            FilterLogLevelIndex = 1; // Default Info

            CurrentPage = -1; // Start at last page
            RefilterLogCollection();

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
            if (AutoScroll && CurrentPage != TotalPages)
            {
                CurrentPage = TotalPages;
            }
            SelectLastLog();
        }
        
        public void OnItemsPerPageChanged()
        {
            if (FormLoaded)
            {
                UIConfig.Instance.LogPageSize = ItemsPerPage;
                UIConfig.Instance.SetConfig("LogPageSize", ItemsPerPage);
                
                CurrentPage = -1;
                RefilterLogCollection();
            }
        }

        public void OnCurrentPageChanged()
        {
            if (FormLoaded)
            {
                RefilterLogCollection();
            }
        }

        public void OnFilterLogLevelIndexChanged()
        {
            FilterLogLevel = FilterLogLevelIndex * 10;
            CurrentPage = -1;
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
            if (_isRefiltering) return;
            _isRefiltering = true;
            
            Task.Run(() =>
            {
                try
                {
                    int totalCount, totalPage;
                    var ls = LogHelper.DetailQueryLogs(FilterLogLevel, CurrentPage, ItemsPerPage, SearchText, out totalCount, out totalPage, null, null, true);
                    
                    Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        TotalItems = totalCount;
                        TotalPages = totalPage;
                        if (CurrentPage == -1)
                        {
                            CurrentPage = TotalPages;
                        }
                        UpdateLogCollections(ls);
                        SelectLastLog();
                    });
                }
                finally
                {
                    _isRefiltering = false;
                }
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
            _pendingLogs.Enqueue(log);
        }

        private void BatchTimer_Tick(object? sender, EventArgs e)
        {
            if (_pendingLogs.IsEmpty) return;

            var newLogs = new List<LogModel>();
            while (_pendingLogs.TryDequeue(out var log))
            {
                newLogs.Add(log);
            }

            ProcessNewLogs(newLogs);
        }

        private void ProcessNewLogs(List<LogModel> logs)
        {
            bool collectionChanged = false;
            bool shouldScroll = false;

            foreach (var log in logs)
            {
                if (log.priority >= FilterLogLevel
                    && (string.IsNullOrEmpty(SearchText) || (log.status.Contains(SearchText) && log.detail.Contains(SearchText) && log.source.Contains(SearchText)
                    && log.name.Contains(SearchText))))
                {
                    TotalItems++;
                    int newTotalPages = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);
                    if (newTotalPages == 0) newTotalPages = 1;

                    bool wasOnLastPage = (CurrentPage == TotalPages);
                    TotalPages = newTotalPages;

                    if (AutoScroll && wasOnLastPage)
                    {
                        if (CurrentPage != TotalPages)
                        {
                            CurrentPage = TotalPages; // This will trigger OnCurrentPageChanged -> Refilter
                            // If we change page, we don't need to manually add logs to current collection as Refilter will reload
                            // But we should continue processing other logs? 
                            // Actually if we change page, Refilter is async/queued.
                            // We should probably stop processing this batch for UI update purposes, 
                            // but we still need to process BalloonTips.
                            // Let's just set a flag and continue loop for BalloonTips.
                            collectionChanged = true; // Triggered via property change
                        }
                        else
                        {
                            // Still on last page
                            if (LogCollections.Count < ItemsPerPage)
                            {
                                log.detail = log.detail.Clean();
                                LogCollections.Add(log);
                                shouldScroll = true;
                            }
                        }
                    }
                    else
                    {
                        // AutoScroll OFF or Not on Last Page
                        // If we are on the last page (and it didn't change), we can add
                        if (CurrentPage == TotalPages && LogCollections.Count < ItemsPerPage)
                        {
                            log.detail = log.detail.Clean();
                            LogCollections.Add(log);
                            // Don't scroll automatically if AutoScroll is off
                        }
                    }
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
            }

            if (shouldScroll && AutoScroll)
            {
                SelectLastLog();
            }
        }

        private void LogHelper_LogStatusUpdated(int logId, string status)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                LogModel? log = LogCollections.FirstOrDefault(x => x.id == logId);
                if (log != null)
                {
                    log.status = status;
                }
            });
        }

        private void SearchDebounceTimer_Tick(object? sender, EventArgs e)
        {
            SearchDebounceTimer.Stop();
            CurrentPage = -1;
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
        
        private void NextPage(object? obj)
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
            }
        }

        private void PreviousPage(object? obj)
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
            }
        }

        private void FirstPage(object? obj)
        {
            CurrentPage = 1;
        }

        private void LastPage(object? obj)
        {
            CurrentPage = TotalPages;
        }
    }
}