using MouseLabAvaloniaApp.Models;

namespace MouseLabAvaloniaApp.Services.AppSettings;

public delegate void ThemeChangedEventHandler(object sender, ThemeChangedEventArgs e);

public delegate void CultureChangedEventHandler(object sender, CultureChangedEventArgs e);

public interface IApplicationSettingsService
{
    Themes CurrentAppTheme { get; set; }

    string CurrentCultureName { get; set; }

    event ThemeChangedEventHandler? ThemeChanged;

    event CultureChangedEventHandler? CultureChanged;
}
