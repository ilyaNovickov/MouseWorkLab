using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using MouseLabAvaloniaDesktop.Services.AppSettings;
using ProTranslate;

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

public partial class ModelingSettingsViewModel : TranslatedViewModelBase
{
    private ObservableCollection<WorktopSizeViewModel> avaibleSizes;

    private readonly IApplicationSettingsService _settings;

    public ModelingSettingsViewModel(ITranslationService translation, IApplicationSettingsService settings) : base(translation)
    {
        _settings = settings;

        avaibleSizes = [
            new WorktopSizeViewModel(8192),
            new WorktopSizeViewModel(2 * 8192)
        ];

        //OnPropertyChanged(nameof(AvaibleSizes));
        selectedSize = AvaibleSizes.First();
    }

    public bool IsExperimentalOptions
    {
        get;
        set
        {
            SetProperty(ref field, value);
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
}
