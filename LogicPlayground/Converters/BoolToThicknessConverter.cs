using Avalonia;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace LogicPlayground.Converters;

public class BoolToThicknessConverter : IValueConverter
{
    public static readonly BoolToThicknessConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue && parameter is string parameterString)
        {
            var options = parameterString.Split('|');
            if (options.Length == 2)
            {
                var targetOption = boolValue ? options[0] : options[1];
                var parts = targetOption.Split(',');
                if (parts.Length == 4)
                {
                    if (double.TryParse(parts[0], out var left) &&
                        double.TryParse(parts[1], out var top) &&
                        double.TryParse(parts[2], out var right) &&
                        double.TryParse(parts[3], out var bottom))
                    {
                        return new Thickness(left, top, right, bottom);
                    }
                }
            }
        }
        return new Thickness(10, 8, 10, 8);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
