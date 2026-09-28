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
| Вывести перевод в XAML | [localization.md](localization.md#три-способа-вывести-перевод) |
| Строка с параметром (`{0}`) | [localization.md](localization.md#строки-с-параметрами) |
| Разобраться, что где лежит | [architecture.md](architecture.md) |
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
    App.axaml.cs                             старт приложения, DI, подключение ProTranslate
    ViewModels/ViewModelBase.cs              базовый класс с доступом к переводам
    ViewModels/MainWindowViewModel.cs        пример: переключатель языка
    Views/MainWindow.axaml                   пример: три способа вывода перевода
    Models/CultureOption.cs                  пункт выпадающего списка языков
    Models/Themes.cs                         тема оформления
    Services/AppSettings/                    чтение и запись настроек
    ViewLocator.cs                           сопоставление ViewModel -> View
    MouseLabAvaloniaApp.csproj               пакеты, каталоги, AOT
docs/                                        эта документация
```

## Три правила, которые нельзя нарушать

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

3. **Каталоги переводов — только в `MouseLabAvaloniaApp`.**
   Код, генерируемый SourceGenerator, попадает в сборку того проекта, который
   его запустил. При переносе моделей представления в `MouseLab.Core` или
   `MouseLab.Services` генератор придётся подключать и там.
   Подробности — в [architecture.md](architecture.md#перенос-моделей-представления-в-другой-проект).

## Текущее состояние

Поддерживаются `en-US` и `ru-RU`. Настройки хранятся в
`%LOCALAPPDATA%\MouseLab\settings.json`. AOT-сборка (`PublishAot=true`)
проходит без предупреждений.
