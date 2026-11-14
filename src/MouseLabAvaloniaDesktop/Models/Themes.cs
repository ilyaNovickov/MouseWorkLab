using System.Text.Json.Serialization;

namespace MouseLabAvaloniaDesktop.Models;

/// <summary>
/// Темы оформления приложения.
/// </summary>
/// <remarks>
/// Обобщённый JsonStringEnumConverter&lt;T&gt; обязателен для NativeAOT:
/// обычный JsonStringEnumConverter использует кодогенерацию в рантайме (IL3050).
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
