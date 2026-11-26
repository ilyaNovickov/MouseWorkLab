using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using MouseLabAvaloniaDesktop.ViewModels.Worktop;
using MouseLabAvaloniaDesktop.ViewModels.Worktop.NodesAndMore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvaloniaDesktop.Behaviors
{
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

            worktop.SelectConnection(connection);
        }
    }
}
