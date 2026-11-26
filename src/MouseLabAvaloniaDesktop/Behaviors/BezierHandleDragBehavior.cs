using Avalonia;
using Avalonia.Controls;
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
}
