using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MouseLabAvaloniaDesktop.Services.Implementations;
using ProTranslate;

namespace MouseLabAvaloniaDesktop.ViewModels;

public partial class SettingsWindowViewModel : TranslatedViewModelBase, IDialogResult
{
    private readonly ProjectService _projectService;

    private readonly FileDialogService _fileDialogService;

    public SettingsWindowViewModel(
        ITranslationService translation,
        FileDialogService fileDialogService,
        ProjectService projectService,
        ModelingSettingsViewModel modelingSettingsViewModel,
        AppearanceSettingsViewModel appearanceSettingsViewModel) : base(translation)
    {
        _fileDialogService = fileDialogService;
        _projectService = projectService;

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
        // Пересмотреть
        if (!File.Exists(_projectService.CurrentFilePath))
        {
            var savedFile = await _fileDialogService.SaveFileAsync();
            if (savedFile is null)
                return;
            if (Path.GetExtension(savedFile.Path.LocalPath) != ".json")
                throw new Exception("File has to be json");
            File.Create(savedFile.Path.AbsolutePath).Close();
            _projectService.SaveTo(savedFile.Path.AbsolutePath);
        }
        else
        {
            _projectService.Save();
        }
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
