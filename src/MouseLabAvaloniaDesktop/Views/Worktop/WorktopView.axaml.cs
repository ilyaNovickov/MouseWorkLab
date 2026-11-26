using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
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
#if DEBUG
            saveDebugImageButton.IsVisible = true;
#endif
        }

        private void SaveDebugImage_Click(object? sender, RoutedEventArgs e)
        {
#if DEBUG
            Size size = mainCanvas.Bounds.Size;
            if (size.Width < 1d || size.Height < 1d)
                return;
            int m = 100;
            using var bitmap = new RenderTargetBitmap(
                new PixelSize((int)size.Width * m, (int)size.Height * m), new Vector(96 * m, 96 * m));
            bitmap.Render(mainCanvas);

            string directory = Path.Combine(Path.GetTempPath(), "MouseLab");
            Directory.CreateDirectory(directory);
            bitmap.Save(Path.Combine(directory, "worktop-debug.png"),
                        PngBitmapEncoderOptions.Default);
#endif
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

        private void WorktopVM_WorktopChanged(object? sender, EventArgs e) =>
            worktop.InvalidateVisual();

        private void this_DataContexChanged(object? sender, EventArgs e)
        {
            if (this.DataContext != _worktopVM)
            {
                if (_worktopVM is not null)
                {
                    _worktopVM.WorktopChanged -= WorktopVM_WorktopChanged;
                    _worktopVM.Dispose();
                }

                if (this.DataContext is not WorktopViewModel worktopViewModel)
                {
                    _worktopVM = null;
                    return;
                }
                _worktopVM = worktopViewModel;
                _worktopVM.WorktopChanged += WorktopVM_WorktopChanged;
            }
        }
    }
}
