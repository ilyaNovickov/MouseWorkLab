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
    /// <summary>
    /// Конвертирует (ArrowFrom, ArrowTo) → треугольник-стрелка.
    /// </summary>
    public class ArrowGeometryConverter : IMultiValueConverter
    {
        private static ArrowGeometryConverter? _instance;
        public static ArrowGeometryConverter Instance => _instance ??= new();

        internal const double ArrowLength = 7d;
        internal const double ArrowWidth = 4.5d;

        public object? Convert(IList<object?> values, Type targetType,
                               object? parameter, CultureInfo culture)
        {
            if (values.Count < 2) return null;
            if (values.Any(v => v is UnsetValueType)) return null;

            var from = ToAvaloniaPoint((PointD)values[0]!);
            var to = ToAvaloniaPoint((PointD)values[1]!);

            return CreateGeometry(from, to, 0d);
        }

        public static (Point Tip, Point Left, Point Right)? TryBuildTriangle(
            Point from, Point to, double outline)
        {
            double dx = to.X - from.X;
            double dy = to.Y - from.Y;
            double len = Math.Sqrt(dx * dx + dy * dy);
            if (len < 1d)
                return null;

            double ux = dx / len;
            double uy = dy / len;
            double px = -uy;
            double py = ux;

            double length = ArrowLength + outline;
            double halfWidth = ArrowWidth / 2d + outline;

            var tip = new Point(to.X + ux * outline, to.Y + uy * outline);
            var left = new Point(to.X - ux * length + px * halfWidth,
                                 to.Y - uy * length + py * halfWidth);
            var right = new Point(to.X - ux * length - px * halfWidth,
                                  to.Y - uy * length - py * halfWidth);

            return (tip, left, right);
        }

        public static StreamGeometry? CreateGeometry(Point from, Point to, double outline)
        {
            var triangle = TryBuildTriangle(from, to, outline);
            if (triangle is null)
                return null;

            var sg = new StreamGeometry();
            using var ctx = sg.Open();
            ctx.BeginFigure(triangle.Value.Tip, true);
            ctx.LineTo(triangle.Value.Left);
            ctx.LineTo(triangle.Value.Right);
            ctx.EndFigure(true);

            return sg;
        }

        private static Point ToAvaloniaPoint(PointD p)
        {
            return new Point(p.X, p.Y);
        }
    }
}
