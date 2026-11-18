using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MouseLabAvaloniaDesktop.Models.DTO
{
    public class UiSettingsDto
    {
        public Themes Theme { get; set; }

        public string Culture { get; set; } = CultureInfo.CurrentCulture.Name;
    }
}
