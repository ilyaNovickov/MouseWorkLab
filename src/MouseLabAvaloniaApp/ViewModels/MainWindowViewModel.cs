using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm;
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
using System.Collections.ObjectModel;
using MouseLabAvaloniaApp.ViewModels.Dock;
using Dock.Model.Core;
using Dock.Model.Controls;
using MouseLabAvaloniaApp.Services.Dock;

namespace MouseLabAvaloniaApp.ViewModels;

/// <summary>
/// Модель представления главного окна: демонстрирует все три способа вывода
/// локализованного текста и содержит переключатель языка.
/// </summary>
public partial class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel(
        ITranslationService translation,
        IWindowsManagerService winowsManagerService,
        IFactory dockFactory
        ) : base(translation)
    {
        WindowsManager = winowsManagerService;

        this.dockFactory = dockFactory;
        var layout = dockFactory.CreateLayout();
        if (layout is null)
            throw new Exception("");
        dockFactory.InitLayout(layout);
        Layout = layout;
    }

    private IWindowsManagerService WindowsManager { get; }

    [RelayCommand]
    private async Task OpenSettingsWindow()
    {
        Task task = WindowsManager.ShowSettingsAsync();

        await task;
    }

    #region Dock

    private readonly IFactory dockFactory;

    private IRootDock? layout;

    public IRootDock? Layout
    {
        get => layout;
        set => SetProperty(ref layout, value);
    }

    /// <summary>
        /// Сброс макета до значений по умолчанию
        /// </summary>
        public void ResetLayout()
        {
            if (Layout is not null)
            {
                if (Layout.Close.CanExecute(null))
                {
                    Layout.Close.Execute(null);
                }
            }

            var layout = dockFactory.CreateLayout();
            if (layout is not null)
            {
                dockFactory.InitLayout(layout);
                Layout = layout;
            }
        }

        /// <summary>
        /// Закрытие Dock макета
        /// </summary>
        public void CloseLayout()
        {
            if (Layout is null)
                return;
            if (Layout.Close.CanExecute(null))
            {
                Layout.Close.Execute(null); ;
            }
        }

        public ObservableCollection<DocumentModel> Documents { get; } = new();

        [RelayCommand]
    private void AddDock()
    {
        var doc = new DocumentModel(Strings.Observe_CommonCancel());
        doc.Context = new BlankViewModel(this.Translations);

        Documents.Add(doc);
    }

        [RelayCommand]
    private void RemoveDock()
    {
        if (Documents.Count == 0)
            return;
        var doc = Documents.ElementAt(Documents.Count - 1);
        Documents.RemoveAt(Documents.Count - 1);
        doc.Context = null;
        doc.Dispose();
    }
    /*
    public ObservableCollection<BlankViewModel> Documents { get; } = new();

    [RelayCommand]
    private void AddDock()
    {
        Documents.Add(new BlankViewModel(this.Translations));
    }

    [RelayCommand]
    private void RemoveDock()
    {
        if (Documents.Count == 0)
            return;

        Documents.RemoveAt(Documents.Count - 1);
    }
    */
    #endregion
}
