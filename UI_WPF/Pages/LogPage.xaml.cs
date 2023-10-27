using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.UI;
using Another_Mirai_Native.Model.Enums;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Threading;

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
            LogCollections = new();
            DataContext = this;
            InitColumnWidth();
            foreach (var item in LogGridView.Columns)
            {
                var descriptor = DependencyPropertyDescriptor.FromProperty(GridViewColumn.WidthProperty, typeof(GridViewColumn));
                descriptor.AddValueChanged(item, ColumnWidthChanged);
            }
        }

        private DispatcherTimer ResizeTimer { get; set; }

        private void ColumnWidthChanged(object? sender, EventArgs e)
        {
            if (ResizeTimer == null)
            {
                return;
            }
            ResizeTimer.Stop();
            ResizeTimer.Start();
        }

        public List<LogModel> RawLogCollections { get; set; } = new();

        public ObservableCollection<LogModel> LogCollections { get; set; }

        private LogModel SelectedLog { get; set; }

        private void LogPage_Loaded(object sender, RoutedEventArgs e)
        {
            ResizeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
            ResizeTimer.Tick += ResizeTimer_Tick;
            LogHelper.LogAdded -= LogHelper_LogAdded;
            LogHelper.LogAdded += LogHelper_LogAdded;
            LogHelper.LogStatusUpdated -= LogHelper_LogStatusUpdated;
            LogHelper.LogStatusUpdated += LogHelper_LogStatusUpdated;
            AutoScroll.IsOn = UIConfig.LogAutoScroll;
        }

        private void InitColumnWidth()
        {
            for (int i = 0; i < LogGridView.Columns.Count; i++)
            {
                var column = LogGridView.Columns[i];
                column.Width = ConfigHelper.GetConfig($"LogColumn{i + 1}_Width", UIConfig.DefaultConfigPath, 200);
            }
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

        private void LogHelper_LogStatusUpdated(int logId, string status)
        {
            var log = RawLogCollections.FirstOrDefault(x => x.id == logId);
            if (log != null)
            {
                log.status = status;
            }
            RefilterLogCollection();
            LogView.SelectedItem = SelectedLog;
        }

        private void LogHelper_LogAdded(int logId, LogModel log)
        {
            RawLogCollections.Add(log);
            RefilterLogCollection();
            Dispatcher.Invoke(() =>
            {
                if (AutoScroll.IsOn)
                {
                    LogView.SelectedItem = LogCollections.Last();
                    LogView.ScrollIntoView(SelectedLog);
                }
                else
                {
                    LogView.SelectedItem = SelectedLog;
                }
            });
        }

        private void RefilterLogCollection()
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
                });
                LogCollections.Clear();
                foreach (var item in ls)
                {
                    LogCollections.Add(item);
                }
            });
        }

        private void FilterLogLevelSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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

        private void AutoScroll_Toggled(object sender, RoutedEventArgs e)
        {
            ConfigHelper.SetConfig("LogAutoScroll", AutoScroll.IsOn, UIConfig.DefaultConfigPath);
            if (AutoScroll.IsOn && LogCollections != null && LogCollections.Count > 0)
            {
                LogView.SelectedItem = LogCollections.Last();
                LogView.ScrollIntoView(SelectedLog);
            }
        }

        private void LogView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedLog = LogView.SelectedItem as LogModel;
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
    }
}