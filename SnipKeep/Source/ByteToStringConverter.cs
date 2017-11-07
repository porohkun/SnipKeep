using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace SnipKeep
{
    public class ByteToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte[] bytes)
                return Encoding.UTF8.GetString(bytes);
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
                return Encoding.UTF8.GetBytes(text);
            return new byte[0];
        }
    }
}
