using MouseLabAvaloniaDesktop.Models;
using MouseLabAvaloniaDesktop.Services.AppSettings;
using System.IO;
using Xunit;

namespace MouseLabAvaloniaDesktop.Tests;

public class ApplicationSettingsServiceTests
{
    private static string TempPath() =>
        Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

    private static AppSettingsSnapshot Snapshot() => new()
    {
        Theme = Themes.Light,
        WorktopResolution = 8192
    };

    private static (ApplicationSettingsService Service, Counter Counter) Counted() =>
        (new ApplicationSettingsService(Snapshot()), new Counter());

    private sealed class Counter
    {
        public int Theme;
        public int Culture;
        public int Modeling;

        public void Hook(IApplicationSettingsService service)
        {
            service.ThemeChanged += (_, _) => Theme++;
            service.CultureChanged += (_, _) => Culture++;
            service.ModelingChanged += (_, _) => Modeling++;
        }
    }

    [Fact]
    public void CtorAssignsSnapshotValuesWithoutEvents()
    {
        var service = new ApplicationSettingsService(new AppSettingsSnapshot
        {
            Theme = Themes.Dark,
            Culture = "ru-RU",
            WorktopResolution = 16384,
            Experimental = true
        });
        var counter = new Counter();
        counter.Hook(service);

        Assert.Equal(Themes.Dark, service.CurrentAppTheme);
        Assert.Equal("ru-RU", service.CurrentCultureName);
        Assert.Equal(16384, service.WorktopResolution);
        Assert.True(service.ExperimentalOptions);
        Assert.Equal(0, counter.Theme);
        Assert.Equal(0, counter.Culture);
        Assert.Equal(0, counter.Modeling);
    }

    [Fact]
    public void SameValueSetRaisesNothing()
    {
        (var service, var counter) = Counted();
        counter.Hook(service);

        service.CurrentAppTheme = Themes.Light;
        service.CurrentCultureName = "ru-RU";
        service.WorktopResolution = 8192;
        service.ExperimentalOptions = false;

        Assert.Equal(0, counter.Theme);
        Assert.Equal(0, counter.Culture);
        Assert.Equal(0, counter.Modeling);
    }

    [Fact]
    public void ChangedValuesRaiseEventsWithNewValues()
    {
        (var service, var counter) = Counted();
        counter.Hook(service);

        service.CurrentAppTheme = Themes.Dark;
        service.CurrentCultureName = "en-US";
        service.WorktopResolution = 16384;
        service.ExperimentalOptions = true;

        Assert.Equal(1, counter.Theme);
        Assert.Equal(1, counter.Culture);
        // Событие на каждое реально изменившееся значение: два свойства
        // моделирования — два события (порядок: Experimental, затем Size).
        Assert.Equal(2, counter.Modeling);
        Assert.Equal(Themes.Dark, service.CurrentAppTheme);
        Assert.Equal("en-US", service.CurrentCultureName);
        Assert.Equal(16384, service.WorktopResolution);
        Assert.True(service.ExperimentalOptions);
    }

    [Fact]
    public void WithoutFilePathNothingIsWritten()
    {
        string directory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(directory);
        try
        {
            var service = new ApplicationSettingsService(Snapshot());
            service.CurrentAppTheme = Themes.Dark;
            service.WorktopResolution = 16384;

            Assert.Empty(Directory.GetFiles(directory));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public void LoadFromBindsPathAndCreatesFile()
    {
        string path = TempPath();
        try
        {
            var service = new ApplicationSettingsService(Snapshot());
            service.LoadFrom(new AppSettingsSnapshot
            {
                Theme = Themes.Dark,
                Culture = "en-US",
                WorktopResolution = 16384,
                Experimental = true
            }, path);

            Assert.True(File.Exists(path));

            AppSettingsSnapshot loaded = AppSettingsStore.Load(path);
            Assert.Equal(Themes.Dark, loaded.Theme);
            Assert.Equal("en-US", loaded.Culture);
            Assert.Equal(16384, loaded.WorktopResolution);
            Assert.True(loaded.Experimental);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void ChangeAfterLoadFromRewritesFile()
    {
        string path = TempPath();
        try
        {
            var service = new ApplicationSettingsService(Snapshot());
            service.LoadFrom(Snapshot(), path);

            service.WorktopResolution = 16384;

            Assert.Equal(16384, AppSettingsStore.Load(path).WorktopResolution);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void CancelKeepsSavedValues()
    {
        // Сценарий Cancel: секция Save-ом сохранила значения, затем Reset-ом
        // откатывает рабочую копию к значениям сервиса. Сохранённое обязано
        // остаться применённым, файл обязан содержать сохранённое.
        string path = TempPath();
        try
        {
            var service = new ApplicationSettingsService(Snapshot());
            service.LoadFrom(Snapshot(), path);

            service.WorktopResolution = 16384;
            service.ExperimentalOptions = true;

            Assert.Equal(16384, service.WorktopResolution);
            Assert.True(service.ExperimentalOptions);
            Assert.Equal(16384, AppSettingsStore.Load(path).WorktopResolution);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
