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
        private readonly PlotViewModel plotViewModel;
        private IRootDock? _rootDock;
        private IDocumentDock? _documentDock;

        public DockFactory(PlotViewModel plotViewModel)
        {
            this.plotViewModel = plotViewModel;
        }

        public override IRootDock CreateLayout()
        {
            var document = new Document
            {
                Id = "PlotDocument",
                Title = "Параметры",
                Context = plotViewModel,
                CanClose = false
            };

            var documentDock = new DocumentDock
            {
                Id = "Documents",
                Title = "Documents",
                IsCollapsable = false,
                CanCreateDocument = false,
                ActiveDockable = document,
                VisibleDockables = CreateList<IDockable>(document)
            };

            var rootDock = CreateRootDock();
            rootDock.Id = "Root";
            rootDock.IsCollapsable = false;
            rootDock.ActiveDockable = documentDock;
            rootDock.DefaultDockable = documentDock;
            rootDock.VisibleDockables = CreateList<IDockable>(documentDock);

            _rootDock = rootDock;
            _documentDock = documentDock;

            return rootDock;
        }

        public override void InitLayout(IDockable layout)
        {
            DockableLocator = new Dictionary<string, Func<IDockable?>>
            {
                ["Root"] = () => _rootDock,
                ["Documents"] = () => _documentDock
            };

            base.InitLayout(layout);
        }
    }
}