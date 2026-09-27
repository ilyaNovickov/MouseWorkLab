using MouseLabAvaloniaApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvaloniaApp.Services.AppSettings
{
    public class ThemeChangedEventArgs : EventArgs
    {
        public ThemeChangedEventArgs(Themes theme)
        {
            this.NewTheme = theme;
        }

        public Themes NewTheme { get; }
    }
}
