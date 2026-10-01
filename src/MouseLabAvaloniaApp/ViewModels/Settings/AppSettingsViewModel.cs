using Avalonia.Threading;
using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualBasic;
using MouseLabAvaloniaApp.Models;
using MouseLabAvaloniaApp.Services.AppSettings;
using ProTranslate;
using ProTranslate.Generated;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime;
using System.Text;

namespace MouseLabAvaloniaApp.ViewModels.Settings
{
    public partial class AppSettingsViewModel : ViewModelBase
    {
        private readonly ICultureService _cultures;
        private readonly IApplicationSettingsService _settings;
        private CultureOption? _selectedCulture;

        private ThemeOption? _selectedTheme;

        public AppSettingsViewModel(
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
                new ThemeOption(Strings.Observe_SettingsDarkTheme(), Themes.Dark),
                new ThemeOption(Strings.Observe_SettingsLightTheme(), Themes.Light),
                new ThemeOption(Strings.Observe_SettingsDefaultTheme(), Themes.Default)
            ];
            // Подписываемся на смену культуры, чтобы пересчитать собственные вычисляемые
            // свойства. Отписка - в Dispose.
            _cultures.CultureChanged += OnCultureChanged;



            // Выставляем в ComboBox язык, который реально применён (из настроек),
            // а не первый в списке.
            SyncSelectedCulture();
            SyncSelectedTheme();
        }

        public IReadOnlyList<CultureOption> AvailableCultures { get; }

        public ObservableCollection<ThemeOption> AvailableThemes { get; }

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
                //_settings.CurrentCultureName = value.CultureName;
                //_cultures.SetCulture(value.Culture);
            }
        }

        public ThemeOption? SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if (value is null || !SetProperty(ref _selectedTheme, value))
                    return;

                //_selectedTheme = value;
            }
        }

        private void OnCultureChanged(object? sender, ProTranslate.CultureChangedEventArgs e) => Dispatcher.UIThread.Post(() =>
        {
            SyncSelectedCulture();

            // ВАЖНО: Strings.* пересчитывают себя сами, а вот обычные CLR-свойства,
            // вычисленные в этой модели, об этом не знают. Без ручного OnPropertyChanged
            // они навсегда останутся на стартовом языке.
            //OnPropertyChanged(nameof(GreetingText));
            //OnPropertyChanged(nameof(CurrentCultureText));
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

        private void SyncSelectedCultureToSettings()
        {
            if (this.SelectedCulture is null)
                return;

            _settings.CurrentCultureName = this.SelectedCulture.CultureName;
            _cultures.SetCulture(this.SelectedCulture.Culture);
        }

        private void SyncSelectedTheme()
        {
            ThemeOption? match = AvailableThemes.FirstOrDefault(option => option.Theme == _settings.CurrentAppTheme);

            if (match is not null && match != SelectedTheme)
                SelectedTheme = match;
        }

        private void SyncSelectedThemeToSettings()
        {
            if (this.SelectedTheme is null)
                return;

            _settings.CurrentAppTheme = this.SelectedTheme.Theme;
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _cultures.CultureChanged -= OnCultureChanged;

            base.Dispose(disposing);
        }

        [RelayCommand]
        private void SaveSettings()
        {
            SyncSelectedCultureToSettings();
            SyncSelectedThemeToSettings();
        }
    }
}
