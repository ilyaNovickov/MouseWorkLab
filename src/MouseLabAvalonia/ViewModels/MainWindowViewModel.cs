using MouseLabAvalonia.Core.Interfaces;

namespace MouseLabAvalonia.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly IMessageDialogService messageBoxService;

        public MainWindowViewModel(IMessageDialogService messageDialogService, PlotViewModel plotViewModel)
        {
            this.messageBoxService = messageDialogService;

            PlotViewModel = plotViewModel;
        }

        public PlotViewModel PlotViewModel { get; private set; }
    }
}