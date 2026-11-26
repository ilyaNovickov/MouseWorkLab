using CommunityToolkit.Mvvm.ComponentModel;
using MouseLabAvaloniaDesktop.Models.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MouseLabAvaloniaDesktop.Models.Settings
{
    public partial class ModelingSettings : ObservableObject, IDataTransferObject<ModelingSettingsDto>
    {
        public const int DefaultWorktopResolution = 64;

        public bool IsExperimentalSettings
        {
            get;
            set
            {
                if (SetProperty(ref field, value))
                {
                    if (field)
                        OnPropertyChanged(nameof(WorktopResolution));
                }
            }
        } = false;


        public int WorktopResolution
        {
            get => IsExperimentalSettings ? field : DefaultWorktopResolution;
            set => SetProperty(ref field, value);
        } = DefaultWorktopResolution;

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

        public void LoadDto(ModelingSettingsDto value)
        {
            this.IsExperimentalSettings = value.IsExperimentalSettings;
            this.WorktopResolution = value.WorktopResolution;
            this.ValidateProperties();
        }

        public ModelingSettingsDto ToDto()
        {
            return new ModelingSettingsDto()
            {
                IsExperimentalSettings = this.IsExperimentalSettings,
                WorktopResolution = this.WorktopResolution
            };
        }
    }
}
