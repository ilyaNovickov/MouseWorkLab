# Документация MouseLab

Документация по локализации приложения на базе библиотеки
[ProTranslate](https://github.com/wieslawsoltes/ProTranslate) (версия `0.1.0`).

Цель документации — чтобы любой следующий разработчик (в том числе вы через полгода)
мог безопасно добавлять строки, языки и экраны, не ломая переключение языка
и AOT-сборку.

## С чего начать

| Задача | Куда смотреть |
|---|---|
| Добавить новую строку перевода | [localization.md](localization.md#как-добавить-новую-строку) |
| Добавить новый язык | [localization.md](localization.md#как-добавить-новый-язык) |
| Вывести перевод в XAML | [localization.md](localization.md#четыре-способа-вывести-перевод) |
| Переводимый пункт списка | [localization.md](localization.md#переводимые-пункты-в-combobox) |
| Строка с параметром (`{0}`) | [localization.md](localization.md#строки-с-параметрами) |
| Разобраться, что где лежит | [architecture.md](architecture.md) |
| Когда нужен `Dispose`, а когда нет | [dispose.md](dispose.md) |
| Что-то не переводится / ошибки сборки | [troubleshooting.md](troubleshooting.md) |

## Устройство решения

```
src/
  MouseLabSolution.slnx                      решение (.slnx)
  MouseLab.Core/                             предметная область, без зависимостей от UI
  MouseLab.Services/                         сервисы предметной области
  MouseLabAvaloniaApp/                       Avalonia-приложение
    Assets/Translations/
      Strings.en-US.json                     каталог переводов (английский, США)
      Strings.ru-RU.json                     каталог переводов (русский, Россия)
      README.md                              соглашения по каталогам
    App.axaml.cs                             оркестрация запуска: тема, главное окно, выход
    AppServices.cs                           точка сборки графа объектов (DI) + контейнер дизайнера
    ViewModels/ViewModelBase.cs              базовый класс с доступом к переводам
    ViewModels/MainWindowViewModel.cs        открывает окно настроек
    ViewModels/Settings/                     модели окна настроек: язык, тема, SaveSettingsCommand
    Views/MainWindow.axaml                   главное окно
    Views/Settings/SettingsWindow.axaml      окно настроек, кнопка Save
    Views/Settings/AppSettingsView.axaml     выпадающие списки языка и темы
    Models/CultureOption.cs                  пункт выпадающего списка языков
    Models/ThemeOption.cs                    пункт выпадающего списка тем
    Models/Themes.cs                         тема оформления
    Services/AppSettings/                    чтение и запись настроек
    Services/WindowsManager/                 создание и закрытие окон
    ViewLocator.cs                           сопоставление ViewModel -> View
    MouseLabAvaloniaApp.csproj               пакеты, каталоги, AOT
docs/                                        эта документация
```

## Шесть правил, которые нельзя нарушать

Это самая частая причина «оно работало и сломалось».

1. **Не использовать `{Translate ...}` и `{Format ...}` в XAML.**
   Эти разметочные расширения не обновляются при смене языка и несовместимы с
   NativeAOT. Подробности и доказательства — в
   [troubleshooting.md](troubleshooting.md#известный-баг-перевод-замораживается-при-смене-языка).
   Вместо них используйте `{Binding Strings.Ключ}`.

2. **Свойства, которые считаются в `ViewModel`, переподнимать вручную.**
   `Strings.*` обновляются сами. А вот `GreetingText => Translations.Format(...)` —
   обычное CLR-свойство, и без `OnPropertyChanged(nameof(GreetingText))`
   в обработчике смены культуры оно навсегда останется на стартовом языке.

3. **Переводимые пункты `ComboBox` обязаны иметь `ItemTemplate`.**
   Без него Avalonia рисует пункт через `ToString()`, то есть берёт снимок
   строки; смена языка такой текст уже не обновляет. Подробности — в
   [localization.md](localization.md#переводимые-пункты-в-combobox).

4. **Каталоги переводов — только в `MouseLabAvaloniaApp`.**
   Код, генерируемый SourceGenerator, попадает в сборку того проекта, который
   его запустил. При переносе моделей представления в `MouseLab.Core` или
   `MouseLab.Services` генератор придётся подключать и там.
   Подробности — в [architecture.md](architecture.md#перенос-моделей-представления-в-другой-проект).

5. **Подписка на событие синглтона требует `Dispose`.**
   `ICultureService` живёт всю сессию, поэтому модель, подписавшаяся на его
   `CultureChanged`, без отписки остаётся живой до конца программы. Освобождать
   нужно не только `Strings`, но и всё, что модель сама создала.
   Подробности — в [dispose.md](dispose.md).

6. **Превью в дизайнере — только через `Design.SetDataContext`.**
   Модель представления требует зависимостей в конструкторе, поэтому в XAML
   `<Design.DataContext><vm:SomeViewModel/></Design.DataContext>` не создастся, а
   код-behind с `SetDataContext` при этом молча проиграет. Контейнер берётся из
   `AppServices.Instance` (`#if DEBUG`), он кэшируется и намеренно не является
   рабочим.
   Подробности — в [architecture.md](architecture.md#контейнер-дизайнера).

## Текущее состояние

Поддерживаются `en-US` и `ru-RU`. Настройки хранятся в
`%LOCALAPPDATA%\MouseLab\settings.json`. AOT-сборка (`PublishAot=true`)
проходит без предупреждений.

Язык и тема переключаются **по кнопке Save** в окне настроек: выбор только
запоминается, применение и сохранение делает `SaveSettingsCommand`.
