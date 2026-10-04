using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using MouseLabAvaloniaApp.ViewModels.Settings;

namespace MouseLabAvaloniaApp.Views;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();

#if DEBUG
        if (Design.IsDesignMode)
        {
            Design.SetDataContext(this, AppServices.Instance.Provider.GetRequiredService<SettingsWindowViewModel>());
        }
#endif
    }

    private void cancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close();
    }

}