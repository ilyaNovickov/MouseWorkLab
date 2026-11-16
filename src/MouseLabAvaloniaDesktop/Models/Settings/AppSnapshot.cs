using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvaloniaDesktop.Models.Settings
{
    [GenerateCopyFrom]
    public partial class AppSnapshot : ObservableObject
    {
        [ObservableProperty]
        private ModelingSettings modelingSettings = new();

        [ObservableProperty]
        private UiSettings uiSettings = new();
    }
}
