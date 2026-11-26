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
    public class PointDToPointConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is PointD point ? ToAvaloniaPoint(point) : null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is Point point ? new PointD(point.X, point.Y) : null;
        }

        private static Point ToAvaloniaPoint(PointD p)
        {
            return new Point(p.X, p.Y);
        }
    }
}
