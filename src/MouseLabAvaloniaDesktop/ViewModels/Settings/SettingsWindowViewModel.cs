using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using ProTranslate;

namespace MouseLabAvaloniaDesktop.ViewModels;

public partial class SettingsWindowViewModel : TranslatedViewModelBase, IDialogResult
{
    public SettingsWindowViewModel(ITranslationService translation,
        ModelingSettingsViewModel modelingSettingsViewModel,
        AppearanceSettingsViewModel appearanceSettingsViewModel) : base(translation)
    {
        SettingsVM = [modelingSettingsViewModel, appearanceSettingsViewModel];
    }

    public Action<bool>? CloseAction { get; set; }

    [RelayCommand]
    private void Save()
    {
        // Save НЕ закрывает окно: пользователь правит и сохраняет дальше.
        foreach (ISettingsSectionViewModel section in Sections)
            section.Save();
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
