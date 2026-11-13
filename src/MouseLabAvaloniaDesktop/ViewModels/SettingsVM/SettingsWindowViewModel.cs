using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using ProTranslate;

namespace MouseLabAvaloniaDesktop.ViewModels;

public partial class SettingsWindowViewModel : ViewModelBase, IDialogResult
{
    public SettingsWindowViewModel(ITranslationService translation,
        ModelingSettingsViewModel modelingSettingsViewModel) : base(translation)
    {
        SettingsVM = [modelingSettingsViewModel];
    }

    public Action<bool>? CloseAction { get; set; }

    [RelayCommand]
    private void Save()
    {
        CloseAction?.Invoke(true);
    }

    [RelayCommand]
    private void Cancel()
    {
        CloseAction?.Invoke(false);
    }

    public IReadOnlyList<ViewModelBase> SettingsVM { get; private set; }
}
