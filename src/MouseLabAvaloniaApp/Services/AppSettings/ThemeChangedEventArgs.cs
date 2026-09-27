using MouseLabAvaloniaApp.Models;
using System;

namespace MouseLabAvaloniaApp.Services.AppSettings;

public class ThemeChangedEventArgs : EventArgs
{
    public ThemeChangedEventArgs(Themes theme)
    {
        NewTheme = theme;
    }

    public Themes NewTheme { get; }
}
