using System.Text.Json.Serialization;

namespace MouseLabAvaloniaApp.Models;

/// <summary>
/// Темы оформления приложения.
/// </summary>
/// <remarks>
/// <see cref="JsonConverterAttribute"/> с обобщённым JsonStringEnumConverter&lt;T&gt;
/// обязателен: он даёт AOT-совместимое чтение и запись перечисления как строки
/// ("Dark" в settings.json). Обычный JsonStringEnumConverter помечен IL3050,
/// потому что использует кодогенерацию в рантайме.
/// </remarks>
[JsonConverter(typeof(JsonStringEnumConverter<Themes>))]
public enum Themes
{
    /// <summary>Светлая тема.</summary>
    Light,

    /// <summary>Тёмная тема.</summary>
    Dark,

    /// <summary>Следовать теме операционной системы.</summary>
    Default
}
