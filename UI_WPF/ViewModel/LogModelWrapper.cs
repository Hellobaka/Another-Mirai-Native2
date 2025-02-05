using Another_Mirai_Native.Model;
using SqlSugar;
using System.ComponentModel;

namespace Another_Mirai_Native.UI.ViewModel
{
    public class LogModelWrapper : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public int id { get; set; }
        public long time { get; set; }
        public int priority { get; set; }
        public string source { get; set; } = "";
        public string status { get; set; } = "";
        public string name { get; set; } = "";
        public string detail { get; set; } = "";
        public string detailNoWrap
        {
            get
            {
                return detail.Replace("\r", " ").Replace("\n", " ");
            }
        }

        public LogModelWrapper(LogModel log)
        {
            log.detail = log.detail.Clean();
            id = log.id;
            time = log.time;
            priority = log.priority;
            source = log.source;
            status = log.status;
            name = log.name;
            detail = log.detail;
        }

        public void InvokePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return $"{detail} [{source}]";
        }
    }
}
