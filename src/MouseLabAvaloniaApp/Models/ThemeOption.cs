using System;
using System.Collections.Generic;
using System.Text;
using ProTranslate;

namespace MouseLabAvaloniaApp.Models
{
    public class ThemeOption
    {
        public ThemeOption(IObservableLocalizedString name, Themes theme)
        {
            this.DisplayName = name;
            this.Theme = theme;
        }

        //public string DisplayName { get; }
        public IObservableLocalizedString DisplayName { get; }

        public Themes Theme { get; }

        public override string ToString()
        {
            return DisplayName.Value;
        }
    }
}
