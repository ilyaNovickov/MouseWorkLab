using MouseLabAvaloniaApp.Models;
using System;

namespace MouseLabAvaloniaApp.Services.AppSettings;

/// <summary>
/// Состояние пользовательских настроек в памяти + автоматическое сохранение в файл.
/// </summary>
public sealed class ApplicationSettingsService : IApplicationSettingsService
{
    private string _currentCultureName;

    public ApplicationSettingsService(AppSettingsSnapshot snapshot)
    {
        // Стартовые значения выставляем напрямую в поля, не поднимая события:
        // до подписки обработчиков это всё равно никто не услышит, а лишняя
        // запись в settings.json при каждом запуске не нужна.
        CurrentAppTheme = snapshot.Theme;
        _currentCultureName = AppSettingsStore.ResolveCulture(snapshot.Culture).Name;

        // Одна и та же реакция на оба события: перезаписать файл настроек целиком.
        // Сам снимок хранит и тему, и культуру, поэтому сохраняются оба значения.
        ThemeChanged += OnSettingChanged;
        CultureChanged += OnSettingChanged;
    }

    public Themes CurrentAppTheme
    {
        get;
        set
        {
            // Игнорируем повторную установку того же значения, иначе событие
            // сработает без причины.
            if (field == value)
                return;

            field = value;
            ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(value));
        }
    }

    /// <summary>
    /// Код выбранной культуры ("ru-RU"). Значение нормализуется через
    /// <see cref="AppSettingsStore.ResolveCulture"/>, поэтому неизвестная или
    /// пустая строка превращается в культуру по умолчанию, а не в ошибку.
    /// </summary>
    public string CurrentCultureName
    {
        get => _currentCultureName;
        set
        {
            string resolved = AppSettingsStore.ResolveCulture(value).Name;
            if (_currentCultureName == resolved)
                return;

            _currentCultureName = resolved;
            CultureChanged?.Invoke(this, new CultureChangedEventArgs(AppSettingsStore.ResolveCulture(resolved)));
        }
    }

    public event ThemeChangedEventHandler? ThemeChanged;

    public event CultureChangedEventHandler? CultureChanged;

    private void OnSettingChanged(object sender, EventArgs e) =>
        AppSettingsStore.Save(new AppSettingsSnapshot
        {
            Culture = _currentCultureName,
            Theme = CurrentAppTheme,
        });
}
