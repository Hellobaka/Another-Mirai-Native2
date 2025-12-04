using Another_Mirai_Native.Model.Enums;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Another_Mirai_Native.UI.Converters
{
    public class LogPriorityToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int priority)
            {
                return (LogLevel)priority switch
                {
                    LogLevel.Warning => Brushes.Orange,
                    LogLevel.Error => Brushes.Red,
                    LogLevel.Fatal => Brushes.DarkRed,
                    LogLevel.Debug => Brushes.LightGray,
                    LogLevel.InfoReceive => Brushes.DodgerBlue,
                    LogLevel.InfoSend => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF31C279")),
                    LogLevel.InfoSuccess => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB402B4")),
                    _ => Application.Current.FindResource("TextControlForeground"),
                };
            }
            return Application.Current.FindResource("TextControlForeground");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
