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

public partial class ModelingSettingsViewModel : TranslatedViewModelBase, ISettingsSectionViewModel
{
    private ObservableCollection<WorktopSizeViewModel> avaibleSizes;

    private readonly ProjectService _projectService;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
    public ModelingSettingsViewModel(ITranslationService translation, ProjectService projectService) : base(translation)
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
    {
        _projectService = projectService;

        int defaultResolution = ModelingSettings.DefaultWorktopResolution;

        avaibleSizes = [
            new WorktopSizeViewModel(defaultResolution),
            new WorktopSizeViewModel(2 * defaultResolution)
        ];

        _projectService.ProjectLoaded += _projectService_ProjectLoaded;

        _projectService_ProjectLoaded(null, EventArgs.Empty);
    }

    private void _projectService_ProjectLoaded(object? sender, EventArgs e)
    {
        isExperimentalOptions = _projectService.ModelingSettings.IsExperimentalSettings;
        selectedSize = AvaibleSizes.FirstOrDefault(
            s => s.SizeResolution == _projectService.ModelingSettings.WorktopResolution,
            AvaibleSizes[0]
            );
        OnPropertyChanged(nameof(IsExperimentalOptions));
        OnPropertyChanged(nameof(SelectedSize));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _projectService.ProjectLoaded -= _projectService_ProjectLoaded;
        }
        
        base.Dispose(disposing);
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
        
        _projectService.ModelingSettings.IsExperimentalSettings = IsExperimentalOptions;
        _projectService.ModelingSettings.WorktopResolution = SelectedSize.SizeResolution;
        
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

    }
}

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
