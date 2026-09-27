using CommunityToolkit.Mvvm.ComponentModel;
using ProTranslate;
using ProTranslate.Generated;
using System;

namespace MouseLabAvaloniaApp.ViewModels;

public abstract class ViewModelBase : ObservableObject, IDisposable
{
    private bool _disposed;

    protected ViewModelBase(ITranslationService translations)
    {
        Translations = translations;
        Strings = new ProTranslateStrings(translations);
    }

    public ProTranslateStrings Strings { get; }

    public ITranslationService Translations { get; }

    // Escape hatches for keys that are only known at runtime (provider-driven or generated key
    // composition). Prefer the generated Strings surface or ProTranslateKeys constants, which are
    // validated at build time by PTA001/PTA002.
#pragma warning disable PTA004
    protected string T(string key) => Translations[key].Value;

    protected string T(string key, params object?[] arguments) => Translations.Format(key, arguments);
#pragma warning restore PTA004

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
            Strings.Dispose();

        _disposed = true;
    }
}
