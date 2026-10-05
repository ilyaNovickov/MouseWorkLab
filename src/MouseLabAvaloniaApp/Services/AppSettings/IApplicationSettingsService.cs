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
    /// для реально изменившихся полей, и привязывает файл, в который пойдут
    /// дальнейшие сохранения.
    /// </summary>
    /// <param name="snapshot">Новые значения.</param>
    /// <param name="filePath">
    /// Путь к настройкам пользователя. На старте приложения пользователя ещё нет,
    /// поэтому путь известен только здесь - в момент подтверждения данных.
    /// </param>
    /// <remarks>
    /// Когда у пользователей появятся свои настройки, источником станет
    /// <c>%LOCALAPPDATA%\MouseLab\{slug}-{hash}\settings.json</c>.
    /// </remarks>
    void LoadFrom(AppSettingsSnapshot snapshot, string filePath);

    event ThemeChangedEventHandler? ThemeChanged;

    event CultureChangedEventHandler? CultureChanged;
}
