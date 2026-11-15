using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using Microsoft.Extensions.DependencyInjection;
using MouseLab.Core.Models;
using MouseLab.Core.Services;
using MouseLabAvaloniaDesktop.Models;
using MouseLabAvaloniaDesktop.Services.AppSettings;
using MouseLabAvaloniaDesktop.ViewModels;
using MouseLabAvaloniaDesktop.Views;
using Avalonia.Markup.Xaml;
using ProTranslate;
using MouseLab.Common.Services;

namespace MouseLabAvaloniaDesktop;

public partial class App : Application
{
    private AppServices _appServices = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public void OnShutdown()
    {
        _appServices.Dispose();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        _appServices = AppServices.CreateDefault();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Exit += (_, _) => OnShutdown();

            // События настроек — единственный канал, по которому Save в окне
            // настроек доезжает до UI (тема) и сервисов (язык, моделирование).
            IApplicationSettingsService settings = _appServices.GetRequiredService<IApplicationSettingsService>();
            ModelingEnv env = _appServices.GetRequiredService<ModelingEnv>();
            ICultureService cultures = _appServices.GetRequiredService<ICultureService>();

            settings.ThemeChanged += (_, e) => ApplyTheme(e.NewTheme);
            settings.CultureChanged += (_, e) => cultures.SetCulture(e.NewCulture);
            settings.ModelingChanged += (_, e) =>
                // Экспериментальный флаг вернётся в Core, когда там раскомментируют
                // ModelingSettings.ExperimentalSettings.
                env.ApplySettings(new ModelingSettings { WorktopResolution = e.WorktopResolution });

            ApplyTheme(settings.CurrentAppTheme);

            // ModelingChanged поднимается только при реальном изменении, а
            // ModelingEnv по умолчанию строит полотно 1024 — без однократного
            // применения стартовые настройки так и не доехали бы до полотна.
            env.ApplySettings(new ModelingSettings { WorktopResolution = settings.WorktopResolution });

            desktop.MainWindow = new MainWindow
            {
                DataContext = _appServices.GetRequiredService<MainViewModel>(),
            };

            /*
desktop.MainWindow = new SettingsWindow
{
    DataContext = _appServices.GetRequiredService<SettingsWindowViewModel>(),
};
*/
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ApplyTheme(Themes theme) =>
        RequestedThemeVariant = theme switch
        {
            Themes.Dark => ThemeVariant.Dark,
            Themes.Light => ThemeVariant.Light,
            _ => ThemeVariant.Default
        };
}