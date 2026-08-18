using Dock.Model.Controls;
using Dock.Model.Core;
using MouseLabAvalonia.Core.Interfaces;

namespace MouseLabAvalonia.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly IMessageDialogService messageBoxService;
        private IRootDock? _layout;

        public MainWindowViewModel(IMessageDialogService messageDialogService, PlotViewModel plotViewModel)
        {
            this.messageBoxService = messageDialogService;

            PlotViewModel = plotViewModel;

            var factory = new DockFactory(plotViewModel);
            var layout = factory.CreateLayout();
            factory.InitLayout(layout);
            Layout = layout;
        }

        public PlotViewModel PlotViewModel { get; private set; }

        public IRootDock? Layout
        {
            get => _layout;
            set => SetProperty(ref _layout, value);
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
    }
}