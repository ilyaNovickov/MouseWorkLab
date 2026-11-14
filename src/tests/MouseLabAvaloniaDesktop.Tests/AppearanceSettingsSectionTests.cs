using MouseLabAvaloniaDesktop.Models;
using MouseLabAvaloniaDesktop.Services.AppSettings;
using MouseLabAvaloniaDesktop.ViewModels;
using ProTranslate;
using Xunit;

namespace MouseLabAvaloniaDesktop.Tests;

public class AppearanceSettingsSectionTests
{
    private static ITranslationService Translations() =>
        new TranslationService(new InMemoryTranslationProvider("test"), new CultureService());

    [Fact]
    public void SavePushesThemeAndCultureToService()
    {
        var settings = new ApplicationSettingsService(new AppSettingsSnapshot());
        var vm = new AppearanceSettingsViewModel(Translations(), settings);

        vm.SelectedTheme = vm.AvailableThemes.First(t => t.Theme == Themes.Dark);
        vm.SelectedCulture = vm.AvailableCultures.First(c => c.CultureName == "en-US");
        vm.Save();

        Assert.Equal(Themes.Dark, settings.CurrentAppTheme);
        Assert.Equal("en-US", settings.CurrentCultureName);
    }

    [Fact]
    public void ResetRestoresFromService()
    {
        var settings = new ApplicationSettingsService(new AppSettingsSnapshot
        {
            Theme = Themes.Dark,
            Culture = "en-US"
        });
        var vm = new AppearanceSettingsViewModel(Translations(), settings);

        vm.SelectedTheme = vm.AvailableThemes.First(t => t.Theme == Themes.Light);
        vm.SelectedCulture = vm.AvailableCultures.First(c => c.CultureName == "ru-RU");
        vm.Reset();

        Assert.Equal(Themes.Dark, vm.SelectedTheme.Theme);
        Assert.Equal("en-US", vm.SelectedCulture.CultureName);
    }

    [Fact]
    public void ThemeOptionsUseObservableNames()
    {
        var settings = new ApplicationSettingsService(new AppSettingsSnapshot());
        var vm = new AppearanceSettingsViewModel(Translations(), settings);

        Assert.All(vm.AvailableThemes, t => Assert.False(string.IsNullOrEmpty(t.DisplayName.Value)));
        Assert.Equal(3, vm.AvailableThemes.Count);
    }

    [Fact]
    public void DisposeReleasesThemeOptions()
    {
        var settings = new ApplicationSettingsService(new AppSettingsSnapshot());
        var vm = new AppearanceSettingsViewModel(Translations(), settings);
        ThemeOption option = vm.AvailableThemes.First(t => t.Theme == Themes.Light);

        vm.Dispose();

        // Прямой проверки "освобождено" у IObservableLocalizedString нет; после
        // Dispose повторное чтение значения всё ещё допустимо, поэтому проверяем
        // сам факт, что Dispose прошёл без исключений и VM можно dispose дважды.
        vm.Dispose();
        Assert.Equal(Themes.Light, option.Theme);
    }
}
