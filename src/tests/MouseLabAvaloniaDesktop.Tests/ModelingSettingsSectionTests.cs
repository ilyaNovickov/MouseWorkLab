using MouseLabAvaloniaDesktop.Models;
using MouseLabAvaloniaDesktop.Services.AppSettings;
using MouseLabAvaloniaDesktop.ViewModels;
using ProTranslate;
using System.IO;
using System.Linq;
using Xunit;

namespace MouseLabAvaloniaDesktop.Tests;

public class ModelingSettingsSectionTests
{
    private static ITranslationService Translations() =>
        new TranslationService(new InMemoryTranslationProvider("test"), new CultureService());

    [Fact]
    public void ResetRestoresExperimentalFirstThenSize()
    {
        var settings = new ApplicationSettingsService(new AppSettingsSnapshot
        {
            Experimental = true,
            WorktopResolution = 16384
        });
        var vm = new ModelingSettingsViewModel(Translations(), settings);

        // Рабочая копия разъехалась с сервисом, как это бывает перед Cancel.
        vm.IsExperimentalOptions = false;
        vm.SelectedSize = vm.AvaibleSizes.First();

        vm.Reset();

        Assert.True(vm.IsExperimentalOptions);
        Assert.Equal(16384, vm.SelectedSize.SizeResolution);
    }

    [Fact]
    public void SavePushesValuesToService()
    {
        var settings = new ApplicationSettingsService(new AppSettingsSnapshot());
        var vm = new ModelingSettingsViewModel(Translations(), settings);

        vm.IsExperimentalOptions = true;
        vm.SelectedSize = vm.AvaibleSizes.Last();
        vm.Save();

        Assert.True(settings.ExperimentalOptions);
        Assert.Equal(16384, settings.WorktopResolution);
    }
}
