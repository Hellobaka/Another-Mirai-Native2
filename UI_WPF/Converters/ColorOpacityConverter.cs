using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Another_Mirai_Native.UI.Converters
{
    public class ColorOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                if (double.TryParse(parameter?.ToString(), out double opacity))
                {
                    // 创建一个新的SolidColorBrush，基于原有颜色但透明度调整
                    return new SolidColorBrush(new Color { A = (byte)(opacity * 255), R = brush.Color.R, G = brush.Color.G, B = brush.Color.B });
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
