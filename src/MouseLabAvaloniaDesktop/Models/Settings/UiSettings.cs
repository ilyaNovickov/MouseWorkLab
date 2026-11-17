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

        /// <summary>
        /// Превращает строку из настроек в CultureInfo, подставляя культуру по умолчанию
        /// при любой проблеме. cultureName хранится строкой специально: CultureInfo
        /// не сериализуется в JSON "человеческим" образом.
        /// </summary>
        public static CultureInfo ResolveCulture(string? cultureName)
        {
            if (string.IsNullOrWhiteSpace(cultureName))
                return CultureInfo.CurrentCulture;

            try
            {
                return CultureInfo.GetCultureInfo(cultureName);
            }
            catch (CultureNotFoundException)
            {
                return CultureInfo.CurrentCulture;
            }
        }

    }
}
