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
        var settingsService = _appServices.GetRequiredService<ProjectService>();
        settingsService.UiSettings.PropertyChanged -= _uiChangedHandler;

        _appServices.Dispose();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        _appServices = AppServices.CreateDefault();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Exit += (_, _) => OnShutdown();

            var projectService = _appServices.GetRequiredService<ProjectService>();
            _uiChangedHandler = (object? s, PropertyChangedEventArgs e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(projectService.UiSettings.Culture):
                        // ??
                        var cultureService = _appServices.GetRequiredService<ICultureService>();
                        cultureService.SetCulture(UiSettings.ResolveCulture(projectService.UiSettings.Culture));
                        break;
                    case nameof(projectService.UiSettings.Theme):
                        RequestedThemeVariant = projectService.UiSettings.Theme switch
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
            projectService.UiSettings.PropertyChanged += _uiChangedHandler;

            desktop.MainWindow = new MainWindow
            {
                DataContext = _appServices.GetRequiredService<MainViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}