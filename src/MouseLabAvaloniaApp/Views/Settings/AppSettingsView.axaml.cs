using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using MouseLabAvaloniaApp.ViewModels;
using MouseLabAvaloniaApp.ViewModels.Settings;

namespace MouseLabAvaloniaApp.Views;

public partial class AppSettingsView : UserControl
{
    public AppSettingsView()
    {
        InitializeComponent();

#if DEBUG
        if (Design.IsDesignMode)
        {
            Design.SetDataContext(this, AppServices.Instance.Provider.GetRequiredService<AppSettingsViewModel>());
        }
#endif
    }
}