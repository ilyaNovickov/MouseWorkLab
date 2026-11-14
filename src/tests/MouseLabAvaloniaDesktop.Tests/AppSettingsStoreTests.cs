using Xunit;
using MouseLabAvaloniaDesktop.Models;
using MouseLabAvaloniaDesktop.Services.AppSettings;
using System.Globalization;
using System.IO;

namespace MouseLabAvaloniaDesktop.Tests;

public class AppSettingsStoreTests
{
    private static string TempPath() =>
        Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

    [Fact]
    public void SaveThenLoadRoundTripsAllValues()
    {
        string path = TempPath();
        try
        {
            AppSettingsSnapshot saved = new()
            {
                Theme = Themes.Dark,
                Culture = "ru-RU",
                WorktopResolution = 16384,
                Experimental = true
            };

            AppSettingsStore.Save(path, saved);
            AppSettingsSnapshot loaded = AppSettingsStore.Load(path);

            Assert.Equal(Themes.Dark, loaded.Theme);
            Assert.Equal("ru-RU", loaded.Culture);
            Assert.Equal(16384, loaded.WorktopResolution);
            Assert.True(loaded.Experimental);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void LoadMissingFileReturnsDefaults()
    {
        AppSettingsSnapshot loaded = AppSettingsStore.Load(TempPath());

        Assert.Equal(Themes.Light, loaded.Theme);
        Assert.Null(loaded.Culture);
        Assert.Equal(8192, loaded.WorktopResolution);
        Assert.False(loaded.Experimental);
    }

    [Fact]
    public void LoadCorruptFileReturnsDefaults()
    {
        string path = TempPath();
        try
        {
            File.WriteAllText(path, "not json");

            AppSettingsSnapshot loaded = AppSettingsStore.Load(path);

            Assert.Equal(Themes.Light, loaded.Theme);
            Assert.Null(loaded.Culture);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("xx-XX")]
    public void ResolveCultureFallsBackToDefault(string? cultureName)
    {
        Assert.Equal("ru-RU", AppSettingsStore.ResolveCulture(cultureName).Name);
    }

    [Fact]
    public void ResolveCultureParsesValidName()
    {
        Assert.Equal("en-US", AppSettingsStore.ResolveCulture("en-US").Name);
    }
}
