using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using MouseLabAvaloniaDesktop.ViewModels.Worktop;
using System;
using System.Collections.Generic;
using System.Text;
using ModelsPoint = MouseLabAvaloniaDesktop.Models.Point;

namespace MouseLabAvaloniaDesktop.Behaviors
{
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
}
