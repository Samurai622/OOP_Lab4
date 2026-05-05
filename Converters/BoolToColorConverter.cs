using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace OOP_Lab4.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isTrue && isTrue)
            {
                return new SolidColorBrush(Color.Parse("#007ACC")); // Синій колір
            }
            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}