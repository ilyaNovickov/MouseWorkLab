using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using MouseLabAvaloniaDesktop.ViewModels;
using MouseLabAvaloniaDesktop.Views;
using Microsoft.Extensions.DependencyInjection;
using Avalonia.Markup.Xaml;

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

            desktop.MainWindow = new MainWindow
            {
                DataContext = _appServices.GetRequiredService<MainViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}