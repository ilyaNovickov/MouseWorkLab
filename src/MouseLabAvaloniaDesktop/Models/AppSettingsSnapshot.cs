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
    [JsonPropertyName("theme")]
    public Themes Theme { get; init; } = Themes.Light;

    /// <summary>
    /// Код культуры. <c>null</c> означает «культура ещё не сохранялась» —
    /// инициализатора нет специально, чтобы дефолт не зависел от культуры процесса.
    /// </summary>
    [JsonPropertyName("culture")]
    public string? Culture { get; init; }

    [JsonPropertyName("worktopResolution")]
    public int WorktopResolution { get; init; } = 8192;

    [JsonPropertyName("experimental")]
    public bool Experimental { get; init; }
}
