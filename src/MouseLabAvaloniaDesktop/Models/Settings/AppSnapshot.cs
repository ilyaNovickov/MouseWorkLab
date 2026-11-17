using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MouseLabAvaloniaDesktop.Models.Settings
{
    [GenerateCopyFrom]
    public partial class AppSnapshot : ObservableObject
    {
        //[ObservableProperty]
        //[property: JsonInclude]
        public ModelingSettings ModelingSettings { get; set => SetProperty(ref field, value); } = new();

        //[ObservableProperty]
        //[property: JsonInclude]
        public UiSettings UiSettings { get; set => SetProperty(ref field, value); } = new();

        public int Number { get; set; } = 100;
    }
}
