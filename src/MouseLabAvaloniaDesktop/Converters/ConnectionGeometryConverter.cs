using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using MouseLabAvaloniaDesktop.ViewModels.Worktop.NodesAndMore;

namespace MouseLabAvaloniaDesktop.Converters;

/// <summary>
/// Конвертирует (Start, End, CP1, CP2, Kind) → StreamGeometry.
/// </summary>
public class ConnectionGeometryConverter : IMultiValueConverter
{
    private static ConnectionGeometryConverter? _instance;
    public static ConnectionGeometryConverter Instance => _instance ??= new();

    public object? Convert(IList<object?> values, Type targetType,
                           object? parameter, CultureInfo culture)
    {
        if (values.Count < 5) return null;
        if (values.Any(v => v is UnsetValueType)) return null;

        var start = ToAvaloniaPoint((MouseLabAvaloniaDesktop.Models.Point)values[0]!);
        var end = ToAvaloniaPoint((MouseLabAvaloniaDesktop.Models.Point)values[1]!);
        var cp1 = ToAvaloniaPoint((MouseLabAvaloniaDesktop.Models.Point)values[2]!);
        var cp2 = ToAvaloniaPoint((MouseLabAvaloniaDesktop.Models.Point)values[3]!);
        var kind = (ConnectionKinds)values[4]!;

        var sg = new StreamGeometry();
        using var ctx = sg.Open();

        ctx.BeginFigure(start, false);

        if (kind == ConnectionKinds.Curved)
            ctx.CubicBezierTo(cp1, cp2, end);
        else
            ctx.LineTo(end);

        return sg;
    }

    private static Point ToAvaloniaPoint(MouseLabAvaloniaDesktop.Models.Point p)
    {
        return new Point(p.X, p.Y);
    }

}

/// <summary>
/// Конвертирует (ArrowFrom, ArrowTo) → треугольник-стрелка.
/// </summary>
public class ArrowGeometryConverter : IMultiValueConverter
{
    private static ArrowGeometryConverter? _instance;
    public static ArrowGeometryConverter Instance => _instance ??= new();

    private const double ArrowLength = 14;
    private const double ArrowWidth = 9;

    public object? Convert(IList<object?> values, Type targetType,
                           object? parameter, CultureInfo culture)
    {
        if (values.Count < 2) return null;
        if (values.Any(v => v is UnsetValueType)) return null;

        var from = ToAvaloniaPoint((MouseLabAvaloniaDesktop.Models.Point)values[0]!);
        var to = ToAvaloniaPoint((MouseLabAvaloniaDesktop.Models.Point)values[1]!);

        double dx = to.X - from.X;
        double dy = to.Y - from.Y;
        double len = Math.Sqrt(dx * dx + dy * dy);
        if (len < 1) return null;

        double ux = dx / len, uy = dy / len;
        double px = -uy, py = ux;

        var tip = to;
        var left = new Point(
            to.X - ux * ArrowLength + px * ArrowWidth / 2,
            to.Y - uy * ArrowLength + py * ArrowWidth / 2);
        var right = new Point(
            to.X - ux * ArrowLength - px * ArrowWidth / 2,
            to.Y - uy * ArrowLength - py * ArrowWidth / 2);

        var sg = new StreamGeometry();
        using var ctx = sg.Open();
        ctx.BeginFigure(tip, true);
        ctx.LineTo(left);
        ctx.LineTo(right);
        ctx.EndFigure(true);

        return sg;
    }

    private static Point ToAvaloniaPoint(MouseLabAvaloniaDesktop.Models.Point p)
    {
        return new Point(p.X, p.Y);
    }
}
