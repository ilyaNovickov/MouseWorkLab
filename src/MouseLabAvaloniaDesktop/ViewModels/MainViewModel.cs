using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MouseLabAvaloniaDesktop.Services.Abstractions;
using ProTranslate;

namespace MouseLabAvaloniaDesktop.ViewModels;

public partial class MainViewModel : TranslatedViewModelBase
{


    public MainViewModel(ITranslationService translation, MenuViewModel menuViewModel) : base(translation)
    {
        MenuViewModel = menuViewModel;
    }

    public MenuViewModel MenuViewModel { get; }
}

