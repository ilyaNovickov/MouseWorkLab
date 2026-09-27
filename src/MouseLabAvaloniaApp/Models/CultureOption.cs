using System.Globalization;

namespace MouseLabAvaloniaApp.Models;

public sealed class CultureOption
{
    public CultureOption(string cultureName, string displayName)
    {
        CultureName = cultureName;
        DisplayName = displayName;
    }

    public string CultureName { get; }

    public string DisplayName { get; }

    public CultureInfo Culture => CultureInfo.GetCultureInfo(CultureName);

    public override string ToString() => DisplayName;
}
