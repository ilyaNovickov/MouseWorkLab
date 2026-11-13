using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using MouseLabAvaloniaDesktop.Services.Abstractions;
using MouseLabAvaloniaDesktop.ViewModels;


namespace MouseLabAvaloniaDesktop.Services.Implementations;

public class DialogService : IDialogService
{
    private static readonly Dictionary<Type, Func<Window>> WindowRegistry = new()
    {
        /*
        [typeof(SettingsViewModel)] = () => new SettingsWindow(),
        [typeof(AboutViewModel)] = () => new AboutWindow(),
        */
    };


    private IServiceProvider _serviceProvider;

    private HashSet<ViewModelBase> openedWindows = new();

    public DialogService(IServiceProvider provider)
    {
        this._serviceProvider = provider;
    }

    public void Show<T>() where T : ViewModelBase
    {
        if (openedWindows.Any(win => win is T))
        {
            return;
        }

        var window = GetWindow<T>(_serviceProvider, openedWindows);

        var owner = GetMainWindow();

        if (owner is not null)
            window.Show(owner);
        else
            window.Show();
    }

    public async Task<bool> ShowDialogAsync<T>() where T : ViewModelBase
    {
        if (openedWindows.Any(win => win is T))
        {
            return false;
        }

        var window = GetWindow<T>(_serviceProvider, openedWindows);

        // 4. Показываем модально
        var owner = GetMainWindow();
        return owner is not null
            ? await window.ShowDialog<bool>(owner)
            : false;
    }

    private static Window GetWindow<T>(IServiceProvider provider, HashSet<ViewModelBase> openedWindows) where T : ViewModelBase
    {
        if (!WindowRegistry.TryGetValue(typeof(T), out var factory))
            throw new InvalidOperationException(
                $"Окно для {typeof(T).Name} не зарегистрировано");

        // 1. ViewModel из DI (со всеми зависимостями)
        var vm = provider.GetRequiredService<T>();

        openedWindows.Add(vm);

        // 2. Window из фабрики (без рефлексии)
        var window = factory();
        window.Closed += (_, _) =>
        {
            openedWindows.Remove(vm);
            if (vm is IDisposable disposalbe) disposalbe.Dispose();
        };
        window.DataContext = vm;

        // 3. Связываем закрытие
        if (vm is IDialogResult dialogResult)
            dialogResult.CloseAction = (res) =>
            {
                window.Close(res);
            };

        return window;
    }

    private static Window? GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime
            is IClassicDesktopStyleApplicationLifetime desktop)
            return desktop.MainWindow;
        return null;
    }

}
