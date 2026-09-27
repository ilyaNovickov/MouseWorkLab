using System.Globalization;

namespace MouseLabAvaloniaApp.Models;

/// <summary>
/// Один пункт выпадающего списка языков.
/// </summary>
public sealed class CultureOption
{
    public CultureOption(string cultureName, string displayName)
    {
        CultureName = cultureName;
        DisplayName = displayName;
    }

    /// <summary>Код культуры в формате BCP-47, например "ru-RU".</summary>
    public string CultureName { get; }

    /// <summary>Название языка для интерфейса, например "русский (Россия)".</summary>
    public string DisplayName { get; }

    /// <summary>Готовый CultureInfo, который передаётся в ICultureService.SetCulture.</summary>
    public CultureInfo Culture => CultureInfo.GetCultureInfo(CultureName);

    public override string ToString() => DisplayName;
}
