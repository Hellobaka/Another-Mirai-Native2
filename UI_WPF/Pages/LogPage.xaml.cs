using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            DataContext = this;
        }

        public ObservableCollection<LogModel> LogCollections { get; set; } = new();

        private void LogPage_Loaded(object sender, RoutedEventArgs e)
        {
            LogHelper.LogAdded -= LogHelper_LogAdded;
            LogHelper.LogAdded += LogHelper_LogAdded;
            LogHelper.LogStatusUpdated -= LogHelper_LogStatusUpdated;
            LogHelper.LogStatusUpdated += LogHelper_LogStatusUpdated;
        }

        private void LogHelper_LogStatusUpdated(int logId, string status)
        {
            var log = LogCollections.FirstOrDefault(x => x.id == logId);
            if (log != null)
            {
                log.status = status;
            }
        }

        private void LogHelper_LogAdded(int logId, LogModel log)
        {
            Dispatcher.Invoke(() => LogCollections.Add(log));
        }
    }
}