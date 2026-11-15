using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MouseLabAvaloniaDesktop.Services.Abstractions;
using ProTranslate;

namespace MouseLabAvaloniaDesktop.ViewModels;

public partial class MainViewModel : TranslatedViewModelBase
{
    private readonly IDialogService _dialogService;

    public MainViewModel(ITranslationService translation, IDialogService dialogService) : base(translation)
    {
        _dialogService = dialogService;
    }


    [RelayCommand]
    private void ShowSettings() => _dialogService.Show<SettingsWindowViewModel>();
}

