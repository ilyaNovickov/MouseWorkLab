using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Microsoft.Extensions.DependencyInjection;
using MouseLabAvaloniaApp.Models;
using MouseLabAvaloniaApp.Services.AppSettings;
using MouseLabAvaloniaApp.Services.WindowsManager;
using MouseLabAvaloniaApp.ViewModels;
using MouseLabAvaloniaApp.ViewModels.Settings;
using MouseLabAvaloniaApp.Views;
using ProTranslate;
using ProTranslate.Avalonia;
using ProTranslate.Generated;
using System;
using System.Globalization;

namespace MouseLabAvaloniaApp;

public partial class App : Application
{
    AppServices? _serviceManager;

    // Обработчик храним в поле, чтобы можно было отписаться от него при выходе.
    private ThemeChangedEventHandler? _themeChangedHandler;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Читаем настройки до сборки контейнера: культура обязана быть известна заранее.
        _serviceManager = AppServices.CreateDefault();
        
        IApplicationSettingsService settings = _serviceManager.Provider.GetRequiredService<IApplicationSettingsService>();

        // Тема применяется и сразу при старте, и на каждое изменение настроек.
        _themeChangedHandler = (_, e) => ApplyTheme(e.NewTheme);
        settings.ThemeChanged += _themeChangedHandler;
        ApplyTheme(settings.CurrentAppTheme);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            MainWindow mainWindow = _serviceManager.Provider.GetRequiredService<MainWindow>();
            mainWindow.DataContext = _serviceManager.Provider.GetRequiredService<MainWindowViewModel>();

            desktop.MainWindow = mainWindow;
            desktop.Exit += (_, _) => Shutdown();
        }

        base.OnFrameworkInitializationCompleted();
    }

    // Themes.Default - это "следовать системной теме" (ThemeVariant.Default).
    private void ApplyTheme(Themes theme) =>
        RequestedThemeVariant = theme switch
        {
            Themes.Light => ThemeVariant.Light,
            Themes.Dark => ThemeVariant.Dark,
            _ => ThemeVariant.Default
        };

    private void Shutdown()
    {
        if (_serviceManager is null)
            return;

        // Отписываемся от события перед Dispose, иначе обработчик остался бы висеть
        // на освобождаемом объекте.
        if (_themeChangedHandler is not null &&
            _serviceManager.Provider.GetService<IApplicationSettingsService>() is { } settings)
        {
            settings.ThemeChanged -= _themeChangedHandler;
        }

        _themeChangedHandler = null;
        _serviceManager.Dispose();
        _serviceManager = null;
    }
}