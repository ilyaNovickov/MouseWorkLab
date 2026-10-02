using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Microsoft.Extensions.DependencyInjection;
using MouseLabAvaloniaApp.Models;
using MouseLabAvaloniaApp.Services.AppSettings;
using MouseLabAvaloniaApp.ViewModels;
using MouseLabAvaloniaApp.Views;

namespace MouseLabAvaloniaApp;

public partial class App : Application
{
    // Контейнер в поле, а не в локальной переменной: см. AppServices.
    // Освобождается в Shutdown() по desktop.Exit.
    private AppServices? _appServices;

    // Обработчик храним в поле, чтобы можно было отписаться от него при выходе.
    private ThemeChangedEventHandler? _themeChangedHandler;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // CreateDefault() читает settings.json и передаёт культуру в AddProTranslate,
        // поэтому настройки обязаны быть прочитаны ДО сборки контейнера - этим и
        // занимается AppServices, а не App.
        _appServices = AppServices.CreateDefault();

        IApplicationSettingsService settings = _appServices.Provider.GetRequiredService<IApplicationSettingsService>();

        // Тема применяется и сразу при старте, и на каждое изменение настроек.
        _themeChangedHandler = (_, e) => ApplyTheme(e.NewTheme);
        settings.ThemeChanged += _themeChangedHandler;
        ApplyTheme(settings.CurrentAppTheme);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            MainWindow mainWindow = _appServices.Provider.GetRequiredService<MainWindow>();
            mainWindow.DataContext = _appServices.Provider.GetRequiredService<MainWindowViewModel>();

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
        if (_appServices is null)
            return;

        // Отписываемся от события перед Dispose, иначе обработчик остался бы висеть
        // на освобождаемом объекте.
        if (_themeChangedHandler is not null &&
            _appServices.Provider.GetService<IApplicationSettingsService>() is { } settings)
        {
            settings.ThemeChanged -= _themeChangedHandler;
        }

        _themeChangedHandler = null;
        _appServices.Dispose();
        _appServices = null;
    }
}