using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using MouseLabAvaloniaApp.Models;
using MouseLabAvaloniaApp.Services.AppSettings;
using ProTranslate;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MouseLabAvaloniaApp.ViewModels.Welcome;

/// <summary>
/// Окно приветствия: спрашивает данные о студенте и язык с темой, после чего
/// передаёт управление главному окну.
/// </summary>
/// <remarks>
/// <para>
/// Настройки здесь живут во временном хранилище
/// (<see cref="TemporaryAppSettingsService"/>), а не в settings.json: у каждого
/// пользователя будут свои настройки, поэтому постоянные пока не трогаются.
/// Перенос происходит один раз - в момент перехода к главному окну.
/// </para>
/// <para>
/// Проверка полей сделана вручную, а не через
/// <c>System.ComponentModel.DataAnnotations</c> и <c>Validator.TryValidateObject</c>:
/// те работают через рефлексию по атрибутам и несовместимы с NativeAOT
/// (см. требования AOT в docs/architecture.md).
/// </para>
/// </remarks>
public partial class WelcomeWindowViewModel : ViewModelBase
{
    private readonly ICultureService _cultures;
    private readonly IApplicationSettingsService _settings;

    private CultureOption? _selectedCulture;
    private ThemeOption? _selectedTheme;

    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _middleName = string.Empty;
    private string _group = string.Empty;

    public WelcomeWindowViewModel(
        ITranslationService translations,
        ICultureService cultures,
        IApplicationSettingsService settings) : base(translations)
    {
        _cultures = cultures;
        _settings = settings;

        // Список языков. Здесь он задан вручную; при росте приложения его можно
        // получать из ProTranslateProviderManifest.Cultures, который SourceGenerator
        // собирает из имён файлов Strings.*.json.
        AvailableCultures =
        [
            new CultureOption("en-US", Describe("en-US")),
            new CultureOption("ru-RU", Describe("ru-RU")),
        ];

        AvailableThemes =
        [
            new ThemeOption(Strings.Observe_CommonDarkTheme(), Themes.Dark),
            new ThemeOption(Strings.Observe_CommonLightTheme(), Themes.Light),
            new ThemeOption(Strings.Observe_CommonDefaultTheme(), Themes.Default)
        ];

        // Подписываемся на смену культуры, чтобы пересчитать собственные вычисляемые
        // свойства. Отписка - в Dispose.
        _cultures.CultureChanged += OnCultureChanged;

        // Начальный выбор берём из временного хранилища, а не из CurrentUICulture:
        // язык приветствия - это язык по умолчанию, а не язык из settings.json.
        // Через SetProperty, а не через сеттеры: иначе переключение на язык по
        // умолчанию слетело бы поверх культуры, заданной при запуске.
        InitializeSelection();
    }

    /// <summary>Выбранный язык. Двусторонне привязан к ComboBox.SelectedItem в XAML.</summary>
    public CultureOption? SelectedCulture
    {
        get => _selectedCulture;
        set
        {
            if (value is null || !SetProperty(ref _selectedCulture, value))
                return;

            // Порядок важен: сначала запоминаем выбор, потом переключаем культуру -
            // так временное хранилище успеет обновиться даже если что-то пойдёт не так.
            _settings.CurrentCultureName = value.CultureName;
            _cultures.SetCulture(value.Culture);
        }
    }

    public ThemeOption? SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            if (value is null || !SetProperty(ref _selectedTheme, value))
                return;

            // Тему применяем сразу, а не по кнопке: окно приветствия - единственное
            // место, где пользователь видит результат до подтверждения.
            // Приложение подписано на ThemeChanged этого хранилища на время
            // показа окна, см. App.ShowWelcome.
            _settings.CurrentAppTheme = value.Theme;
        }
    }

    public IReadOnlyList<CultureOption> AvailableCultures { get; }

    public IReadOnlyList<ThemeOption> AvailableThemes { get; }

    #region Данные о студенте

    public string FirstName
    {
        get => _firstName;
        set
        {
            if (SetProperty(ref _firstName, value))
                RefreshValidation(nameof(FirstName));
        }
    }

    public string LastName
    {
        get => _lastName;
        set
        {
            if (SetProperty(ref _lastName, value))
                RefreshValidation(nameof(LastName));
        }
    }

    /// <summary>Отчество. Необязательное поле, поэтому ошибок не имеет.</summary>
    public string MiddleName
    {
        get => _middleName;
        set => SetProperty(ref _middleName, value);
    }

    public string Group
    {
        get => _group;
        set
        {
            if (SetProperty(ref _group, value))
                RefreshValidation(nameof(Group));
        }
    }

    #endregion

    #region Проверка полей

    public bool HasFirstNameError => string.IsNullOrWhiteSpace(FirstName);

    public bool HasLastNameError => string.IsNullOrWhiteSpace(LastName);

    public bool HasGroupError => string.IsNullOrWhiteSpace(Group);

    public string? FirstNameError => HasFirstNameError ? Strings.WelcomeWindowFirstNameRequired : null;

    public string? LastNameError => HasLastNameError ? Strings.WelcomeWindowLastNameRequired : null;

    public string? GroupError => HasGroupError ? Strings.WelcomeWindowGroupRequired : null;

    private bool CanConfirm() => !(HasFirstNameError || HasLastNameError || HasGroupError);

    /// <summary>
    /// Пересчитывает состояние проверки после изменения <paramref name="field"/>.
    /// </summary>
    private void RefreshValidation(string field)
    {
        OnPropertyChanged(field + "Error");
        OnPropertyChanged("Has" + field + "Error");

        // Кнопка подтверждения не должна позволять уйти с неполными данными.
        ConfirmCommand.NotifyCanExecuteChanged();
    }

    #endregion

    /// <summary>Пользователь подтвердил данные: переходим к главному окну.</summary>
    public event EventHandler? Confirmed;

    /// <summary>Пользователь отказался: закрываем приложение.</summary>
    public event EventHandler? ExitRequested;

    /// <summary>
    /// Подтверждение. Доступна только когда обязательные поля заполнены, поэтому
    /// проверка повторяется здесь ещё раз - на случай вызова из кода или теста.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanConfirm))]
    private void Confirm()
    {
        if (!CanConfirm())
            return;

        Confirmed?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Exit() => ExitRequested?.Invoke(this, EventArgs.Empty);

    // Название языка показываем на его же родном языке ("русский (Россия)"),
    // чтобы пользователь узнал язык независимо от текущей локали приложения.
    private static string Describe(string cultureName)
    {
        CultureInfo culture = CultureInfo.GetCultureInfo(cultureName);
        return culture.EnglishName == culture.NativeName
            ? culture.NativeName
            : $"{culture.EnglishName} ({culture.NativeName})";
    }

    private void InitializeSelection()
    {
        CultureOption? culture = AvailableCultures.FirstOrDefault(
            option => string.Equals(option.CultureName, _settings.CurrentCultureName, StringComparison.OrdinalIgnoreCase));

        if (culture is not null)
            SetProperty(ref _selectedCulture, culture, nameof(SelectedCulture));

        ThemeOption? theme = AvailableThemes.FirstOrDefault(option => option.Theme == _settings.CurrentAppTheme);

        if (theme is not null)
            SetProperty(ref _selectedTheme, theme, nameof(SelectedTheme));
    }

    private void OnCultureChanged(object? sender, ProTranslate.CultureChangedEventArgs e) => Dispatcher.UIThread.Post(() =>
    {
        // Культуру могли сменить извне - тогда ComboBox обязан показать фактическую.
        SyncSelectedCulture(_cultures.CurrentUICulture.Name);

        // ВАЖНО: Strings.* пересчитывают себя сами, а вот тексты ошибок - это
        // обычные CLR-свойства, вычисленные здесь же. Без ручного OnPropertyChanged
        // они навсегда останутся на стартовом языке.
        OnPropertyChanged(nameof(FirstNameError));
        OnPropertyChanged(nameof(LastNameError));
        OnPropertyChanged(nameof(GroupError));
    });

    private void SyncSelectedCulture(string cultureName)
    {
        CultureOption? match = AvailableCultures.FirstOrDefault(
            option => string.Equals(option.CultureName, cultureName, StringComparison.OrdinalIgnoreCase));

        if (match is not null && !ReferenceEquals(match, _selectedCulture))
            SelectedCulture = match;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _cultures.CultureChanged -= OnCultureChanged;

            // Наблюдаемые строки в названиях тем созданы этим классом, значит
            // и освобождать их должен он: ProTranslateStrings.Dispose() их не
            // трогает, и без этого они остались бы подписаны на переводы до
            // самого выхода из приложения.
            foreach (ThemeOption option in AvailableThemes)
                option.Dispose();
        }

        base.Dispose(disposing);
    }
}