using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MouseLabAvaloniaDesktop.Models.Settings
{
    [GenerateCopyFrom]
    public partial class UiSettings : ObservableObject
    {
        [ObservableProperty]
        private Themes theme = Themes.Default;

        [ObservableProperty]
        private CultureInfo culture = CultureInfo.CurrentCulture;

    }
}
