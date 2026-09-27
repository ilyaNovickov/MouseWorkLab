using MouseLabAvaloniaApp.Models;
using System;

namespace MouseLabAvaloniaApp.Services.AppSettings;

/// <summary>Аргументы события смены темы.</summary>
public class ThemeChangedEventArgs : EventArgs
{
    public ThemeChangedEventArgs(Themes theme)
    {
        NewTheme = theme;
    }

    /// <summary>Тема, которая была выбрана.</summary>
    public Themes NewTheme { get; }
}
