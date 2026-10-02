# Архитектура локализации

Документ описывает, как локализация устроена внутри `MouseLabAvaloniaApp`,
чтобы при изменении связей не потерять работоспособность.

## Файлы и их роль

### Каталоги

| Файл | Роль |
|---|---|
| `Assets/Translations/Strings.en-US.json` | каталог переводов, английский (США) |
| `Assets/Translations/Strings.ru-RU.json` | каталог переводов, русский (Россия) |
| `Assets/Translations/README.md` | соглашения по именованию ключей |

Каталоги **исключены** из `AvaloniaResource` и подключены как `AdditionalFiles`:
их читает только генератор на этапе сборки. В рантайме файлов рядом с exe нет.

### Настройки

| Файл | Роль |
|---|---|
| `Services/AppSettings/IApplicationSettingsService.cs` | контракт: тема, культура, два события |
| `Services/AppSettings/ApplicationSettingsService.cs` | состояние в памяти + автосохранение |
| `Services/AppSettings/AppSettingsStore.cs` | чтение/запись `%LOCALAPPDATA%\MouseLab\settings.json` |
| `Services/AppSettings/ThemeChangedEventArgs.cs` | аргументы события темы |
| `Services/AppSettings/CultureChangedEventArgs.cs` | аргументы события языка |
| `AppServices.cs` | точка сборки графа объектов, чтение настроек, контейнер |

### Модели и представления

| Файл | Роль |
|---|---|
| `ViewModels/ViewModelBase.cs` | даёт каждой модели `Strings` и `Translations` |
| `ViewModels/MainWindowViewModel.cs` | открывает окно настроек |
| `ViewModels/Settings/SettingsWindowViewModel.cs` | модель окна настроек, владеет `AppSettings` |
| `ViewModels/Settings/AppSettingsViewModel.cs` | выбор языка и темы, `SaveSettingsCommand` |
| `Models/CultureOption.cs` | пункт выпадающего списка языков |
| `Models/ThemeOption.cs` | пункт выпадающего списка тем, `IDisposable` |
| `Models/Themes.cs` | тема оформления |
| `Views/MainWindow.axaml` | кнопка открытия настроек |
| `Views/Settings/SettingsWindow.axaml` | окно настроек, кнопка Save (`f`) |
| `Views/Settings/AppSettingsView.axaml` | выпадающие списки языка и темы |
| `Services/WindowsManager/WindowsManagerService.cs` | создаёт и закрывает окна |
| `ViewLocator.cs` | сопоставление `ViewModel` → `View` |

## Порядок запуска

Регистрация сервисов вынесена в `AppServices`; `App` занимается только
оркестрацией. Порядок важен:

```
App.OnFrameworkInitializationCompleted
│
├─ 1. AppServices.CreateDefault()               // AppServices.cs:130
│        ├─ AppSettingsStore.Load()             // прочитать settings.json
│        ├─ ResolveCulture(...)                 // культура из настроек или en-US
│        ├─ AddSingleton<IApplicationSettingsService>(new ...(snapshot))
│        ├─ AddProTranslate(culture: initialCulture)
│        ├─ AddProTranslateAvalonia()
│        ├─ AddTransient/AddSingleton для моделей и представлений
│        ├─ BuildServiceProvider()               // AppServices.cs:42
│        └─ provider.UseProTranslateAvalonia()  // AppServices.cs:46 — сразу после сборки
│
├─ 2. settings.ThemeChanged += ApplyTheme       // App.axaml.cs:37
├─ 3. ApplyTheme(settings.CurrentAppTheme)      // применить тему из настроек
├─ 4. new MainWindow { DataContext = ... }
└─ 5. desktop.Exit += Shutdown                  // освободить контейнер при выходе
```

Шаг `UseProTranslateAvalonia` обязателен и часто забывается. Он передаёт
адаптеру те же `ITranslationService` и `ICultureService`, что использует
приложение. Без него статический binding source адаптера остался бы
настроенным на пустой провайдер по умолчанию. Он находится **в том же методе**,
что и `BuildServiceProvider`, чтобы порядок «сначала собрать, потом подключить
адаптер» нельзя было нарушить извне.

### Почему контейнер в поле

```csharp
// AppServices.cs:28
private readonly ServiceProvider _serviceProvider;
```

Если написать `using ServiceProvider sp = ...`, контейнер будет уничтожен
в момент выхода из метода, и все синглтоны (включая `MainWindowViewModel`,
который живёт дольше) окажутся освобождёнными. Контейнер живёт до `desktop.Exit`,
где вызывается `App.Shutdown()`: сначала отписка от события темы, потом
`AppServices.Dispose()`.

У `AppServices` **нет финализатора** — см. [dispose.md](dispose.md#у-appservices-нет-финализатора--и-это-не-упущение).

### Почему культура известна до сборки контейнера

`AddProTranslate` принимает культуру аргументом, а читать настройки нужно
раньше, чем появятся сервисы. Поэтому `AppSettingsStore.Load()` и
`ResolveCulture` выполняются в `AppServices.CreateDefault()` — **до** сборки
контейнера — и готовый снимок передаётся в регистрацию:

```csharp
services.AddSingleton<IApplicationSettingsService>(new ApplicationSettingsService(snapshot));
```

### Контейнер дизайнера

`AppServices.Instance` (только `#if DEBUG`) — отдельный контейнер, кэшируемый в
статическом поле. Нужен вьюхам, чтобы превью в IDE могло создать модель
представления с внедрёнными зависимостями:

```csharp
#if DEBUG
        if (Design.IsDesignMode)
            Design.SetDataContext(this, AppServices.Instance.Provider.GetRequiredService<MainWindowViewModel>());
#endif
```

Три правила, из-за которых это работает:

* **`Design.SetDataContext` — единственный способ.** Не ставьте модель
  представления в `<Design.DataContext>` внутри XAML: такая модель требует
  параметров конструктора и не создаётся. В XAML-варианте превью падало бы, а
  код-behind с `SetDataContext` при этом молча игнорировался бы.
* **`Instance` обязан кэшироваться.** Без кэша каждое обращение создавало бы
  новый контейнер, перечитывало `settings.json` и переустанавливало статический
  binding source ProTranslate.
* **Контейнер дизайнера не должен быть рабочим.** У него собственный
  `IApplicationSettingsService`, на который `App` не подписан, поэтому смена темы
  в превью не доедет до `App.ApplyTheme`. Общий контейнер означал бы, что
  превью влияет на живое приложение.

Освобождать контейнер дизайнера не нужно — он живёт только в процессе превью.

## Поток смены языка и темы

Переключение **происходит по кнопке Save**, а не сразу при выборе пункта.
`SelectedCulture` и `SelectedTheme` только запоминают выбор; применяет его
`SaveSettingsCommand`.

```
пользователь выбирает язык/тему в ComboBox
        │
        ▼
AppSettingsViewModel.SelectedCulture / SelectedTheme   (двусторонние привязки)
        │   только SetProperty, ничего не применяют
        ▼
пользователь нажимает Save (кнопка "f" в SettingsWindow)
        │
        ▼
AppSettingsViewModel.SaveSettingsCommand
        │
        ├─► SyncSelectedCultureToSettings()
        │        ├─► IApplicationSettingsService.CurrentCultureName = "ru-RU"
        │        │        └─► CultureChanged (приложения) ─► AppSettingsStore.Save()
        │        │                                          settings.json перезаписан
        │        │
        │        └─► ICultureService.SetCulture("ru-RU")
        │                 └─► CultureChanged (ProTranslate)
        │                         ├─► ProTranslateStrings.Refresh()
        │                         │        └─► PropertyChanged по КАЖДОМУ ключу
        │                         │            └─► {Binding Strings.Ключ} обновляется
        │                         │                автоматически
        │                         ├─► ObservableLocalizedString.Value
        │                         │        └─► подписи пунктов ComboBox
        │                         │            (нужен ItemTemplate, см. localization.md)
        │                         └─► AppSettingsViewModel.OnCultureChanged
        │                                  └─► SyncSelectedCulture()
        │
        └─► SyncSelectedThemeToSettings()
                 └─► IApplicationSettingsService.CurrentAppTheme = Themes.Dark
                          └─► ThemeChanged ─► App.ApplyTheme()
                                               └─► Application.RequestedThemeVariant
```

**Ключевой момент:** `ProTranslateStrings` сам поднимает `PropertyChanged`
для каждого сгенерированного свойства. Поэтому `{Binding Strings.ЛюбойКлюч}`
обновляется без единой строки кода. А вот свойства, вычисленные в
`ViewModel`, — обычные CLR-свойства, их переподнимать нужно руками.
Забыть это — самая частая ошибка после переноса проекта на ProTranslate.

Отдельно: `nameof(SelectedTheme.DisplayName)` **не** переподнимает свойство
вложенного объекта. `nameof` отбрасывает квалификатор и даёт просто
`"DisplayName"`, а событие уходит в `AppSettingsViewModel`, где свойства
с таким именем нет. Чтобы обновить `ThemeOption`, нужна привязка к
`IObservableLocalizedString` — то есть `ItemTemplate`. Подробности в
[localization.md](localization.md#переводимые-пункты-в-combobox).

Защита от рекурсии: `SetCulture` внутри себя сравнивает культуру и, если
она не изменилась, событие не поднимает. Дополнительно `SyncSelectedCulture`
сравнивает объекты по ссылке, чтобы не переустанавливать `SelectedCulture`
из обработчика события.

### Кто и когда освобождается

`AppSettingsViewModel` подписан на `CultureChanged` синглтона `ICultureService`,
поэтому событие держит его живым. Освобождается он в три шага, и каждый шаг
обязателен:

1. `WindowsManagerService` проверяет список открытых окон **до** создания
   VM и окна — иначе повторный клик по меню создал бы вторую
   `SettingsWindowViewModel` с подпиской, которую никто не отпишет.
2. В обработчике `window.Closed` вызывается `viewModel.Dispose()`.
   Само `ViewModelBase.Dispose` освобождает только `Strings`, поэтому
   `SettingsWindowViewModel` дополнительно переопределяет `Dispose(bool)` и
   освобождает `AppSettings`.
3. `AppSettingsViewModel` в своём `Dispose(bool)` отписывается от `CultureChanged`
   **и** освобождает `ThemeOption`. Последнее нужно потому, что
   `ProTranslateStrings.Dispose()` отписывает только сам себя, а
   `IObservableLocalizedString`, выданные `Strings.Observe_*()`, не трогает вовсе.

Повторное освобождение безопасно: `ViewModelBase.Dispose` выставляет `_disposed`,
поэтому контейнер DI в `Shutdown()` может освободить те же транзиентные
объекты ещё раз.

Общее правило «когда модели представления нужен `Dispose`, а когда нет» и разбор
всех ошибок, которые с этим случались, — в
[dispose.md](dispose.md).

## Хранение настроек

Файл: `%LOCALAPPDATA%\MouseLab\settings.json`
(`~/.local/share/MouseLab/settings.json` на Linux).

```json
{
  "culture": "ru-RU",
  "theme": "Light"
}
```

Особенности реализации, о которых надо знать:

* **Сериализация через `AppSettingsJsonContext`.** Обычные
  `JsonSerializer.Serialize/Deserialize` помечены `IL2026`/`IL3050` — они
  опираются на рефлексию, и в AOT-сборке настройки молча перестали бы
  работать. `AppSettingsJsonContext` — это контекст
  `System.Text.Json source generation`, сгенерированный компилятором.
* **`Themes` помечен `[JsonConverter(typeof(JsonStringEnumConverter<Themes>))]`.**
  Именно обобщённая версия: обычная `JsonStringEnumConverter` требует
  кодогенерации в рантайме и даёт `IL3050`.
* **Ошибки чтения и записи проглатываются.** Битый JSON или
  `settings.json` в read-only не должны ронять приложение. Побочный эффект:
  реальная ошибка формата может остаться незамеченной — при разборе
  проблем с настройками временно замените `catch` на вывод в лог.
* **Настройки записываются целиком** на любое изменение темы или языка.
  Переключение происходит по кнопке Save в окне настроек: выбор только
  запоминается в `AppSettingsViewModel`, а применяется и сохраняется в
  `SaveSettingsCommand`. Тема доходит до интерфейса через
  `IApplicationSettingsService.ThemeChanged` → `App.ApplyTheme()` →
  `Application.RequestedThemeVariant`.

## Требования AOT

Приложение публикуется с `PublishAot=true` и `<AotAssemblies>True</AotAssemblies>`.
Что из этого следует:

| Требование | Почему |
|---|---|
| `<AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>` | рефлексивные привязки вырезаются триммером |
| Только `{Binding Strings.Ключ}`, без `{Translate ...}` | привязка по индексатору — рефлексия |
| Никаких `Type.GetType` / `Activator.CreateInstance` по вычисленному имени | имя вычисляется в рантайме, анализатор не может его разрешить |
| `AppSettingsJsonContext` | см. выше |
| `JsonStringEnumConverter<Themes>` | обобщённая версия без кодогенерации |

`ViewLocator` изначально был написан по шаблону Avalonia с
`Type.GetType(...)` и `Activator.CreateInstance(...)`. Он заменён на обычный
switch. Если будете добавлять вторичные представления (например,
`SettingsView` для `SettingsViewModel`), добавляйте их **в `ViewLocator`
явным случаем**, а не рассчитывайте на поиск по имени.

Проверка AOT-сборки:

```bash
dotnet publish src/MouseLabAvaloniaApp/MouseLabAvaloniaApp.csproj `
  -c Release -r win-x64 -p:PublishAot=true
```

Ожидаемый результат — ноль предупреждений `IL2026` / `IL3050` и один
`MouseLabAvaloniaApp.exe` рядом с нативными библиотеками Skia/HarfBuzz/ANGLE.

## Перенос моделей представления в другой проект

Сейчас все модели представления живут в `MouseLabAvaloniaApp`, и это важно:
`ProTranslate.SourceGenerator` генерирует код **в сборку того проекта,
который его подключил**. `ProTranslateStrings` существует только в
`MouseLabAvaloniaApp.dll`.

Если `ViewModelBase` переедет в `MouseLab.Core` или `MouseLab.Services`,
нужно:

1. Подключить в этом проекте пакеты:
   ```xml
   <PackageReference Include="ProTranslate.Core" Version="0.1.0" />
   <PackageReference Include="ProTranslate.SourceGenerator" Version="0.1.0">
     <OutputItemType>Analyzer</OutputItemType>
     <ReferenceOutputAssembly>false</ReferenceOutputAssembly>
   </PackageReference>
   <PackageReference Include="ProTranslate.Analyzers" Version="0.1.0">
     <PrivateAssets>all</PrivateAssets>
     <OutputItemType>Analyzer</OutputItemType>
     <ReferenceOutputAssembly>false</ReferenceOutputAssembly>
   </PackageReference>
   ```
2. Подключить те же файлы каталогов, иначе проект получит пустой набор ключей:
   ```xml
   <AdditionalFiles Include="..\MouseLabAvaloniaApp\Assets\Translations\Strings.*.json" />
   ```
3. **Не подключать** `ProTranslate.Avalonia` в непредстатренном проекте.
   Адаптеры должны жить только в UI-проекте, иначе появится циклическая
   зависимость между `ProTranslate.Core` и `ProTranslate.Avalonia`.

Альтернатива без переноса: вынести каталоги в отдельную папку на уровне
решения и дать каждому проекту свою `AdditionalFiles`-ссылку.

## Зависимости

```
ProTranslate.MicrosoftExtensions
        │
        ├──► ProTranslate.Core ──► ProTranslate.Abstractions
        │
ProTranslate.Avalonia ──► ProTranslate.Core
        │
        ├──► ProTranslate.SourceGenerator (анализатор, генерирует код)
        └──► ProTranslate.Analyzers      (анализатор, проверки)
```

Адаптер зависит от ядра, но не наоборот. Поэтому ядро можно использовать
в проектах без UI (тесты, фоновые задачи), а `ProTranslate.Avalonia` —
только в приложении.
