using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Microsoft.Extensions.DependencyInjection;
using MouseLabAvaloniaApp.Models;
using MouseLabAvaloniaApp.Services.AppSettings;
using MouseLabAvaloniaApp.ViewModels;
using MouseLabAvaloniaApp.Views;
using ProTranslate;
using ProTranslate.Avalonia;
using ProTranslate.Generated;
using System;
using System.Globalization;

namespace MouseLabAvaloniaApp;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;
    private ThemeChangedEventHandler? _themeChangedHandler;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private static ServiceCollection GetServicesCollection(AppSettingsSnapshot snapshot, CultureInfo initialCulture)
    {
        ServiceCollection services = new();

        services.AddSingleton<IApplicationSettingsService>(new ApplicationSettingsService(snapshot));

        services.AddProTranslate(
            provider: new ProTranslateGeneratedTranslationProvider("MouseLabCatalog"),
            culture: initialCulture,
            cultureOptions: new CultureServiceOptions
            {
                ApplyToCurrentThread = true,
                ApplyToDefaultThread = true
            },
            translationOptions: new TranslationFallbackOptions
            {
                DefaultCulture = CultureInfo.GetCultureInfo(AppSettingsStore.DefaultCultureName)
            },
            cacheOptions: new TranslationCacheOptions
            {
                MaximumEntries = 2048
            });

        services.AddProTranslateAvalonia();
        services.AddSingleton<MainWindowViewModel>();

        return services;
    }

    public override void OnFrameworkInitializationCompleted()
    {
        AppSettingsSnapshot snapshot = AppSettingsStore.Load();
        CultureInfo initialCulture = AppSettingsStore.ResolveCulture(snapshot.Culture);

        _serviceProvider = GetServicesCollection(snapshot, initialCulture).BuildServiceProvider();
        _serviceProvider.UseProTranslateAvalonia();

        IApplicationSettingsService settings = _serviceProvider.GetRequiredService<IApplicationSettingsService>();
        _themeChangedHandler = (_, e) => ApplyTheme(e.NewTheme);
        settings.ThemeChanged += _themeChangedHandler;
        ApplyTheme(settings.CurrentAppTheme);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = _serviceProvider.GetRequiredService<MainWindowViewModel>(),
            };

            desktop.Exit += (_, _) => Shutdown();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ApplyTheme(Themes theme) =>
        RequestedThemeVariant = theme switch
        {
            Themes.Light => ThemeVariant.Light,
            Themes.Dark => ThemeVariant.Dark,
            _ => ThemeVariant.Default
        };

    private void Shutdown()
    {
        if (_serviceProvider is null)
            return;

        if (_themeChangedHandler is not null &&
            _serviceProvider.GetService<IApplicationSettingsService>() is { } settings)
        {
            settings.ThemeChanged -= _themeChangedHandler;
        }

        _themeChangedHandler = null;
        _serviceProvider.Dispose();
        _serviceProvider = null;
    }
}
