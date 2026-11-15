using System;
using System.Globalization;
using Avalonia.Data.Converters;
using MouseLab.Core.Models;

namespace MouseLabAvaloniaDesktop.Converters;

public class ReadOnlyMatrixConverter : IValueConverter
{
    private static ReadOnlyMatrixConverter? _instance;
    public static ReadOnlyMatrixConverter Instance => _instance ??= new();
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not IReadOnlyMatrix matrix)
            throw new Exception();

        return new object();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
