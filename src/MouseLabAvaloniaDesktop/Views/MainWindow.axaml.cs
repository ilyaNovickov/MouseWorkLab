using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using MouseLabAvaloniaDesktop.ViewModels;

namespace MouseLabAvaloniaDesktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

#if DEBUG
        if (Design.IsDesignMode)
        {
            var vm = AppServices.Instance.GetRequiredService<MainViewModel>();
            if (vm is not null)
                Design.SetDataContext(this, vm);
        }
#endif
    }
}