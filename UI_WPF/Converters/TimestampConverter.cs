using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Another_Mirai_Native.UI.Converters
{
    internal class TimestampConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Helper.TimeStamp2DateTime((long)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class DisplayTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var time = (DateTime)value;
            if ((DateTime.Now - time).Days < 1 && DateTime.Now.Day == time.Day)
            {
                return time.ToString("T");
            }
            else if ((DateTime.Now - time).Days <= 7)
            {
                return time.ToString("dddd");
            }
            else
            {
                return time.ToString("D");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}