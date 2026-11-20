using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MouseLabAvaloniaDesktop.Models;
using MouseLabAvaloniaDesktop.Models.Settings;
using MouseLabAvaloniaDesktop.Services.Abstractions;
using MouseLabAvaloniaDesktop.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvaloniaDesktop.ViewModels.Worktop
{
    public partial class WorktopCanvasViewModel : ViewModelBase
    {
        
        private readonly ModelingEnv _modelingEnv;
        private readonly ModelingSettings _modelingSettings;

        public WorktopCanvasViewModel(ModelingEnv modelingEnv, ModelingSettings modelingSettings)
        {
            _modelingEnv = modelingEnv;
            _modelingSettings = modelingSettings;
        }

        public IReadOnlyMatrix Worktop => _modelingEnv.Worktop;


        [ObservableProperty]
        private int x;

        [ObservableProperty]
        private int y;

        private int diifValue = 0;

        public void SetCanvasDiff(int avaibleResolution, int resolution)
        {
            diifValue = (avaibleResolution - resolution) / 2;
            X = diifValue;
            Y = diifValue;
        }
    }
}
