using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace LogicPlayground.Converters
{
    public class BoolToBackgroundConverter : IValueConverter
    {
        public static readonly BoolToBackgroundConverter Instance = new();

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isSelected)
            {
                return isSelected ? new SolidColorBrush(Color.Parse("#007ACC")) : Brushes.Transparent;
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
