using Another_Mirai_Native.Native;
using System.ComponentModel;

namespace Another_Mirai_Native.UI.ViewModel
{
    public class CQPluginProxyWrapper : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public CQPluginProxy TargetPlugin { get; set; }

        public CQPluginProxyWrapper(CQPluginProxy proxy)
        {
            TargetPlugin = proxy;
        }

        public void InvokePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}