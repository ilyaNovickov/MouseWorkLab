using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;

namespace MouseLabAvaloniaDesktop.Models.Settings
{
    [GenerateCopyFrom]
    public partial class UiSettings : ObservableObject
    {
        //[ObservableProperty]
        //[property: JsonInclude]
        public Themes Theme { get; set => SetProperty(ref field, value); } = Themes.Default;

        //[ObservableProperty]
        //[property: JsonInclude]
        public CultureInfo Culture { get; set => SetProperty(ref field, value); } = CultureInfo.CurrentCulture;

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
