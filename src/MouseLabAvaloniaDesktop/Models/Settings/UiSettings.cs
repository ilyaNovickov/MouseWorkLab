using CommunityToolkit.Mvvm.ComponentModel;
using MouseLabAvaloniaDesktop.Models.DTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;

namespace MouseLabAvaloniaDesktop.Models.Settings
{
    public partial class UiSettings : ObservableObject, IDataTransferObject<UiSettingsDto>
    {
        public Themes Theme { get; set => SetProperty(ref field, value); } = Themes.Default;

        public string Culture { get; set => SetProperty(ref field, value); } = CultureInfo.CurrentCulture.Name;

        public void LoadDto(UiSettingsDto value)
        {
            this.Culture = value.Culture;
            this.Theme = value.Theme;
        }

        public UiSettingsDto ToDto()
        {
            return new UiSettingsDto()
            {
                Theme = this.Theme,
                Culture = this.Culture
            };
        }

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
