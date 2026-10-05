using MouseLabAvaloniaApp.Models;

namespace MouseLabAvaloniaApp.Services.AppSettings;

/// <summary>Сигнатура события смены темы.</summary>
public delegate void ThemeChangedEventHandler(object sender, ThemeChangedEventArgs e);

/// <summary>Сигнатура события смены языка в настройках.</summary>
public delegate void CultureChangedEventHandler(object sender, CultureChangedEventArgs e);

/// <summary>
/// Пользовательские настройки приложения.
/// </summary>
/// <remarks>
/// Существует отдельно от <c>ProTranslate.ICultureService</c> намеренно:
/// ICultureService отвечает за текущую культуру в рантайме и ничего не знает
/// о хранении, а этот сервис помнит выбор пользователя между запусками.
/// </remarks>
public interface IApplicationSettingsService
{
    /// <summary>Выбранная тема оформления.</summary>
    Themes CurrentAppTheme { get; set; }

    /// <summary>Код выбранной культуры, например "en-US" или "ru-RU".</summary>
    string CurrentCultureName { get; set; }

    /// <summary>
    /// Заменяет текущие значения на значения из снимка, поднимая события только
    /// для реально изменившихся полей.
    /// </summary>
    /// <remarks>
    /// Нужна для смены настроек на лету: окно приветствия собирает выбор во
    /// временном хранилище, а главное окно запускается уже с ним. Когда у
    /// каждого пользователя будут свои настройки, источником станет
    /// <c>%LOCALAPPDATA%\MouseLab\{userhash}\settings.json</c>.
    /// </remarks>
    void LoadFrom(AppSettingsSnapshot snapshot);

    event ThemeChangedEventHandler? ThemeChanged;

    event CultureChangedEventHandler? CultureChanged;
}
