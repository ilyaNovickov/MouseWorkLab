# Освобождение ресурсов: зачем нужен `Dispose` и когда его вызывать

Документ объясняет, что именно освобождает `Dispose` в этом приложении, когда он
нужен, а когда является лишним шумом, и какие ошибки с ним уже случались.

## Коротко

Правило одно:

> `Dispose` нужен не «для экономии памяти», а чтобы **разорвать сильную ссылку на
> объект, который живёт дольше нас**.

Подробности ниже, но если читать одну строку: подписка на событие синглтона без
отписки — это баг, даже если потери не видно.

## Что `Dispose` делает в этом приложении

Цепочка вызовов ровно одна:

```
ViewModelBase.Dispose()                          ViewModelBase.cs:53
  └─ Strings.Dispose()                           ViewModelBase.cs:72
       └─ ProTranslateStrings.Dispose()          ProTranslateStrings.g.cs:156
            └─ _translations.CultureChanged -= OnCultureChanged
```

И парная ей подписка в конструкторе того же класса:

```csharp
// obj/gen/ProTranslate.SourceGenerator/.../ProTranslateStrings.g.cs:10-14
public ProTranslateStrings(ITranslationService translations)
{
    _translations = translations ?? throw new ArgumentNullException(nameof(translations));
    _translations.CultureChanged += OnCultureChanged;
}
```

То есть **каждая модель представления подписывается на событие синглтона в
конструкторе и отписывается только в `Dispose`**. Больше `Dispose` в этой цепочке
ничего не делает — ни полей, ни буферов, ни unmanaged-ресурсов он не освобождает.

Чтобы посмотреть сгенерированный код целиком:

```bash
dotnet build src\MouseLabAvaloniaApp\MouseLabAvaloniaApp.csproj -t:Rebuild `
  -p:EmitCompilerGeneratedFiles=true -p:CompilerGeneratedFilesOutputPath=obj\gen
```

## Почему GC здесь не спасает

`ITranslationService` и `ICultureService` регистрируются пакетом через
`TryAddSingleton` — это подтверждается тем, что `ProTranslate.MicrosoftExtensions.dll`
содержит вызов `TryAddSingleton` и не содержит `TryAddScoped`/`TryAddTransient`.
Значит, они живут столько же, сколько контейнер, то есть всю сессию.

Подписка на событие в .NET — обычная сильная ссылка. Если на событии синглтона
подписан объект, синглтон **держит его живым**, и сборщик мусора не сможет его
собрать: объект достижим из корня.

```
контейнер DI
  └── ITranslationService          (синглтон, корень, живёт всю сессию)
        └── CultureChanged ──► ProTranslateStrings
                                     └── _translations   (только обратная ссылка)
```

### Два независимых случая

**Важное наблюдение:** `ProTranslateStrings` не ссылается на модель представления.
Поле `ViewModelBase.Strings` — однонаправленная ссылка. Поэтому отписка от
`CultureChanged` освобождает **только сам `ProTranslateStrings`**, а не модель
представления.

Отсюда два совершенно разных по цене случая:

| | Кто держит | Что удерживается | Цена |
|---|---|---|---|
| **A** | `ITranslationService` | только `ProTranslateStrings` | десятки байт |
| **B** | `ICultureService` | **вся модель представления** с её графом | вот это дорого |

Случай **B** и есть настоящая проблема. Именно он был в
`AppSettingsViewModel`: модель подписывалась на `CultureChanged` синглтона, и
синглтон удерживал её — вместе со всем, на что она ссылается.

## Сколько это стоит

### Память

`ProTranslateStrings` для 14 ключей — это два readonly-поля, один `bool` и
никакого состояния на ключ: каждое свойство просто проксирует вызов в сервис
(`ProTranslateStrings.g.cs:24`). Освобождение такого объекта экономит
десятки байт.

Кэш переводов (`TranslationCacheOptions.MaximumEntries = 2048`,
`App.axaml.cs:81`) живёт в **синглтоне**, а не в модели представления, поэтому
его освобождение в `Dispose` не даёт ничего.

Реальная польза для памяти одна: список подписчиков события в синглтоне
перестаёт расти. Это заметно в длинной сессии, где окно настроек открывают и
закрывают много раз.

### CPU

При каждой смене культуры каждый живой подписчик выполняет `Refresh()`, а тот
поднимает 16 `PropertyChanged` (14 ключей + `Culture` + `UICulture`,
`ProTranslateStrings.g.cs:136-154`).

Для **мёртвого** экземпляра без слушателей это ~16 вызовов делегата — мелочь.

А вот случай **B** стоил заметно дороже: мёртвая `AppSettingsViewModel` делала
`Dispatcher.UIThread.Post(...)` на каждую смену культуры
(`AppSettingsViewModel.cs:96`). Это **поставленная в очередь задача UI-потока на
каждую утечку на каждое переключение языка** — аллокации, нагрузка на
диспетчер, давление на инвалидацию. На порядки дороже, чем 16 вызовов делегата,
которые экономит `Strings.Dispose()`.

### Вывод

Относитесь к `Dispose` как к инструменту **корректности и владения ресурсами**,
а не как к оптимизации. Выигрыш в памяти и CPU реален, но вторичен: сначала
пишите `Dispose` потому, что подписка без отписки — это баг, и только потом
потому, что это ещё и медленнее.

## Правило: когда `Dispose` нужен

> Освобождайте модель представления тогда и только тогда, когда она
> **регистрирует себя где-то, что переживёт её** — событие синглтона,
> статическая коллекция, таймер, подписка, — **либо владеет** объектом,
> реализующим `IDisposable`, который она сама создала.

Применено к моделям представления этого приложения:

| Модель | Время жизни | Причина | Нужен `Dispose`? |
|---|---|---|---|
| `MainWindowViewModel` | синглтон, вся сессия | ничего не подписывает (старый переключатель лежит в блоке `/* */`, `MainWindowViewModel.cs:40-154`) | **нет** |
| `SettingsWindowViewModel` | transient, на окно | **владеет** `AppSettingsViewModel` | **да** — владение |
| `AppSettingsViewModel` | transient, на окно | **подписывается** на `ICultureService` (`:52`) и владеет тремя `IObservableLocalizedString` | **да** — оба случая |

Две модели из трёх, и по разным причинам.

## Когда `Dispose` НЕ нужен

* **Ничего не подписано и ничего не владеешь.** `MainWindowViewModel` — пример:
  он живёт всю сессию, ничего не подписывает, поэтому отдельный `Dispose` ему не
  нужен. Контейнер освободит `Strings` сам при выходе.
* **`using` на модели представления — запрещён.** Никогда. Модель, которая
  переживает окно, нельзя освобождать в конструкторе владельца или в
  операторе `using`. Ровно этот класс ошибок уже случался с
  `ServiceProvider` в `App.axaml.cs` — см. [architecture.md](architecture.md#почему-контейнер-в-поле).
* **Не добавляйте `IDisposable` «на всякий случай».** Контейнер тогда начнёт
  вызывать `Dispose` при выходе — это безвредно, но провоцирует кого-то позже
  дописать `using`, и вот это уже нет.
* **Пустой `Dispose(bool disposing) { base.Dispose(disposing); }`** — тоже
  лишний. Не переопределяйте шаблон, если освобождать нечего.

## Кто вызывает `Dispose` в этом приложении

Цепочка освобождения транзиентных моделей настроек:

```
пользователь закрывает окно настроек
        │
        ▼
window.Closed  (WindowsManagerService.cs:61)
        ├─ openedWindows.Remove(window)
        └─ viewModel.Dispose()               (WindowsManagerService.cs:68)
                 │
                 ▼
SettingsWindowViewModel.Dispose(bool)        (SettingsWindowViewModel.cs:28)
        ├─ AppSettings.Dispose()
        │        │
        │        ▼
        │   AppSettingsViewModel.Dispose(bool) (AppSettingsViewModel.cs:154)
        │        ├─ _cultures.CultureChanged -= OnCultureChanged   (:158)
        │        ├─ foreach (ThemeOption) option.Dispose()         (:165)
        │        │        └─ (DisplayName as IDisposable)?.Dispose()  ThemeOption.cs:39
        │        └─ base.Dispose(disposing) → Strings.Dispose()
        │
        └─ base.Dispose(disposing) → Strings.Dispose()
```

При выходе из приложения:

```
desktop.Exit → Shutdown()                    (App.axaml.cs:130, :145)
        ├─ settings.ThemeChanged -= _themeChangedHandler   (:155)
        └─ _serviceProvider.Dispose()                      (:159)
```

### Двойное освобождение безопасно

Одну и ту же transient-модель освобождают дважды: обработчик `window.Closed` при
закрытии окна и контейнер при выходе из приложения. Это не ошибка, потому что
`ViewModelBase.Dispose` защищён флагом:

```csharp
// ViewModelBase.cs:63-76
protected virtual void Dispose(bool disposing)
{
    if (_disposed)
        return;
    ...
    _disposed = true;
}
```

Поэтому в `Dispose(bool)` производных классов **не нужно** проверять `_disposed`
вручную и **не нужно** рассчитывать на единственный вызов.

## Отдельный случай: `IObservableLocalizedString`

Его не покрывает `Strings.Dispose()`. Генерированный метод `Observe_*()` просто
передаёт вызов сервису:

```csharp
// ProTranslateStrings.g.cs:78
public IObservableLocalizedString Observe_SettingsDarkTheme(params object?[] arguments)
    => _translations.Observe(ProTranslateKeys.SettingsDarkTheme, arguments);
```

Каждый такой объект подписывается на `CultureChanged` сам и умеет
`IDisposable`. Но:

* `ProTranslateStrings.Dispose()` их **не трогает** — он отписывает только себя;
* в интерфейсе `IObservableLocalizedString` метода `Dispose` **нет** — он есть
  только у конкретного класса `ProTranslate.ObservableLocalizedString`, поэтому
  освобождать нужно через `as IDisposable` (см. `ThemeOption.cs:39`).

Вывод: **кто создал наблюдаемую строку, тот и обязан её освободить.** В этом
приложении это `AppSettingsViewModel`, а хранит строку `ThemeOption`, который
поэтому и `IDisposable`.

## Ошибки, которые уже были

### 1. Повторное открытие окна создавало ненужные модели

`WindowsManagerService.ShowSettingsAsync` проверял список открытых окон
**после** создания VM и окна. Второй клик по меню создавал ещё одну
`SettingsWindowViewModel` с подпиской на `CultureChanged`, которую никто не
отписывал, и выбрасывал её.

Проверка перенесена в начало метода (`WindowsManagerService.cs:31-37`).

### 2. `AppSettingsViewModel` не освобождался при закрытии окна

`SettingsWindowViewModel` не переопределял `Dispose`, поэтому освобождался только
его собственный `Strings`, а строка `_cultures.CultureChanged -= OnCultureChanged`
не выполнялась. Синглтон `ICultureService` удерживал модель до конца сессии.

Добавлен `Dispose(bool)` в `SettingsWindowViewModel.cs:28`.

### 3. Наблюдаемые строки тем не освобождались

См. предыдущий раздел. `ThemeOption` стал `IDisposable`, список тем освобождается
в `AppSettingsViewModel.cs:165`.

### 4. Ловушка в `MainWindowViewModel`

Старый переключатель языка лежит в блочном комментарии
(`MainWindowViewModel.cs:40-154`). Внутри него есть строка `_cultures.CultureChanged +=
OnCultureChanged;` (`:66`) — **и нет соответствующего `Dispose`**.

Сейчас это безвредно, потому что код не компилируется. Но если кто-то снимет
комментарий, модель (а она синглтон) останется подписанной на всё время работы
программы. Если разкомментируете — добавляйте `Dispose(bool)` с отпиской.

## Чек-лист для ревью

* [ ] Модель подписывается на событие? Есть ли `Dispose` с отпиской?
* [ ] Модель создаёт `IObservableLocalizedString`? Кто их освобождает?
* [ ] Модель владеет другой `IDisposable` моделью? Освобождает ли её?
* [ ] Нигде ли не стоит `using` на модели представления?
* [ ] Модель transient, но переживает окно? Кто её освобождает при закрытии?

## Связанные документы

* [architecture.md](architecture.md) — поток смены языка и темы, цепочка
  освобождения, порядок запуска.
* [localization.md](localization.md) — `IObservableLocalizedString`,
  `ItemTemplate` в `ComboBox`, почему `nameof(X.Y)` не переподнимает свойство.
* [troubleshooting.md](troubleshooting.md) — симптомы и разборы известных багов.