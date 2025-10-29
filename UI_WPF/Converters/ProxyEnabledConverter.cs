using Another_Mirai_Native.Config;
using Another_Mirai_Native.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            if (proxy.AppInfo.AuthCode == AppConfig.Instance.TestingAuthCode)
            {
                return "🧪";
            }
            if (proxy == null || (proxy.HasConnection && proxy.Enabled))
            {
                return "";
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

    public class EnumToItemsSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Type enumType = value is Array array && array.Length > 0 ? array.GetValue(0)?.GetType() : null;
            return enumType != null && enumType.IsEnum ? Enum.GetValues(enumType) : (object)null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            var type = value.GetType();
            if (!type.IsEnum)
            {
                return value.ToString();
            }

            var name = Enum.GetName(type, value);
            if (name == null)
            {
                return value.ToString();
            }

            var field = type.GetField(name);
            if (field == null)
            {
                return value.ToString();
            }

            return Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr ? attr.Description : name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !targetType.IsEnum)
            {
                return null;
            }

            foreach (var field in targetType.GetFields())
            {
                var attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if ((attr != null && attr.Description == value.ToString()) || field.Name == value.ToString())
                {
                    return Enum.Parse(targetType, field.Name);
                }
            }
            return null;
        }

    }
}