using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using MouseLabAvaloniaDesktop.ViewModels.Worktop;
using MouseLabAvaloniaDesktop.ViewModels.Worktop.NodesAndMore;
using ModelsPoint = MouseLabAvaloniaDesktop.Models.Point;

namespace MouseLabAvaloniaDesktop.Behaviors;

public class CanvasPointerBehavior : Behavior<Control>
{
    public static readonly StyledProperty<ZoomBorder?> ViewportProperty =
        AvaloniaProperty.Register<CanvasPointerBehavior, ZoomBorder?>(nameof(Viewport));

    public ZoomBorder? Viewport
    {
        get => GetValue(ViewportProperty);
        set => SetValue(ViewportProperty, value);
    }

    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null)
            AssociatedObject.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies.Bubble);
    }

    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject is not null)
            AssociatedObject.RemoveHandler(InputElement.PointerPressedEvent, OnPointerPressed);
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (AssociatedObject is null)
            return;

        if (!e.GetCurrentPoint(AssociatedObject).Properties.IsLeftButtonPressed)
            return;

        if (e.Source is not Canvas)
            return;

        if (AssociatedObject.DataContext is not WorktopViewModel worktop || Viewport is null)
            return;

        Point viewportPoint = e.GetCurrentPoint(Viewport).Position;
        Point contentPoint = Viewport.ViewportToContent(viewportPoint);

        worktop.OnCanvasPressed(new ModelsPoint((int)contentPoint.X, (int)contentPoint.Y));
    }
}

public class NodeDragBehavior : Behavior<Control>
{
    public static readonly StyledProperty<bool> CanDragProperty =
        AvaloniaProperty.Register<NodeDragBehavior, bool>(nameof(CanDrag));

    public bool CanDrag
    {
        get => GetValue(CanDragProperty);
        set => SetValue(CanDragProperty, value);
    }

    private Canvas? _canvas;
    private NodeViewModel? _node;
    private WorktopViewModel? _worktop;
    private IPointer? _pointer;
    private Point _last;
    private bool _captured;

    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is null)
            return;

        AssociatedObject.AddHandler(InputElement.PointerPressedEvent, OnPressed, RoutingStrategies.Tunnel);
        AssociatedObject.AddHandler(InputElement.PointerMovedEvent, OnMoved, RoutingStrategies.Tunnel);
        AssociatedObject.AddHandler(InputElement.PointerReleasedEvent, OnReleased, RoutingStrategies.Tunnel);
        AssociatedObject.AddHandler(InputElement.PointerCaptureLostEvent, OnCaptureLost, RoutingStrategies.Tunnel);
    }

    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject is null)
            return;

        AssociatedObject.RemoveHandler(InputElement.PointerPressedEvent, OnPressed);
        AssociatedObject.RemoveHandler(InputElement.PointerMovedEvent, OnMoved);
        AssociatedObject.RemoveHandler(InputElement.PointerReleasedEvent, OnReleased);
        AssociatedObject.RemoveHandler(InputElement.PointerCaptureLostEvent, OnCaptureLost);
        Stop();
    }

    private void OnPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!CanDrag || !e.GetCurrentPoint(AssociatedObject).Properties.IsLeftButtonPressed)
            return;

        NodeViewModel? node = AssociatedObject?.DataContext as NodeViewModel;
        WorktopViewModel? worktop = AssociatedObject?.FindAncestorOfType<ItemsControl>()?.DataContext as WorktopViewModel;
        Canvas? canvas = AssociatedObject?.FindAncestorOfType<Canvas>();

        if (node is null || worktop is null || canvas is null || AssociatedObject is null)
            return;

        e.Handled = true;
        worktop.SelectNode(node);

        _node = node;
        _worktop = worktop;
        _canvas = canvas;
        _pointer = e.Pointer;
        _last = e.GetPosition(canvas);
        _captured = true;
        e.Pointer.Capture(AssociatedObject);
    }

    private void OnMoved(object? sender, PointerEventArgs e)
    {
        if (!_captured || _canvas is null || _node is null || _worktop is null)
            return;

        if (!e.GetCurrentPoint(AssociatedObject).Properties.IsLeftButtonPressed)
        {
            Stop();
            return;
        }

        Point current = e.GetPosition(_canvas);
        Vector delta = current - _last;
        _last = current;

        _worktop.MoveNode(_node, delta);
    }

    private void OnReleased(object? sender, PointerReleasedEventArgs e) => Stop();

    private void OnCaptureLost(object? sender, PointerCaptureLostEventArgs e) => Stop();

    private void Stop()
    {
        if (!_captured)
            return;

        _captured = false;
        _pointer?.Capture(null);
        _pointer = null;
        _node = null;
        _worktop = null;
        _canvas = null;
    }
}
