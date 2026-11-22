using System;
using System.ComponentModel;
using System.Globalization;
using ProTranslate;

namespace MouseLab.Tests;

internal sealed class FakeTranslationService : ITranslationService
{
    public event EventHandler<CultureChangedEventArgs>? CultureChanged
    {
        add { }
        remove { }
    }

    public event EventHandler<ProTranslateDiagnosticEventArgs>? DiagnosticReported
    {
        add { }
        remove { }
    }

    public CultureInfo CurrentCulture { get; } = CultureInfo.InvariantCulture;

    public CultureInfo CurrentUICulture { get; } = CultureInfo.InvariantCulture;

    public LocalizedString this[string key] => GetString(key);

    public LocalizedString GetString(string key) =>
        new(key, key, CurrentUICulture, false, "test");

    public LocalizedString GetString(string key, CultureInfo culture) =>
        new(key, key, culture, false, "test");

    public string Format(string key, params object?[] arguments) => key;

    public IObservableLocalizedString Observe(string key, params object?[] arguments) =>
        new FakeObservableLocalizedString(key);
}

internal sealed class FakeObservableLocalizedString : IObservableLocalizedString, INotifyPropertyChanged
{
    public FakeObservableLocalizedString(string key)
    {
        Key = key;
        LocalizedString = new LocalizedString(key, key, CultureInfo.InvariantCulture, false, "test");
    }

    public event PropertyChangedEventHandler? PropertyChanged
    {
        add { }
        remove { }
    }

    public string Key { get; }

    public LocalizedString LocalizedString { get; }

    public string Value => LocalizedString.Value;

    public CultureInfo Culture => LocalizedString.Culture;

    public void UpdateArguments(object?[] arguments)
    {
    }

    public void Dispose()
    {
    }
}
