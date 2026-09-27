using Avalonia.Threading;
using MouseLabAvaloniaApp.Models;
using MouseLabAvaloniaApp.Services.AppSettings;
using ProTranslate;
using ProTranslate.Generated;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MouseLabAvaloniaApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly ICultureService _cultures;
    private readonly IApplicationSettingsService _settings;
    private CultureOption? _selectedCulture;
    private string _userName = "Alex";

    public MainWindowViewModel(
        ITranslationService translations,
        ICultureService cultures,
        IApplicationSettingsService settings)
        : base(translations)
    {
        _cultures = cultures;
        _settings = settings;

        AvailableCultures =
        [
            new CultureOption("en-US", Describe("en-US")),
            new CultureOption("ru-RU", Describe("ru-RU")),
        ];

        _cultures.CultureChanged += OnCultureChanged;
        SyncSelectedCulture();
    }

    public IReadOnlyList<CultureOption> AvailableCultures { get; }

    public CultureOption? SelectedCulture
    {
        get => _selectedCulture;
        set
        {
            if (value is null || !SetProperty(ref _selectedCulture, value))
                return;

            _settings.CurrentCultureName = value.CultureName;
            _cultures.SetCulture(value.Culture);
        }
    }

    public string UserName
    {
        get => _userName;
        set
        {
            if (SetProperty(ref _userName, value))
                OnPropertyChanged(nameof(GreetingText));
        }
    }

    public string GreetingText => Translations.Format(ProTranslateKeys.AppGreetingWithName, _userName);

    public string CurrentCultureText =>
        Translations.Format(ProTranslateKeys.SettingsCurrentCulture, _cultures.CurrentUICulture.Name);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _cultures.CultureChanged -= OnCultureChanged;

        base.Dispose(disposing);
    }

    private void OnCultureChanged(object? sender, ProTranslate.CultureChangedEventArgs e) => Dispatcher.UIThread.Post(() =>
    {
        SyncSelectedCulture();

        // Every property this view model derives from Translations must be re-raised by hand:
        // Strings.* refresh themselves, but values computed here are plain CLR properties.
        OnPropertyChanged(nameof(GreetingText));
        OnPropertyChanged(nameof(CurrentCultureText));
    });

    private void SyncSelectedCulture()
    {
        CultureOption? match = AvailableCultures.FirstOrDefault(
            option => string.Equals(option.CultureName, _cultures.CurrentUICulture.Name, StringComparison.OrdinalIgnoreCase));

        if (match is not null && !ReferenceEquals(match, _selectedCulture))
            SelectedCulture = match;
    }

    private static string Describe(string cultureName)
    {
        CultureInfo culture = CultureInfo.GetCultureInfo(cultureName);
        return culture.EnglishName == culture.NativeName
            ? culture.NativeName
            : $"{culture.EnglishName} ({culture.NativeName})";
    }
}
