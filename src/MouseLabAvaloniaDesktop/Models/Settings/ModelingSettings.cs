using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MouseLabAvaloniaDesktop.Models.Settings
{
    [GenerateCopyFrom]
    public partial class ModelingSettings : ObservableObject
    {
        public const int DefaultWorktopResolution = 8_192;

        //[ObservableProperty]
        //[property: JsonInclude]
        public bool IsExperimentalSettings { get; set => SetProperty(ref field, value); } = false;


        //[ObservableProperty]
        //[property: JsonInclude]
        public int WorktopResolution { get; set => SetProperty(ref field, value); } = DefaultWorktopResolution;

        public void ValidateProperties()
        {
            if (IsExperimentalSettings)
            {

            }
            else
            {
                WorktopResolution = DefaultWorktopResolution;
            }
        }
    }
}
