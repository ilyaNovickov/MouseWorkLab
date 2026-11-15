using System.Text.Json.Serialization;

namespace MouseLabAvaloniaDesktop.Models;

/// <summary>
/// Неизменяемый снимок настроек — ровно те данные, что лежат в файле настроек.
/// </summary>
/// <remarks>
/// Имя и расположение файла выбирает будущий проектный слой («проект»
/// пользователя), поэтому снимок знает только значения, а не путь.
/// </remarks>
public sealed class AppSettingsSnapshot
{
    public Themes Theme { get; init; } = Themes.Light;

    /// <summary>
    /// Код культуры. <c>null</c> означает «культура ещё не сохранялась» —
    /// инициализатора нет специально, чтобы дефолт не зависел от культуры процесса.
    /// </summary>
    public string? Culture { get; init; }

    public int WorktopResolution { get; init; } = 8192;

    public bool Experimental { get; init; }
}
