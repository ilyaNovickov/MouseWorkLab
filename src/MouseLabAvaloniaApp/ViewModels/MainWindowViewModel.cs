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
using MouseLabAvaloniaApp.Services.WindowsManager;
using System.Threading.Tasks;
using MouseLabAvaloniaApp.ViewModels.Dock;
using Dock.Model.Core;
using Dock.Model.Controls;
using Dock.Model.Mvvm.Controls;
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

        IRootDock? created = dockFactory.CreateLayout()
            ?? throw new InvalidOperationException("DockFactory.CreateLayout() вернул null.");

        // InitLayout обязан выполняться здесь, а не через InitializeLayout="True"
        // в XAML: именно он проставляет dockable.Factory, без чего DockControl
        // прекращает инициализацию и макет остаётся пустым.
        dockFactory.InitLayout(created);

        Layout = created;
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
    /// Сброс макета к значениям по умолчанию
    /// </summary>
    public void ResetLayout()
    {
        if (Layout is not null && Layout.Close.CanExecute(null))
            Layout.Close.Execute(null);

        IRootDock? newLayout = dockFactory.CreateLayout();
        if (newLayout is not null)
        {
            dockFactory.InitLayout(newLayout);
            Layout = newLayout;
        }
    }

    /// <summary>
    /// Добавляет окно с содержимым <see cref="BlankViewModel"/> в док документов.
    /// </summary>
    [RelayCommand]
    private void AddDock()
    {
        // DockFactory типизирован как IFactory, поэтому приводим к своему типу
        // ради NewDocument: доков он создаёт сам, а нам нужен ещё и контекст.
        if (dockFactory is not DockFactory factory || factory.DocumentDock is not IDocumentDock documentDock)
            return;

        Document document = factory.NewDocument("Рабочая поверхность");
        document.Context = new BlankViewModel(Translations);

        factory.AddDockable(documentDock, document);
    }

    /// <summary>
    /// Закрывает последнее открытое окно.
    /// </summary>
    [RelayCommand]
    private void RemoveDock()
    {
        // Состав вкладок берём у самого дока: своя коллекция в модели
        // представления неизбежно разошлась бы с деревом доков.
        if (dockFactory is not DockFactory factory || factory.DocumentDock is not IDocumentDock documentDock)
            return;

        if (documentDock.VisibleDockables is not IList<IDockable> dockables || dockables.Count == 0)
            return;

        IDockable last = dockables[^1];
        factory.RemoveDockable(last, true);
    }
    #endregion
}
