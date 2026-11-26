using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using MouseLabAvaloniaDesktop.ViewModels.Worktop.NodesAndMore;

namespace MouseLabAvaloniaDesktop.Converters
{
    public class ArrowBackGeometryConverter : IMultiValueConverter
    {
        private static ArrowBackGeometryConverter? _instance;
        public static ArrowBackGeometryConverter Instance => _instance ??= new();

        private const double DefaultOutline = 1.5d;

        public object? Convert(IList<object?> values, Type targetType,
                               object? parameter, CultureInfo culture)
        {
            if (values.Count < 2) return null;
            if (values.Any(v => v is UnsetValueType)) return null;

            var from = ToAvaloniaPoint((PointD)values[0]!);
            var to = ToAvaloniaPoint((PointD)values[1]!);

            return ArrowGeometryConverter.CreateGeometry(from, to, GetOutline(parameter));
        }

        public static double GetOutline(object? parameter)
        {
            if (parameter is double value)
                return value;

            if (parameter is string text
                && double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsed))
                return parsed;

            return DefaultOutline;
        }

        private static Point ToAvaloniaPoint(PointD p)
        {
            return new Point(p.X, p.Y);
        }
    }
}
