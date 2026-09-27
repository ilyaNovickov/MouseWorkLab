using MouseLabAvaloniaApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvaloniaApp.Services.AppSettings
{
    public class ApplicationSettingsService : IApplicationSettingsService
    {
        public Themes CurrentAppTheme 
        { 
            get; 
            set
            {
                field = value;
                ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(field));
            }
        }

        public event ThemeChangedEventHandler? ThemeChanged;

    }
}
