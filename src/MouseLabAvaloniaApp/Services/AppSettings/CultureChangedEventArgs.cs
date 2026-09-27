using System;
using System.Globalization;

namespace MouseLabAvaloniaApp.Services.AppSettings;

public class CultureChangedEventArgs : EventArgs
{
    public CultureChangedEventArgs(CultureInfo culture)
    {
        NewCulture = culture;
    }

    public CultureInfo NewCulture { get; }
}
