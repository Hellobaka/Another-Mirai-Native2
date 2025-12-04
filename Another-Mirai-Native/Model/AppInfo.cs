using Newtonsoft.Json;
using PropertyChanged;
using System.ComponentModel;

namespace Another_Mirai_Native.Model
{
    public class AppInfo : INotifyPropertyChanged
    {
        [JsonIgnore]
        public int AuthCode { get; set; }

        public string AppId { get; set; } = "";

        public int LoaderType { get; set; }

        public int ret { get; set; }

        public int apiver { get; set; }

        public string name { get; set; }

        public string version { get; set; }

        public int version_id { get; set; }

        public string author { get; set; }

        public string description { get; set; }

        [JsonProperty("event")]
        public Event[] _event { get; set; }

        public Menu[] menu { get; set; }

        public object[] status { get; set; }

        public int[] auth { get; set; }

        public class Event
        {
            public int id { get; set; }

            public int type { get; set; }

            public string name { get; set; }

            public string function { get; set; }

            public int priority { get; set; }

            public int address { get; set; }
        }

        public class Menu
        {
            public string name { get; set; }

            public string function { get; set; }

            public int address { get; set; }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void InvokePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}