namespace MouseLabAvalonia.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {

        public MainWindowViewModel()
        {
            PlotViewModel = new PlotViewModel();
        }

        public PlotViewModel PlotViewModel { get; private set; }
    }
}
