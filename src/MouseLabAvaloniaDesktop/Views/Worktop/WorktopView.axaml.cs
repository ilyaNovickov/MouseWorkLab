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
#if DEBUG
            if (Design.IsDesignMode)
            {
                var vm = AppServices.Instance.GetRequiredService<WorktopViewModel>();
                if (vm is not null)
                    Design.SetDataContext(this, vm);
            }
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
    }
}