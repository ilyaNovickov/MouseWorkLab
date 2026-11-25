using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using MouseLabAvaloniaDesktop.Models;

namespace MouseLabAvaloniaDesktop.ViewModels.Worktop.NodesAndMore;

public struct PointD
{
    public PointD()
    {

    }

    public PointD(double x, double y)
    {
        X = x;
        Y = y;
    }

    public double X { get; set; }
    public double Y { get; set; }
}

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

    public NodeViewModel Source { get; }
    public NodeViewModel Target { get; }

    [ObservableProperty]
    private bool _isSelected;

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

    private void SetCurvedArrow(PointD targetCenter, PointD end, double nodeRadius)
    {
        const double arrowDirectionLength = 10d;

        double vx = end.X - Control2.X;
        double vy = end.Y - Control2.Y;
        double vlen = Math.Sqrt(vx * vx + vy * vy);

        if (vlen < 1e-6)
        {
            ArrowFrom = end;
            ArrowTo = end;
            return;
        }

        double dx = vx / vlen;
        double dy = vy / vlen;

        double relX = end.X - targetCenter.X;
        double relY = end.Y - targetCenter.Y;
        double backX = -dx;
        double backY = -dy;
        double b = relX * backX + relY * backY;
        double c = relX * relX + relY * relY - nodeRadius * nodeRadius;
        double distance = -b + Math.Sqrt(Math.Max(0d, b * b - c));

        var tip = new PointD(end.X + backX * distance, end.Y + backY * distance);

        ArrowFrom = new PointD(tip.X - dx * arrowDirectionLength,
                               tip.Y - dy * arrowDirectionLength);
        ArrowTo = tip;
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
                SetCurvedArrow(t, End, nodeRadius);
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

            SetCurvedArrow(t, End, nodeRadius);
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

public enum ConnectionKinds
{
    Straight,
    Curved,
}
