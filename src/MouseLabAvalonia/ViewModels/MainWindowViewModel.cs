using CommunityToolkit.Mvvm.Input;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm;
using MouseLabAvalonia.Core.Interfaces;
using System.Collections.ObjectModel;

namespace MouseLabAvalonia.ViewModels
{
    public partial class MenuListViewModel : ViewModelBase
    {
        private DockFactory factory;
        public MenuListViewModel(string name, string vmName, DockFactory factory)
        {
            Name = name;
            ViewModelName = vmName;
            this.factory = factory;
        }
        public string Name { get; }

        private string ViewModelName { get; }

        [RelayCommand]
        private void AddDockable()
        {
            factory.AddDockable(this.ViewModelName);
        }
    }

    public partial class MainWindowViewModel : ViewModelBase
    {
        private IRootDock? _layout;
        private IFactory dockFactory;

        public MainWindowViewModel(DockFactory dockFactory, ViewModelFactory viewModelFactory)
        {
            this.dockFactory = dockFactory;

            this.PlotViewModel = viewModelFactory.PlotViewModel;

            var layout = dockFactory.CreateLayout();
            dockFactory.InitLayout(layout);
            Layout = layout;

            MenuItems.Add(new MenuListViewModel("Графики", "PlotViewModelDocument", dockFactory));
        }

        public ObservableCollection<MenuListViewModel> MenuItems { get; } = new();

        public PlotViewModel PlotViewModel { get; private set; }

        public IRootDock? Layout
        {
            get => _layout;
            set => SetProperty(ref _layout, value);
        }

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

        public void CloseLayout()
        {
            if (Layout is IDock dock)
            {
                if (dock.Close.CanExecute(null))
                {
                    dock.Close.Execute(null);
                }
            }
        }
#if DEBUG
        public static MainWindowViewModel Instance 
        {
            get
            {
                var vmFactory = new ViewModelFactory();

                return new MainWindowViewModel(new DockFactory(vmFactory), vmFactory);
            }
        }
#endif
    }
}