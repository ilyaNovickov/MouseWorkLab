using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MouseLabAvaloniaDesktop.Services.Abstractions;
using ProTranslate;

namespace MouseLabAvaloniaDesktop.ViewModels;

public partial class MainViewModel : TranslatedViewModelBase
{
    private readonly IDialogService _dialogService;

    public MainViewModel(ITranslationService translation, IDialogService dialogService, WorktopViewModel worktopViewModel) : base(translation)
    {
        _dialogService = dialogService;
        WorktopViewModel = worktopViewModel;
    }

    public WorktopViewModel WorktopViewModel { get; }

    [RelayCommand]
    private void ShowSettings() => _dialogService.Show<SettingsWindowViewModel>();

    [ObservableProperty]
    public partial string Greeting { get; set; } = "Welcome to Avalonia!";
}
