using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MouseLabAvaloniaDesktop.Services.Implementations;
using ProTranslate;

namespace MouseLabAvaloniaDesktop.ViewModels;

public partial class SettingsWindowViewModel : TranslatedViewModelBase, IDialogResult
{
    private readonly ProjectFileService _projectFileService;

    public SettingsWindowViewModel(
        ITranslationService translation,
        ProjectFileService projectFileService,
        ModelingSettingsViewModel modelingSettingsViewModel,
        AppearanceSettingsViewModel appearanceSettingsViewModel) : base(translation)
    {
        _projectFileService = projectFileService;

        SettingsVM = [modelingSettingsViewModel, appearanceSettingsViewModel];
    }

    public Action<bool>? CloseAction { get; set; }

    [ObservableProperty]
    private bool isEnable = true;

    [RelayCommand]
    private async Task Save()
    {
        // Save НЕ закрывает окно: пользователь правит и сохраняет дальше.
        foreach (ISettingsSectionViewModel section in Sections)
            section.Save();

        IsEnable = false;
        await _projectFileService.SaveAsync(
            T("FileDialog.SaveProjectTitle"), T("FileDialog.JsonFilter"));
        IsEnable = true;
    }

    [RelayCommand]
    private void Cancel()
    {
        // Cancel откатывает только несохранённые правки: рабочие копии секций
        // возвращаются к значениям, применённым сервисом, а сам сервис не трогаем.
        foreach (ISettingsSectionViewModel section in Sections)
            section.Reset();

        CloseAction?.Invoke(false);
    }

    private IEnumerable<ISettingsSectionViewModel> Sections =>
        SettingsVM.OfType<ISettingsSectionViewModel>();

    public IReadOnlyList<ViewModelBase> SettingsVM { get; private set; }

    /// <summary>
    /// Секциями владеет окно: DI выдаёт их транзиентно, и без освобождения здесь
    /// наблюдаемые строки ThemeOption пережили бы закрытие окна.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach (ViewModelBase section in SettingsVM)
                if (section is IDisposable disposable)
                    disposable.Dispose();
        }

        base.Dispose(disposing);
    }
}
