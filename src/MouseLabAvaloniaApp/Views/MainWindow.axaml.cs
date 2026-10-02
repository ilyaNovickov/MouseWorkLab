using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using MouseLabAvaloniaApp.ViewModels;
using MouseLabAvaloniaApp.ViewModels.Settings;

namespace MouseLabAvaloniaApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

#if DEBUG
        if (Design.IsDesignMode)
        {
            Design.SetDataContext(this, AppServices.Instance.Provider.GetRequiredService<MainWindowViewModel>());
        }
#endif
    }
}