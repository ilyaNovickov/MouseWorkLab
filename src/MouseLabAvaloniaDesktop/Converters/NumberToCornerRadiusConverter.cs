using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace MouseLabAvaloniaDesktop.Converters;

public class NumberToCornerRadiusConverter : IValueConverter
{
    private static NumberToCornerRadiusConverter? _instance;

    public static NumberToCornerRadiusConverter Instance => _instance ??= new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
            throw new ArgumentException();

        double radius = 0d;

        if (value is double or float)
            radius = (double)value;
        else if (value is int)
            radius = System.Convert.ToDouble(value);
        else
            throw new Exception();

        return new CornerRadius(radius);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
