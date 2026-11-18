using CommunityToolkit.Mvvm.ComponentModel;
using MouseLabAvaloniaDesktop.Models;
using MouseLabAvaloniaDesktop.Models.Settings;
using MouseLabAvaloniaDesktop.Services.Implementations;
using ProTranslate;
using ProTranslate.Generated;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MouseLabAvaloniaDesktop.ViewModels;

public partial class AppearanceSettingsViewModel : TranslatedViewModelBase, ISettingsSectionViewModel
{
    private readonly ProjectService _projectService;

    private List<ThemeOption> availableThemes;

    private List<CultureOption> availableCultures;

    private ThemeOption? selectedTheme;

    private CultureOption? selectedCulture;

    //public AppearanceSettingsViewModel(ITranslationService translation, IApplicationSettingsService settings) : base(translation)
    public AppearanceSettingsViewModel(ITranslationService translation, ProjectService projectService) : base(translation)
    {
        _projectService = projectService;

        // Список языков задан вручную; при росте приложения его можно получать
        // из ProTranslateProviderManifest.Cultures, который SourceGenerator
        // собирает из имён файлов Strings.*.json.
        availableCultures =
        [
            new CultureOption("ru-RU", Describe("ru-RU")),
            new CultureOption("en-US", Describe("en-US"))
        ];

        availableThemes =
        [
            new ThemeOption(Strings.Observe_CommonDefaultTheme(), Themes.Default),
            new ThemeOption(Strings.Observe_CommonDarkTheme(), Themes.Dark),
            new ThemeOption(Strings.Observe_CommonLightTheme(), Themes.Light)
        ];

        // Выставляем в ComboBox значения, реально применённые в настройках,
        // а не первые в списке.
        SyncSelectedTheme();
        SyncSelectedCulture();

        _projectService.ProjectLoaded += _projectService_ProjectLoaded;
    }

    private void _projectService_ProjectLoaded(object? sender, EventArgs e)
    {
        SyncSelectedTheme();
        SyncSelectedCulture();
    }

    public IReadOnlyList<ThemeOption> AvailableThemes => availableThemes;

    public IReadOnlyList<CultureOption> AvailableCultures => availableCultures;

    public ThemeOption? SelectedTheme
    {
        get => selectedTheme;
        set => SetProperty(ref selectedTheme, value);
    }

    public CultureOption? SelectedCulture
    {
        get => selectedCulture;
        set => SetProperty(ref selectedCulture, value);
    }

    /// <summary>
    /// Публикует рабочую копию в сервис настроек (кнопка Save). Культуру и тему
    /// применяет подписчик сервиса, а не сама модель представления.
    /// </summary>
    public void Save()
    {
        if (SelectedTheme is not null)
        {
            _projectService.UiSettings.Theme = SelectedTheme.Theme;
        }

        if (SelectedCulture is not null)
        {
           _projectService.UiSettings.Culture = SelectedCulture.CultureName;
        }
    }

    /// <summary>
    /// Откатывает рабочую копию к применённым значениям (кнопка Cancel).
    /// </summary>
    public void Reset()
    {

    }

    // Название языка на самом языке («русский (Россия)»), чтобы список был
    // читаемым независимо от текущего языка интерфейса.
    private static string Describe(string cultureName)
    {
        CultureInfo culture = CultureInfo.GetCultureInfo(cultureName);
        string native = culture.NativeName;
        return string.IsNullOrEmpty(native) ? cultureName : native;
    }

    private void SyncSelectedTheme()
    {

        selectedTheme = AvailableThemes.FirstOrDefault(t => t.Theme == _projectService.UiSettings.Theme);
        OnPropertyChanged(nameof(SelectedTheme));
    }

    private void SyncSelectedCulture()
    {

        selectedCulture = AvailableCultures.FirstOrDefault(c => string.Equals(
            c.CultureName, 
            _projectService.UiSettings.Culture, 
            StringComparison.OrdinalIgnoreCase
            ));
        OnPropertyChanged(nameof(SelectedCulture));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this._projectService.ProjectLoaded -= _projectService_ProjectLoaded;
            // ProTranslateStrings.Dispose() не освобождает наблюдаемые строки,
            // выданные Strings.Observe_*(); ими владеет тот, кто их создал.
            foreach (ThemeOption option in availableThemes)
                option.Dispose();

            availableThemes = [];
            availableCultures = [];
        }

        base.Dispose(disposing);
    }
}
