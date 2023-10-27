using Another_Mirai_Native.Native;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Another_Mirai_Native.UI.Converters
{
    internal class ProxyEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CQPluginProxy proxy = (CQPluginProxy)value;
            if (proxy == null || (proxy.HasConnection && proxy.Enabled))
            {
                return "";
            }
            if (proxy.HasConnection is false)
            {
                return "🚫";
            }
            else if (proxy.Enabled is false)
            {
                return "❌";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}