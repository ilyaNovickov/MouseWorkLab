using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MouseLabAvaloniaDesktop.Models.Settings;
using MouseLabAvaloniaDesktop.Services.Implementations;
using ProTranslate;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvaloniaDesktop.ViewModels.Worktop
{
    public partial class WorktopViewModel : TranslatedViewModelBase
    {
        private readonly ModelingEnv _modelingEnv;
        private readonly ModelingSettings _modelingSettings;

        public WorktopViewModel(
            ITranslationService translation, 
            WorktopCanvasViewModel worktopCanvasViewModel,
            ModelingEnv modelingEnv,
            ModelingSettings modelingSettings
            ) : base(translation)
        {
            WorktopCanvasViewModel = worktopCanvasViewModel;
            _modelingEnv = modelingEnv;
            _modelingSettings = modelingSettings;

            _modelingSettings.PropertyChanged += _modelingSettings_PropertyChanged;
        }

        private void _modelingSettings_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            IsNeedGeneration = _modelingEnv.Worktop.Width == _modelingEnv.Worktop.Height
                && _modelingEnv.Worktop.Width == _modelingSettings.WorktopResolution;
        }

        public WorktopCanvasViewModel WorktopCanvasViewModel { get; }

        public string? Seed
        {
            get;
            set
            {
                if (SetProperty(ref field, value))
                {
                    OnSeedChanged();
                }

            }
        }

        [ObservableProperty]
        private bool isSeedNumber;

        private void OnSeedChanged()
        {
            IsSeedNumber = Seed switch
            {
                null => true,
                not null => int.TryParse(Seed, out _)
            };
        }

        private static int? GetSeedFromString(string? seed)
        {
            if (seed is null)
                return null;

            if (int.TryParse(seed, out int result))
                return result;
            else
                return null;
        }

        [ObservableProperty]
        private bool isNeedGeneration;

        [RelayCommand]
        private void GenerateWorktop()
        {
            _modelingEnv.GenerateNewWorktop(_modelingSettings.WorktopResolution, GetSeedFromString(Seed));
            IsNeedGeneration = false;
            AvaibleResolution = (int)(_modelingEnv.Worktop.Size.Width * (1.00d + 2d * Marging));
            OnPropertyChanged(nameof(Worktop));
        }

        private const double Marging = 0.10d;

        public int AvaibleResolution
        {
            get;
            set
            {
                if (SetProperty(ref field, value))
                {
                    WorktopCanvasViewModel.SetCanvasDiff(field, _modelingEnv.Worktop.Size.Width);
                }
            }
        }
    }
}
