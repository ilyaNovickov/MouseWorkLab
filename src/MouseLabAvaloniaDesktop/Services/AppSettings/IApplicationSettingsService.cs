using MouseLabAvaloniaDesktop.Models;
using System;

namespace MouseLabAvaloniaDesktop.Services.AppSettings;

/// <summary>
/// Пользовательские настройки приложения.
/// </summary>
/// <remarks>
/// Существует отдельно от <c>ProTranslate.ICultureService</c> намеренно:
/// ICultureService отвечает за текущую культуру в рантайме и ничего не знает
/// о хранении, а этот сервис помнит выбор пользователя. Имя и расположение
/// файла выбирает будущий проектный слой, поэтому до <see cref="LoadFrom"/>
/// сервис работает только в памяти.
/// </remarks>
public interface IApplicationSettingsService
{
    /// <summary>Выбранная тема оформления.</summary>
    Themes CurrentAppTheme { get; set; }

    /// <summary>Код выбранной культуры, например "en-US" или "ru-RU".</summary>
    string CurrentCultureName { get; set; }

    /// <summary>Разрешение полотна моделирования.</summary>
    int WorktopResolution { get; set; }

    /// <summary>Экспериментальные настройки моделирования.</summary>
    bool ExperimentalOptions { get; set; }

    /// <summary>
    /// Заменяет текущие значения на значения из снимка и привязывает файл,
    /// в который пойдут дальнейшие сохранения.
    /// </summary>
    /// <param name="snapshot">Новые значения.</param>
    /// <param name="filePath">
    /// Путь к файлу настроек. Передаёт его проектный слой, когда пользователь
    /// откроет или создаст «проект». <c>null</c> оставляет работу в памяти.
    /// </param>
    void LoadFrom(AppSettingsSnapshot snapshot, string? filePath);

    event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

    event EventHandler<CultureChangedEventArgs>? CultureChanged;

    event EventHandler<ModelingChangedEventArgs>? ModelingChanged;
}
