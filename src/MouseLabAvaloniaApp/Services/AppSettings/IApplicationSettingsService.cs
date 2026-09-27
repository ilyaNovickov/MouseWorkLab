using MouseLabAvaloniaApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvaloniaApp.Services.AppSettings
{
    public delegate void ThemeChangedEventHandler(object sender, ThemeChangedEventArgs e);

    public interface IApplicationSettingsService
    {
        Themes CurrentAppTheme { get; set; }

        event ThemeChangedEventHandler? ThemeChanged;
    }
}
