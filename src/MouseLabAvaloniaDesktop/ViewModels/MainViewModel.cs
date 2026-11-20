using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MouseLabAvaloniaDesktop.Services.Abstractions;
using MouseLabAvaloniaDesktop.ViewModels.Worktop;
using ProTranslate;

namespace MouseLabAvaloniaDesktop.ViewModels;

public partial class MainViewModel : TranslatedViewModelBase
{


    public MainViewModel(ITranslationService translation,
                        MenuViewModel menuViewModel,
                        WorktopViewModel worktopViewModel) : base(translation)
    {
        MenuViewModel = menuViewModel;
        WorktopViewModel = worktopViewModel;
    }

    public MenuViewModel MenuViewModel { get; }

    public WorktopViewModel WorktopViewModel { get; }
}

