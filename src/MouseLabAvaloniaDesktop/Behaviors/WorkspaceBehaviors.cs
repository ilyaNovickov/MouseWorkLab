using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
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

        if (e.Source is not (Canvas or Image))
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
    private Point _originPointer;
    private int _originX;
    private int _originY;
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
        _originPointer = e.GetPosition(canvas);
        _originX = node.X;
        _originY = node.Y;
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

        Vector offset = e.GetPosition(_canvas) - _originPointer;

        _worktop.MoveNodeFromOrigin(_node, _originX, _originY, offset);
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

public class ConnectionPointerBehavior : Behavior<Control>
{
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null)
            AssociatedObject.AddHandler(InputElement.PointerPressedEvent, OnPressed, RoutingStrategies.Tunnel);
    }

    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject is not null)
            AssociatedObject.RemoveHandler(InputElement.PointerPressedEvent, OnPressed);
    }

    private void OnPressed(object? sender, PointerPressedEventArgs e)
    {
        if (AssociatedObject is null)
            return;

        if (e.Source is Ellipse)
            return;

        if (!e.GetCurrentPoint(AssociatedObject).Properties.IsLeftButtonPressed)
            return;

        if (AssociatedObject.DataContext is not ConnectionViewModel connection)
            return;

        if (AssociatedObject.FindAncestorOfType<ItemsControl>()?.DataContext is not WorktopViewModel worktop)
            return;

        e.Handled = true;
        worktop.SelectConnection(connection);
    }
}

public class BezierHandleDragBehavior : Behavior<Control>
{
    public static readonly StyledProperty<int> HandleIndexProperty =
        AvaloniaProperty.Register<BezierHandleDragBehavior, int>(nameof(HandleIndex));

    public static readonly StyledProperty<bool> CanDragProperty =
        AvaloniaProperty.Register<BezierHandleDragBehavior, bool>(nameof(CanDrag));

    public int HandleIndex
    {
        get => GetValue(HandleIndexProperty);
        set => SetValue(HandleIndexProperty, value);
    }

    public bool CanDrag
    {
        get => GetValue(CanDragProperty);
        set => SetValue(CanDragProperty, value);
    }

    private ConnectionViewModel? _connection;
    private WorktopViewModel? _worktop;
    private Canvas? _canvas;
    private IPointer? _pointer;
    private Point _originPointer;
    private PointD _originControl;
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
        if (!CanDrag || AssociatedObject is null)
            return;

        if (!e.GetCurrentPoint(AssociatedObject).Properties.IsLeftButtonPressed)
            return;

        if (AssociatedObject.DataContext is not ConnectionViewModel connection)
            return;

        if (AssociatedObject.FindAncestorOfType<ItemsControl>()?.DataContext is not WorktopViewModel worktop)
            return;

        Canvas? canvas = AssociatedObject.FindAncestorOfType<Canvas>();
        if (canvas is null)
            return;

        e.Handled = true;

        _connection = connection;
        _worktop = worktop;
        _canvas = canvas;
        _pointer = e.Pointer;
        _originPointer = e.GetPosition(canvas);
        _originControl = HandleIndex == 0 ? connection.Control1 : connection.Control2;
        _captured = true;
        e.Pointer.Capture(AssociatedObject);
    }

    private void OnMoved(object? sender, PointerEventArgs e)
    {
        if (!_captured || _canvas is null || _connection is null || _worktop is null)
            return;

        if (!e.GetCurrentPoint(AssociatedObject).Properties.IsLeftButtonPressed)
        {
            Stop();
            return;
        }

        Vector offset = e.GetPosition(_canvas) - _originPointer;
        var point = new PointD(_originControl.X + offset.X, _originControl.Y + offset.Y);

        _worktop.SetConnectionControlPoint(_connection, HandleIndex, point);
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
        _connection = null;
        _worktop = null;
        _canvas = null;
    }
}
