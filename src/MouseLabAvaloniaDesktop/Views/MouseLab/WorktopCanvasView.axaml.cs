using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using MouseLabAvaloniaDesktop.ViewModels;

namespace MouseLabAvaloniaDesktop.Views;

public partial class WorktopCanvasView : UserControl
{
    public WorktopCanvasView()
    {
        InitializeComponent();
#if DEBUG
        if (Design.IsDesignMode)
        {
            var vm = AppServices.Instance.GetRequiredService<WorktopCanvasViewModel>();
            if (vm is not null)
                Design.SetDataContext(this, vm);
        }
#endif
    }
}