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
    private Point _start;

    [ObservableProperty]
    private Point _end;

    [ObservableProperty]
    private Point _control1;

    [ObservableProperty]
    private Point _control2;

    // Для стрелки: откуда идёт касательная к концу
    [ObservableProperty]
    private Point _arrowFrom;

    [ObservableProperty]
    private Point _arrowTo;

    public bool ShowHandles => IsSelected && Kind == ConnectionKinds.Curved;

    private void OnNodePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(NodeViewModel.X) or nameof(NodeViewModel.Y))
            Recompute();
    }

    private void Recompute()
    {
        const double nodeRadius = 20;

        var s = new Point(Source.X, Source.Y);
        var t = new Point(Target.X, Target.Y);

        double dx = t.X - s.X;
        double dy = t.Y - s.Y;
        double len = Math.Sqrt(dx * dx + dy * dy);

        if (len < nodeRadius * 2)
        {
            Start = s;
            End = t;
            Control1 = s;
            Control2 = t;
            ArrowFrom = s;
            ArrowTo = t;
            return;
        }

        double ux = dx / len, uy = dy / len;

        Start = new Point((int)(s.X + ux * nodeRadius),
                          (int)(s.Y + uy * nodeRadius));
        End = new Point((int)(t.X - ux * nodeRadius),
                        (int)(t.Y - uy * nodeRadius));

        if (Kind == ConnectionKinds.Curved)
        {
            double nx = -uy, ny = ux;
            double offset = len * Curvature;

            Control1 = new Point(
                (int)(Start.X + dx * 0.4 + nx * offset * 0.25),
                (int)(Start.Y + dy * 0.4 + ny * offset * 0.25));
            Control2 = new Point(
                (int)(End.X + dx * (-0.4) + nx * offset * 0.25),
                (int)(End.Y + dy * (-0.4) + ny * offset * 0.25));

            ArrowFrom = Control2;
            ArrowTo = End;
        }
        else
        {
            Control1 = Start;
            Control2 = End;
            ArrowFrom = Start;
            ArrowTo = End;
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
