using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using MouseLabAvaloniaDesktop.ViewModels;

namespace MouseLabAvaloniaDesktop.Views;

public partial class AppearanceSettingsView : UserControl
{
    public AppearanceSettingsView()
    {
        InitializeComponent();
#if DEBUG
        if (Design.IsDesignMode)
        {
            var vm = AppServices.Instance.GetRequiredService<AppearanceSettingsViewModel>();
            if (vm is not null)
                Design.SetDataContext(this, vm);
        }
#endif
    }
}
