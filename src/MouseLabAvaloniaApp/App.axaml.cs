using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Microsoft.Extensions.DependencyInjection;
using MouseLabAvaloniaApp.Services.AppSettings;
using MouseLabAvaloniaApp.ViewModels;
using MouseLabAvaloniaApp.Views;
using ProTranslate;
using System.Globalization;
using System.Linq;

namespace MouseLabAvaloniaApp;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private ServiceCollection GetServicesCollection()
    {
        ServiceCollection services = new ServiceCollection();

        services.AddSingleton<IApplicationSettingsService, ApplicationSettingsService>()
            .AddProTranslate(
                culture: CultureInfo.GetCultureInfo("en-US"),
                cultureOptions: new CultureServiceOptions
                {
                    ApplyToCurrentThread = false,
                    ApplyToDefaultThread = false
                },
                cacheOptions: new TranslationCacheOptions
                {
                    MaximumEntries = 2048
                })
            .AddSingleton<MainWindowViewModel>();

        return services;
    }

    public override void OnFrameworkInitializationCompleted()
    {
        ServiceCollection services = GetServicesCollection();

        using ServiceProvider serviceProvider = services.BuildServiceProvider();

        serviceProvider.GetService<IApplicationSettingsService>()?.ThemeChanged +=
            (sender, e) => this.RequestedThemeVariant = e.NewTheme switch
            {
                Models.Themes.Light => ThemeVariant.Light,
                Models.Themes.Dark => ThemeVariant.Dark,
                Models.Themes.Default => ThemeVariant.Default,
                _ => ThemeVariant.Light
            };
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = serviceProvider.GetService<MainWindowViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}