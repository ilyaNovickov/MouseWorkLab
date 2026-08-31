using Dock.Avalonia.Controls;
using Dock.Model;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm;
using Dock.Model.Mvvm.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MouseLabAvalonia.ViewModels
{
    public class DockFactory : Factory
    {
        //private readonly PlotViewModel plotViewModel;
        private ViewModelFactory factory;
        private IRootDock? _rootDock;
        private IDocumentDock? _documentDock;

        private IToolDock? _toolDock;

        private List<IDockable> deletedItems = new();
        private IDocument? plotDocument;

        public DockFactory(ViewModelFactory viewModelFactory)
        {
            //this.plotViewModel = plotViewModel;
            factory = viewModelFactory;

            this.HideDocumentsOnClose = true;
        }

        public override IRootDock CreateLayout()
        {
            plotDocument = new Document
            {
                Id = "PlotViewModelDocument",
                Title = "Параметры",
                Context = factory.PlotViewModel,
                CanClose = true,
                CanFloat = false//должен быть всегда `false`, иначе страшный баг
            };

            _documentDock = new DocumentDock
            {
                Id = "Documents",
                Title = "Documents",
                IsCollapsable = false,
                CanCreateDocument = false,
                ActiveDockable = plotDocument,
                VisibleDockables = CreateList<IDockable>(plotDocument)
            };

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
            DockableLocator = new Dictionary<string, Func<IDockable?>>
            {
                ["Root"] = () => _rootDock,
                ["Documents"] = () => _documentDock,
                ["PlotViewModelDocument"] = () => plotDocument
            };

            HostWindowLocator = new Dictionary<string, Func<IHostWindow?>>
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
            
            if (window != null)
            {
                //window.Title = "Dock Avalonia Demo";
            }
            return window;
        }

        public void AddDockable(string vmName)
        {
            if (this.DockableLocator is null)
                return;

            bool? res = this.DockableLocator.ContainsKey(vmName);

            if (!res.HasValue || !res.Value)
                return;

            IDockable? item = this.DockableLocator[vmName].Invoke();

            if (item is null)
                return;

            if (!deletedItems.Contains(item))
                return;

            IDock? dock = null;

            if (item is IDocument)
            {
                dock = _documentDock;
            }
            else if (item is ITool)
            {
                dock = _toolDock;
            }
            else
            {
                return;
            }

            dock?.VisibleDockables?.Add(item);
            deletedItems.Remove(item);
        }

        public override void CloseDockable(IDockable dockable)
        {
            bool? res = this.DockableLocator?.ContainsKey(dockable.Id);
            if (res.HasValue && res.Value && (dockable is IDocument or ITool))
            {
                deletedItems.Add(dockable);
            }
            
            base.CloseDockable(dockable);
        }

        public override void CloseWindow(IDockWindow window)
        {
            base.CloseWindow(window);
        }
    }
}