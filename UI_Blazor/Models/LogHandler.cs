using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using System.Collections.ObjectModel;

namespace Another_Mirai_Native.BlazorUI.Models
{
    public static class LogHandler
    {
        public static ObservableCollection<LogModel> Logs { get; set; }

        public static void StartSaveLogs()
        {
            Logs = [];
            foreach(var item in LogHelper.GetDisplayLogs((int)Model.Enums.LogLevel.Info, 100))
            {
                Logs.Add(item);
            }
            LogHelper.LogAdded += LogHelper_LogAdded;
        }

        public static void StopSaveLogs()
        {
            LogHelper.LogAdded -= LogHelper_LogAdded;
        }

        private static void LogHelper_LogAdded(int logId, LogModel log)
        {
            if (Logs.Count > 100)
            {
                Logs.RemoveAt(0);
            }
            Logs.Add(log);
        }
    }

    public class LogDisplay
    {
        public int Id { get; set; }

        public string Name { get; set; }
       
        public string Time { get; set; }
       
        public string Source { get; set; }
       
        public string Detail { get; set; }
       
        public string Status { get; set; }

        public int Priority { get; set; }

        public static LogDisplay ParseLogModel(LogModel item)
        {
            return new LogDisplay
            {
                Id = item.id,
                Detail = item.detail,
                Name = item.name,
                Source = item.source,
                Status = item.status,
                Priority = item.priority,
                Time = Helper.TimeStamp2DateTime(item.time).ToString("yyyy-MM-dd HH:mm:ss")
            };
        }
    }
}
