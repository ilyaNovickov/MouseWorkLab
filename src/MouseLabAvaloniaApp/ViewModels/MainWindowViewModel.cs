using Avalonia.Threading;
using MouseLabAvaloniaApp.Models;
using MouseLabAvaloniaApp.Services.AppSettings;
using ProTranslate;
using ProTranslate.Generated;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MouseLabAvaloniaApp.ViewModels;

/// <summary>
/// Модель представления главного окна: демонстрирует все три способа вывода
/// локализованного текста и содержит переключатель языка.
/// </summary>
public class MainWindowViewModel : ViewModelBase
{
    private readonly ICultureService _cultures;
    private readonly IApplicationSettingsService _settings;
    private CultureOption? _selectedCulture;
    private string _userName = "Alex";

    public MainWindowViewModel(
        ITranslationService translations,
        ICultureService cultures,
        IApplicationSettingsService settings)
        : base(translations)
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

        // Подписываемся на смену культуры, чтобы пересчитать собственные вычисляемые
        // свойства. Отписка - в Dispose.
        _cultures.CultureChanged += OnCultureChanged;

        // Выставляем в ComboBox язык, который реально применён (из настроек),
        // а не первый в списке.
        SyncSelectedCulture();
    }

    public IReadOnlyList<CultureOption> AvailableCultures { get; }

    /// <summary>
    /// Выбранный язык. Двусторонне привязан к ComboBox.SelectedItem в XAML.
    /// </summary>
    public CultureOption? SelectedCulture
    {
        get => _selectedCulture;
        set
        {
            if (value is null || !SetProperty(ref _selectedCulture, value))
                return;

            // Порядок важен: сначала сохраняем выбор, потом переключаем культуру -
            // так настройка успеет записаться даже если что-то пойдёт не так.
            _settings.CurrentCultureName = value.CultureName;
            _cultures.SetCulture(value.Culture);
        }
    }

    public string UserName
    {
        get => _userName;
        set
        {
            // SetProperty сам поднимет PropertyChanged для UserName, но GreetingText
            // зависит от него косвенно, поэтому уведомляем о нём вручную.
            if (SetProperty(ref _userName, value))
                OnPropertyChanged(nameof(GreetingText));
        }
    }

    // Вычисляемое свойство: собирает строку из перевода и данных предметной области.
    // ProTranslateStrings так не умеет - там только готовые ключи, поэтому форматирование
    // выполняется здесь, через Translations.Format с константой ключа.
    // Плейсхолдер {0} в каталоге проверяется анализатором PTA002.
    public string GreetingText => Translations.Format(ProTranslateKeys.AppGreetingWithName, _userName);

    public string CurrentCultureText =>
        Translations.Format(ProTranslateKeys.SettingsCurrentCulture, _cultures.CurrentUICulture.Name);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _cultures.CultureChanged -= OnCultureChanged;

        base.Dispose(disposing);
    }

    private void OnCultureChanged(object? sender, ProTranslate.CultureChangedEventArgs e) => Dispatcher.UIThread.Post(() =>
    {
        SyncSelectedCulture();

        // ВАЖНО: Strings.* пересчитывают себя сами, а вот обычные CLR-свойства,
        // вычисленные в этой модели, об этом не знают. Без ручного OnPropertyChanged
        // они навсегда останутся на стартовом языке.
        OnPropertyChanged(nameof(GreetingText));
        OnPropertyChanged(nameof(CurrentCultureText));
    });

    // Синхронизирует выбранный в ComboBox язык с реально применённой культурой.
    // Сравнение по ссылке (!ReferenceEquals) защищает от лишнего переустановления
    // свойства, а значит и от рекурсии: SelectedCulture -> SetCulture -> CultureChanged.
    private void SyncSelectedCulture()
    {
        CultureOption? match = AvailableCultures.FirstOrDefault(
            option => string.Equals(option.CultureName, _cultures.CurrentUICulture.Name, StringComparison.OrdinalIgnoreCase));

        if (match is not null && !ReferenceEquals(match, _selectedCulture))
            SelectedCulture = match;
    }

    // Название языка показываем на его же родном языке ("русский (Россия)"),
    // чтобы пользователь узнал язык независимо от текущей локали приложения.
    private static string Describe(string cultureName)
    {
        CultureInfo culture = CultureInfo.GetCultureInfo(cultureName);
        return culture.EnglishName == culture.NativeName
            ? culture.NativeName
            : $"{culture.EnglishName} ({culture.NativeName})";
    }
}
