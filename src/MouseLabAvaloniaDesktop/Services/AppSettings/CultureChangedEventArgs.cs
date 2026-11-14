using System;
using System.Globalization;

namespace MouseLabAvaloniaDesktop.Services.AppSettings;

/// <summary>Аргументы события смены языка в настройках.</summary>
/// <remarks>
/// Это событие приложения, а не <c>ProTranslate.CultureChangedEventArgs</c>:
/// оно сообщает о смене настройки, а не о применении культуры в UI.
/// </remarks>
public class CultureChangedEventArgs : EventArgs
{
    public CultureChangedEventArgs(CultureInfo culture)
    {
        NewCulture = culture;
    }

    /// <summary>Культура, которая была выбрана в настройках.</summary>
    public CultureInfo NewCulture { get; }
}
