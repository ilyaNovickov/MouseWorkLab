using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MouseLabAvalonia.Core;
using MouseLabAvalonia.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MouseLabAvalonia.ViewModels
{
    public partial class ModelingSettingsViewModel : ViewModelBase
    {
        private readonly IModelingSettings modelingSettings;
        public ModelingSettingsViewModel(IModelingSettings modelingSettings)
        {
            this.modelingSettings = modelingSettings;

            this.SurfaceHeight = modelingSettings.SurfaceHeight;
            this.SurfaceWidth = modelingSettings.SurfaceWidth;
        }

        [ObservableProperty]
        private int surfaceWidth;

        [ObservableProperty]
        private int surfaceHeight;

        [RelayCommand]
        private void SaveSettings()
        {
            this.modelingSettings.SurfaceHeight = this.SurfaceHeight;
            this.modelingSettings.SurfaceWidth = this.SurfaceWidth;
        }


#if DEBUG
        public static ModelingSettingsViewModel Instance => new(new ModelingSettingsService());
#endif
    }
}
