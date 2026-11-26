using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using MouseLabAvaloniaDesktop.Models;

namespace MouseLabAvaloniaDesktop.ViewModels.Worktop.NodesAndMore;

public partial class ConnectionViewModel : ViewModelBase, IDisposable
{
    public ConnectionViewModel(NodeViewModel source, NodeViewModel target)
    {
        Source = source;
        Target = target;

        // Подписка на изменение позиций узлов
        Source.PropertyChanged += OnNodePropertyChanged;
        Target.PropertyChanged += OnNodePropertyChanged;

        Recompute();
    }
    
    /// <summary>
    /// Узел от куда идёт соединение
    /// </summary>
    public NodeViewModel Source { get; }

    /// <summary>
    /// Узел к которому идёт соединение
    /// </summary>
    public NodeViewModel Target { get; }

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private bool _canInteract;

    [ObservableProperty]
    private bool _canDelete;

    [ObservableProperty]
    private ConnectionKinds _kind;

    [ObservableProperty]
    private double _curvature = 0.5;

    // Чистые данные
    [ObservableProperty]
    private PointD _start;

    [ObservableProperty]
    private PointD _end;

    [ObservableProperty]
    private PointD _control1;

    [ObservableProperty]
    private PointD _control2;

    // Для стрелки: откуда идёт касательная к концу
    [ObservableProperty]
    private PointD _arrowFrom;

    [ObservableProperty]
    private PointD _arrowTo;

    private bool _manualControlPoints;

    public bool HasManualControlPoints => _manualControlPoints;

    public bool ShowHandles => IsSelected && Kind == ConnectionKinds.Curved;

    public void SetControlPoint(int index, PointD point)
    {
        if (Kind != ConnectionKinds.Curved)
            return;

        if (index == 0)
            Control1 = point;
        else if (index == 1)
            Control2 = point;
        else
            return;

        _manualControlPoints = true;
        Recompute();
    }

    private void SetCurvedArrow(PointD start, PointD targetCenter, PointD end, double nodeRadius)
    {
        const double arrowDirectionLength = 10d;
        double radiusSquared = nodeRadius * nodeRadius;

        double CircleDistance(double t)
        {
            PointD point = BezierPoint(start, Control1, Control2, end, t);
            double dx = point.X - targetCenter.X;
            double dy = point.Y - targetCenter.Y;
            return dx * dx + dy * dy - radiusSquared;
        }

        double crossing = -1d;
        double previousT = 0d;
        double previousDistance = CircleDistance(0d);

        for (int i = 1; i <= 32; i++)
        {
            double t = i / 32d;
            double distance = CircleDistance(t);

            if (previousDistance >= 0d && distance < 0d)
            {
                double low = previousT;
                double high = t;

                for (int j = 0; j < 24; j++)
                {
                    double middle = (low + high) * 0.5d;
                    if (CircleDistance(middle) >= 0d)
                        low = middle;
                    else
                        high = middle;
                }

                crossing = (low + high) * 0.5d;
            }

            previousT = t;
            previousDistance = distance;
        }

        PointD tip;
        PointD direction;

        if (crossing >= 0d)
        {
            tip = BezierPoint(start, Control1, Control2, end, crossing);
            direction = BezierTangent(start, Control1, Control2, end, crossing);
        }
        else
        {
            double vx = targetCenter.X - end.X;
            double vy = targetCenter.Y - end.Y;
            double length = Math.Sqrt(vx * vx + vy * vy);
            if (length < 1e-6)
            {
                vx = 1d;
                vy = 0d;
                length = 1d;
            }

            direction = new PointD(vx / length, vy / length);
            tip = new PointD(targetCenter.X - direction.X * nodeRadius,
                             targetCenter.Y - direction.Y * nodeRadius);
        }

        double directionLength = Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
        if (directionLength < 1e-6)
        {
            direction = new PointD(1d, 0d);
            directionLength = 1d;
        }

        var unit = new PointD(direction.X / directionLength, direction.Y / directionLength);
        if (unit.X * (targetCenter.X - tip.X) + unit.Y * (targetCenter.Y - tip.Y) < 0d)
            unit = new PointD(-unit.X, -unit.Y);

        ArrowFrom = new PointD(tip.X - unit.X * arrowDirectionLength,
                               tip.Y - unit.Y * arrowDirectionLength);
        ArrowTo = tip;
    }

    private static PointD BezierPoint(PointD p0, PointD p1, PointD p2, PointD p3, double t)
    {
        double u = 1d - t;
        double a = u * u * u;
        double b = 3d * u * u * t;
        double c = 3d * u * t * t;
        double d = t * t * t;

        return new PointD(
            p0.X * a + p1.X * b + p2.X * c + p3.X * d,
            p0.Y * a + p1.Y * b + p2.Y * c + p3.Y * d);
    }

    private static PointD BezierTangent(PointD p0, PointD p1, PointD p2, PointD p3, double t)
    {
        double u = 1d - t;
        double a = 3d * u * u;
        double b = 6d * u * t;
        double c = 3d * t * t;

        return new PointD(
            (p1.X - p0.X) * a + (p2.X - p1.X) * b + (p3.X - p2.X) * c,
            (p1.Y - p0.Y) * a + (p2.Y - p1.Y) * b + (p3.Y - p2.Y) * c);
    }

    private void OnNodePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(NodeViewModel.X) or nameof(NodeViewModel.Y))
            Recompute();
    }

    private void Recompute()
    {
        const double nodeRadius = NodeViewModel.DefaultSize / 2d;
        const double connectorOverlap = 1d;
        const double arrowDirectionLength = 10d;

        var s = new PointD(Source.X + nodeRadius, Source.Y + nodeRadius);
        var t = new PointD(Target.X + nodeRadius, Target.Y + nodeRadius);

        double dx = t.X - s.X;
        double dy = t.Y - s.Y;
        double len = Math.Sqrt(dx * dx + dy * dy);

        double ux = len > 0d ? dx / len : 1d;
        double uy = len > 0d ? dy / len : 0d;

        if (len < nodeRadius * 2)
        {
            Start = s;
            End = t;
            if (!_manualControlPoints)
            {
                Control1 = s;
                Control2 = t;
            }
            if (Kind == ConnectionKinds.Curved)
            {
                SetCurvedArrow(Start, t, End, nodeRadius);
            }
            else
            {
                ArrowFrom = new PointD(t.X - ux * arrowDirectionLength,
                                       t.Y - uy * arrowDirectionLength);
                ArrowTo = t;
            }
            return;
        }

        double connectorRadius = nodeRadius - connectorOverlap;

        Start = new PointD(s.X + ux * connectorRadius,
                           s.Y + uy * connectorRadius);
        End = new PointD(t.X - ux * connectorRadius,
                         t.Y - uy * connectorRadius);

        PointD arrowTip = new(t.X - ux * nodeRadius, t.Y - uy * nodeRadius);


        if (Kind == ConnectionKinds.Curved)
        {
            double nx = -uy, ny = ux;
            double offset = len * Curvature;

            if (!_manualControlPoints)
            {
                Control1 = new PointD(
                    Start.X + dx * 0.4 + nx * offset * 0.25,
                    Start.Y + dy * 0.4 + ny * offset * 0.25);
                Control2 = new PointD(
                    End.X + dx * (-0.4) + nx * offset * 0.25,
                    End.Y + dy * (-0.4) + ny * offset * 0.25);
            }

            SetCurvedArrow(Start, t, End, nodeRadius);
        }
        else
        {
            if (!_manualControlPoints)
            {
                Control1 = Start;
                Control2 = End;
            }
            ArrowFrom = new PointD(arrowTip.X - ux * arrowDirectionLength,
                                   arrowTip.Y - uy * arrowDirectionLength);
            ArrowTo = arrowTip;
        }
    }

    partial void OnKindChanged(ConnectionKinds value)
    {
        Recompute();
        OnPropertyChanged(nameof(ShowHandles));
    }

    partial void OnCurvatureChanged(double value) => Recompute();

    partial void OnIsSelectedChanged(bool value) =>
        OnPropertyChanged(nameof(ShowHandles));

    public void Dispose()
    {
        Source.PropertyChanged -= OnNodePropertyChanged;
        Target.PropertyChanged -= OnNodePropertyChanged;
    }

    public object ToModel()
    {
        return new object();
    }
}


