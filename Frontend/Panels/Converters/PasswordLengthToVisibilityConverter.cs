using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MesseauftrittDatenerfassung_UI.Converters
{
    public class PasswordLengthToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value != null && value is int && (int)value > 0) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}