using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;
using MouseLabAvaloniaDesktop.ViewModels;
using MouseLabAvaloniaDesktop.ViewModels.Worktop;

namespace MouseLabAvaloniaDesktop.Views
{
    public partial class WorktopView : UserControl
    {
        public WorktopView()
        {
            InitializeComponent();
            this.DataContextChanged += this_DataContexChanged;
#if DEBUG
            if (Design.IsDesignMode)
            {
                var vm = AppServices.Instance.GetRequiredService<WorktopViewModel>();
                if (vm is not null)
                    Design.SetDataContext(this, vm);
            }
            else
#endif
                this.worktop.PropertyChanged += Image_PropertyChanged;
        }

        private void Image_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property.Name != Image.SourceProperty.Name)
            {
                return;
            }

            if (e.OldValue is WriteableBitmap bitmap)
            {
                bitmap.Dispose();

            }

            // Не сбрасывает масштаб
            //zoomBorder.FitCommand.Execute(null);
        }

        private WorktopViewModel? _worktopVM;

        private void this_DataContexChanged(object? sender, EventArgs e)
        {
            if (this.DataContext != _worktopVM)
            {
                _worktopVM?.Dispose();

                if (this.DataContext is not WorktopViewModel worktopViewModel)
                {
                    _worktopVM = null;
                    return;
                }
                _worktopVM = worktopViewModel;
            }
        }

        private void upperCanvas_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            if (!e.Properties.IsLeftButtonPressed)
                return;

            var point = e.GetCurrentPoint(this.zoomBorder).Position;

            Point contentPoint = zoomBorder.ViewportToContent(point);

            _worktopVM?.AddNodeCommand.Execute(
                new MouseLabAvaloniaDesktop.Models.Point((int)contentPoint.X, (int)contentPoint.Y));
        }
    }
}