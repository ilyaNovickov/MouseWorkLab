using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using MouseLabAvaloniaDesktop.Models.Settings;
using MouseLabAvaloniaDesktop.Services.Implementations;
using MouseLabAvaloniaDesktop.ViewModels;
using MouseLabAvaloniaDesktop.Views;
using ProTranslate;
using System.ComponentModel;

namespace MouseLabAvaloniaDesktop;

public partial class App : Application
{
    private AppServices _appServices = null!;
    PropertyChangedEventHandler _uiChangedHandler = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public void OnShutdown()
    {
        var settingsService = _appServices.GetRequiredService<SettingsService>();
        settingsService.Current.UiSettings.PropertyChanged -= _uiChangedHandler;

        _appServices.Dispose();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        _appServices = AppServices.CreateDefault();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Exit += (_, _) => OnShutdown();

            var settingsService = _appServices.GetRequiredService<SettingsService>();
            _uiChangedHandler = (object? s, PropertyChangedEventArgs e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(settingsService.Current.UiSettings.Culture):
                        // ??
                        var cultureService = _appServices.GetRequiredService<ICultureService>();
                        cultureService.SetCulture(UiSettings.ResolveCulture(settingsService.Current.UiSettings.Culture.Name));
                        break;
                    case nameof(settingsService.Current.UiSettings.Theme):
                        RequestedThemeVariant = settingsService.Current.UiSettings.Theme switch
                        {
                            Models.Themes.Light => Avalonia.Styling.ThemeVariant.Light,
                            Models.Themes.Dark => Avalonia.Styling.ThemeVariant.Dark,
                            Models.Themes.Default or _ => Avalonia.Styling.ThemeVariant.Default
                        };
                        break;
                    default:
                        break;
                }
            };
            settingsService.Current.UiSettings.PropertyChanged += _uiChangedHandler;

            desktop.MainWindow = new MainWindow
            {
                DataContext = _appServices.GetRequiredService<MainViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}