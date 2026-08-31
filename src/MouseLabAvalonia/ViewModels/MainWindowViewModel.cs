using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm;
using MouseLabAvalonia.Core.Interfaces;

namespace MouseLabAvalonia.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private IRootDock? _layout;
        private IFactory dockFactory;

        //public MainWindowViewModel(IMessageDialogService messageDialogService, PlotViewModel plotViewModel)
        //{
        //    this.messageBoxService = messageDialogService;

        //    PlotViewModel = plotViewModel;

        //    var factory = new DockFactory(plotViewModel);
        //    var layout = factory.CreateLayout();
        //    factory.InitLayout(layout);
        //    Layout = layout;
        //}

        public MainWindowViewModel(DockFactory dockFactory, ViewModelFactory viewModelFactory)
        {
            this.dockFactory = dockFactory;

            this.PlotViewModel = viewModelFactory.PlotViewModel;

            var layout = dockFactory.CreateLayout();
            dockFactory.InitLayout(layout);
            Layout = layout;
        }

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

        public static MainWindowViewModel Instance 
        {
            get
            {
                var vmFactory = new ViewModelFactory();

                return new MainWindowViewModel(new DockFactory(vmFactory), vmFactory);
            }
        }

    }
}