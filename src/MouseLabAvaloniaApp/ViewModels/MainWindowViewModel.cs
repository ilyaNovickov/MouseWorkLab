using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using MouseLabAvaloniaApp.Models;
using MouseLabAvaloniaApp.Services.AppSettings;
using ProTranslate;
using ProTranslate.Generated;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MouseLabAvaloniaApp.Services.WindowsManager;
using System.Threading.Tasks;

namespace MouseLabAvaloniaApp.ViewModels;

/// <summary>
/// Модель представления главного окна: демонстрирует все три способа вывода
/// локализованного текста и содержит переключатель языка.
/// </summary>
public partial class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel(
        ITranslationService translation,
        IWindowsManagerService winowsManagerService
        ) : base(translation)
    {
        WindowsManager = winowsManagerService;
    }

    private IWindowsManagerService WindowsManager { get; }

    [RelayCommand]
    private async Task OpenSettingsWindow()
    {
        Task task = WindowsManager.ShowSettingsAsync();

        await task;
    }

    
}
