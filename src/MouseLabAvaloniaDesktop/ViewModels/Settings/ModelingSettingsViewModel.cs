using CommunityToolkit.Mvvm.ComponentModel;
using MouseLabAvaloniaDesktop.Models;
using MouseLabAvaloniaDesktop.Models.Settings;
using MouseLabAvaloniaDesktop.Services.Implementations;
using ProTranslate;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MouseLabAvaloniaDesktop.ViewModels;

public partial class WorktopSizeViewModel : ViewModelBase
{
    public WorktopSizeViewModel(int size)
    {
        SizeResolution = size;
    }

    public int SizeResolution
    {
        get;
        set
        {
            if (value < 0)
                throw new ArgumentException("Размер не может быть отрицательным");
            SetProperty(ref field, value);
        }
    }

    public override string ToString()
    {
        return $"{SizeResolution}x{SizeResolution}";
    }
}

public partial class ModelingSettingsViewModel : TranslatedViewModelBase, ISettingsSectionViewModel
{
    private ObservableCollection<WorktopSizeViewModel> avaibleSizes;

    private readonly SettingsService _settings;

    //public ModelingSettingsViewModel(ITranslationService translation, IApplicationSettingsService settings) : base(translation)
    public ModelingSettingsViewModel(ITranslationService translation, SettingsService settings) : base(translation)
    {
        _settings = settings;

        int defaultResolution = ModelingSettings.DefaultWorktopResolution;

        avaibleSizes = [
            new WorktopSizeViewModel(defaultResolution),
            new WorktopSizeViewModel(2 * defaultResolution)
        ];

        //OnPropertyChanged(nameof(AvaibleSizes));
        //selectedSize = AvaibleSizes.First();
        isExperimentalOptions = _settings.Current.ModelingSettings.IsExperimentalSettings;
        selectedSize = AvaibleSizes.FirstOrDefault(
            s => s.SizeResolution == _settings.Current.ModelingSettings.WorktopResolution,
            AvaibleSizes[0]
            );

        // Рабочая копия стартует с значений, применённых сервисом, — иначе
        // Reset после Save вернул бы не то, что реально сохранено.
        
        //selectedSize = AvaibleSizes.First(s => s.SizeResolution == _settings.Current.ModelingSettings.WorktopResolution);
        //selectedSize = AvaibleSizes.First();
    }

    private bool isExperimentalOptions;
    public bool IsExperimentalOptions
    {
        get => isExperimentalOptions;
        set
        {
            SetProperty(ref isExperimentalOptions, value);
            OnIsExperimentalOptionsChanged();
        }
    }

    private void OnIsExperimentalOptionsChanged()
    {
        if (!IsExperimentalOptions)
        {
            selectedSize = AvaibleSizes.First();
            OnPropertyChanged(nameof(SelectedSize));
        }
    }

    public IReadOnlyList<WorktopSizeViewModel> AvaibleSizes => avaibleSizes;

    private WorktopSizeViewModel selectedSize;

    public WorktopSizeViewModel SelectedSize
    {
        get => selectedSize;
        set
        {
            // При IsExp == false всегда первый из списка
            if (!IsExperimentalOptions)
                return;
            if (!AvaibleSizes.Contains(value) || value is null)
                throw new ArgumentException("Аргумент должен быть из списка и не равень null");
            SetProperty(ref selectedSize, value);
        }
    }

    /// <summary>
    /// Публикует рабочую копию в сервис настроек (кнопка Save).
    /// </summary>
    public void Save()
    {
        
        _settings.Current.ModelingSettings.IsExperimentalSettings = IsExperimentalOptions;
        _settings.Current.ModelingSettings.WorktopResolution = SelectedSize.SizeResolution;
        
    }

    /// <summary>
    /// Откатывает рабочую копию к применённым значениям (кнопка Cancel).
    /// </summary>
    /// <remarks>
    /// Порядок важен: при IsExp == false сеттер SelectedSize молча игнорируется,
    /// поэтому сначала выставляется флаг, потом размер — иначе после открата
    /// с отключёнными экспериментами размер навсегда остался бы первым.
    /// </remarks>
    public void Reset()
    {
        /*
        IsExperimentalOptions = _settings.ExperimentalOptions;
        selectedSize = AvaibleSizes.FirstOrDefault(s => s.SizeResolution == _settings.WorktopResolution)
            ?? AvaibleSizes.First();
            */
        //OnPropertyChanged(nameof(SelectedSize));
    }
}
