using Dock.Avalonia.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm;
using Dock.Model.Mvvm.Controls;
using System;
using System.Collections.Generic;

namespace MouseLabAvalonia.ViewModels
{
    public class DockFactory : Factory
    {
        //private readonly PlotViewModel plotViewModel;
        private ViewModelFactory factory;
        private IRootDock? _rootDock;
        private IDocumentDock? _documentDock;

        public DockFactory(ViewModelFactory viewModelFactory)
        {
            //this.plotViewModel = plotViewModel;
            factory = viewModelFactory;
        }

        public override IRootDock CreateLayout()
        {
            var document = new Document
            {
                Id = "PlotDocument",
                Title = "Параметры",
                Context = factory.PlotViewModel,
                CanClose = true,
                CanFloat = true
            };

            _documentDock = new DocumentDock
            {
                Id = "Documents",
                Title = "Documents",
                IsCollapsable = false,
                CanCreateDocument = false,
                ActiveDockable = document,
                VisibleDockables = CreateList<IDockable>(document)
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
                ["Documents"] = () => _documentDock
            };

            HostWindowLocator = new Dictionary<string, Func<IHostWindow?>>
            {
                [nameof(IDockWindow)] = () => new HostWindow()
            };

            base.InitLayout(layout);
        }

        public override IDockWindow? CreateWindowFrom(IDockable dockable)
        {
            var window = base.CreateWindowFrom(dockable);

            if (window != null)
            {
                window.Title = "Dock Avalonia Demo";
            }
            return window;
        }

        public override void CloseDockable(IDockable dockable)
        {
            base.CloseDockable(dockable);
        }

        public override void CloseWindow(IDockWindow window)
        {
            base.CloseWindow(window);
        }
    }
}