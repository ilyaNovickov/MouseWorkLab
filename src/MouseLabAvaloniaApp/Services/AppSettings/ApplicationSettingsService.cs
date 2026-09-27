using MouseLabAvaloniaApp.Models;
using System;

namespace MouseLabAvaloniaApp.Services.AppSettings;

public sealed class ApplicationSettingsService : IApplicationSettingsService
{
    private string _currentCultureName;

    public ApplicationSettingsService(AppSettingsSnapshot snapshot)
    {
        CurrentAppTheme = snapshot.Theme;
        _currentCultureName = AppSettingsStore.ResolveCulture(snapshot.Culture).Name;

        ThemeChanged += OnSettingChanged;
        CultureChanged += OnSettingChanged;
    }

    public Themes CurrentAppTheme
    {
        get;
        set
        {
            if (field == value)
                return;

            field = value;
            ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(value));
        }
    }

    public string CurrentCultureName
    {
        get => _currentCultureName;
        set
        {
            string resolved = AppSettingsStore.ResolveCulture(value).Name;
            if (_currentCultureName == resolved)
                return;

            _currentCultureName = resolved;
            CultureChanged?.Invoke(this, new CultureChangedEventArgs(AppSettingsStore.ResolveCulture(resolved)));
        }
    }

    public event ThemeChangedEventHandler? ThemeChanged;

    public event CultureChangedEventHandler? CultureChanged;

    private void OnSettingChanged(object sender, EventArgs e) =>
        AppSettingsStore.Save(new AppSettingsSnapshot
        {
            Culture = _currentCultureName,
            Theme = CurrentAppTheme,
        });
}
