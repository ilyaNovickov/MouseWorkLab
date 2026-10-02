using System;
using System.Collections.Generic;
using System.Text;
using ProTranslate;

namespace MouseLabAvaloniaApp.Models
{
    public class ThemeOption : IDisposable
    {
        public ThemeOption(IObservableLocalizedString name, Themes theme)
        {
            this.DisplayName = name;
            this.Theme = theme;
        }

        //public string DisplayName { get; }
        public IObservableLocalizedString DisplayName { get; }

        public Themes Theme { get; }

        // ВНИМАНИЕ: это единственный способ узнать тему "человеческим языком",
        // но он же - источник бага с замороженными подписями. Avalonia без
        // ItemTemplate рисует пункт ComboBox через ToString(), то есть берёт
        // ОДИН снимок строки на момент создания контейнера пункта, и смена
        // языка его уже не обновляет. Поэтому в AppSettingsView обязателен
        // ItemTemplate с Text="{Binding DisplayName.Value}".
        public override string ToString()
        {
            return DisplayName.Value;
        }

        // ProTranslateStrings.Dispose() отписывает от CultureChanged только сам
        // себя; IObservableLocalizedString, выданные Strings.Observe_*(), он не
        // трогает, а подписываются на переводы сами. Без Dispose они пережили бы
        // и окно, и модель представления. Dispose есть только у конкретного
        // ObservableLocalizedString - в самом интерфейсе его нет, поэтому кастуем.
        public void Dispose()
        {
            (DisplayName as IDisposable)?.Dispose();
        }
    }
}
