using MouseLabAvalonia.Core.Interfaces;

namespace MouseLabAvalonia.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly IMessageDialogService messageBoxService;

        public MainWindowViewModel(IMessageDialogService messageDialogService)
        {
            this.messageBoxService = messageDialogService;

            PlotViewModel = new PlotViewModel(messageBoxService);
        }

        public PlotViewModel PlotViewModel { get; private set; }
    }
}
