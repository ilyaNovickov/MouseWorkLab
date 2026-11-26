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

public class NodeDeleteBehavior : Behavior<Control>
{
    public static readonly StyledProperty<bool> CanDeleteProperty =
        AvaloniaProperty.Register<NodeDeleteBehavior, bool>(nameof(CanDelete));

    public bool CanDelete
    {
        get => GetValue(CanDeleteProperty);
        set => SetValue(CanDeleteProperty, value);
    }

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
        if (!CanDelete || AssociatedObject is null)
            return;

        if (!e.GetCurrentPoint(AssociatedObject).Properties.IsLeftButtonPressed)
            return;

        if (AssociatedObject.DataContext is not NodeViewModel node)
            return;

        if (AssociatedObject.FindAncestorOfType<ItemsControl>()?.DataContext is not WorktopViewModel worktop)
            return;

        e.Handled = true;
        worktop.DeleteNode(node);
    }
}
