using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.UI.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Another_Mirai_Native.UI.Pages
{
    /// <summary>
    /// Test_DebugLogPage.xaml 的交互逻辑
    /// </summary>
    public partial class Test_DebugLogPage : Page, INotifyPropertyChanged
    {
        public Test_DebugLogPage()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private ObservableCollection<LogModelWrapper> logCollections;

        public ObservableCollection<LogModelWrapper> LogCollections
        {
            get => logCollections;

            set
            {
                logCollections = value;
                OnPropertyChanged(nameof(LogCollections));
            }
        }

        public bool FormLoaded { get; private set; }
        public bool PageEnabled { get; private set; }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SelectLastLog();
            PageEnabled = AppConfig.Instance.DebugMode;
            OnPropertyChanged(nameof(PageEnabled));
            if (FormLoaded)
            {
                return;
            }
            FormLoaded = true;
            DataContext = this;
            LogCollections = new();
            foreach (var item in LogHelper.DebugLogs)
            {
                LogCollections.Add(new LogModelWrapper(item));
            }
            LogHelper.DebugLogAdded -= LogHelper_DebugLogAdded;
            LogHelper.DebugLogAdded += LogHelper_DebugLogAdded;
        }

        private void LogHelper_DebugLogAdded(LogModel logModel)
        {
            Dispatcher.BeginInvoke(() =>
            {
                LogCollections.Add(new LogModelWrapper(logModel));
            });

            SelectLastLog();
        }

        public void SelectLastLog()
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (LogCollections.Count > 0)
                {
                    LogView.SelectedItem = LogCollections.Last();
                    LogView.UpdateLayout();
                    LogView.ScrollIntoView(LogView.SelectedItem);
                }
            });
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            LogHelper.DebugLogs.Clear();
            LogCollections.Clear();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            string dir = Path.Combine("logs", "Debug");
            Directory.CreateDirectory(dir);
            StringBuilder sb = new StringBuilder();
            foreach (var item in LogCollections)
            {
                sb.AppendLine($"{Helper.TimeStamp2DateTime(item.time):G}\t{item.source}\t{item.name}\t{item.detail}");
            }
            string fileName = $"{DateTime.Now:yyyyMMddHHmmss}.log";
            File.WriteAllText(Path.Combine(dir, fileName), sb.ToString());
            DialogHelper.ShowSimpleDialog("保存调试日志", $"保存成功，路径: {Path.Combine(dir, fileName)}");
        }
    }
}
