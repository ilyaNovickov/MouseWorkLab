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

        var start = ToAvaloniaPoint((Models.PointD)values[0]!);
        var end = ToAvaloniaPoint((Models.PointD)values[1]!);
        var cp1 = ToAvaloniaPoint((Models.PointD)values[2]!);
        var cp2 = ToAvaloniaPoint((Models.PointD)values[3]!);
        var kind = (Models.ConnectionKinds)values[4]!;

        var sg = new StreamGeometry();
        using var ctx = sg.Open();

        ctx.BeginFigure(start, false);

        if (kind == Models.ConnectionKinds.Curved)
            ctx.CubicBezierTo(cp1, cp2, end);
        else
            ctx.LineTo(end);

        return sg;
    }

    private static Point ToAvaloniaPoint(Models.PointD p)
    {
        return new Point(p.X, p.Y);
    }

}
