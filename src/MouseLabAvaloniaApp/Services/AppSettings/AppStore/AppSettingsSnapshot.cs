using MouseLabAvaloniaApp.Models;
using System;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MouseLabAvaloniaApp.Services.AppSettings;

/// <summary>
/// Неизменяемый снимок настроек одного пользователя - ровно те данные, что лежат
/// в его <c>settings.json</c>.
/// </summary>
public sealed class AppSettingsSnapshot
{
    [JsonPropertyName("user")]
    public UserProfile? User { get; init; }

    /// <summary>
    /// Код культуры. <c>null</c> означает «взять умолчание приложения», поэтому
    /// здесь намеренно нет инициализатора: значение по умолчанию, равное
    /// <c>CultureInfo.CurrentCulture.Name</c>, завязало бы снимок на окружающую
    /// культуру процесса.
    /// </summary>
    [JsonPropertyName("culture")]
    public string? Culture { get; init; }

    [JsonPropertyName("theme")]
    public Themes Theme { get; init; } = Themes.Default;
}
