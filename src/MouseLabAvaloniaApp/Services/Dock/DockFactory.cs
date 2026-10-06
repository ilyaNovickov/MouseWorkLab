using System;
using Dock.Model.Mvvm.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm;
using System.Collections.Generic;
using Dock.Avalonia.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ProTranslate;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MouseLabAvaloniaApp.Services.Dock;

public class DockFactory  : Factory
{
    private IRootDock? _rootDock;

    private IDocumentDock? _documentDock;

    public override IRootDock CreateLayout()
    {
        //Документ с графикаи
            var plotDocument = new Document
            {
                Id = "PlotViewModelDocument",
                Title = "Параметры",
                CanClose = true,
                CanFloat = false//должен быть всегда `false`, иначе страшный баг
            };

            var workSurfaceDocument = new Document
            {
                Id = "WorkSurfaceViewModelDocument",
                Title = "Рабочая поверхность",
                CanClose = true,
                CanFloat = true
            };

            //Док для документов
            _documentDock = new DocumentDock
            {
                Id = "Documents",
                Title = "Documents",
                IsCollapsable = false,
                CanCreateDocument = false,
                ActiveDockable = workSurfaceDocument,
                VisibleDockables = CreateList<IDockable>
                (
                    plotDocument, workSurfaceDocument
                )
            };

            //Корневой док
            _rootDock = CreateRootDock();
            _rootDock.Id = "Root";
            _rootDock.IsCollapsable = false;
            _rootDock.ActiveDockable = _documentDock;
            _rootDock.DefaultDockable = _documentDock;
            _rootDock.VisibleDockables = CreateList<IDockable>(_documentDock);

            return _rootDock;
    }

    public override void InitLayout(IDockable layout)
    {
        DockableLocator = new Dictionary<string, Func<IDockable?>>()
        {
            ["Root"] = () => _rootDock
        };

        HostWindowLocator = new Dictionary<string, Func<IHostWindow?>>()
        {
            [nameof(IDockWindow)] = () =>
            {
                var win = new HostWindow();

                return win;
            }
        };

        base.InitLayout(layout);
    }

    public override IDockWindow? CreateWindowFrom(IDockable dockable)
    {
        var window = base.CreateWindowFrom(dockable);

        return window;
    }

    public override void AddDockable(IDock dock, IDockable dockable)
    {
        base.AddDockable(dock, dockable);
    }

    public static void foo()
    {
        Document doc = new Document();
        
    }
}

public partial class DocumentModel : ObservableObject, IDisposable
{
    private IObservableLocalizedString _title;

    public DocumentModel(IObservableLocalizedString title)
    {
        _title = title;

        _title.PropertyChanged += (_, _) =>
        {
             OnPropertyChanged(nameof(Title));
        };
    }

    public string Title
    {
        get => _title.Value;
    }

    [ObservableProperty]
    private object? context;
    
    public bool CanClose { get; set; } = true;
     
    public void Dispose()
    {
        _title.Dispose();
    }
}