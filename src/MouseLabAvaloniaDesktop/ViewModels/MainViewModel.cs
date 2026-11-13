using CommunityToolkit.Mvvm.ComponentModel;
using ProTranslate;

namespace MouseLabAvaloniaDesktop.ViewModels;

public partial class MainViewModel : TranslatedViewModelBase
{
    public MainViewModel(ITranslationService translation) : base(translation)
    {

    }

    [ObservableProperty]
    public partial string Greeting { get; set; } = "Welcome to Avalonia!";
}
