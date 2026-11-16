using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvaloniaDesktop.Models.Settings
{
    [GenerateCopyFrom]
    public partial class ModelingSettings : ObservableObject
    {
        private const int defaultWorktopResolution = 8_192;

        [ObservableProperty]
        private bool isExperimentalSettings = false;


        [ObservableProperty]
        private int worktopResolution = defaultWorktopResolution;

        public void ValidateProperties()
        {
            if (IsExperimentalSettings)
            {

            }
            else
            {
                WorktopResolution = defaultWorktopResolution;
            }
        }
    }
}
