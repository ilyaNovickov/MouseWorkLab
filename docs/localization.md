# Локализация: как пользоваться ProTranslate в этом приложении

## Что такое ProTranslate за одну минуту

Библиотека состоит из двух независимых частей.

**Ядро (`ProTranslate.Core`)** не знает про UI и отвечает на вопросы:

- *Какая сейчас культура?* → `ICultureService`
- *Что означает ключ `App.Greeting` в текущей культуре?* → `ITranslationService`
- *А если ключа нет?* → fallback по родительским культурам и по культуре по
  умолчанию; если и так нет, возвращается сам ключ (без исключения)
- *Как подставить `{0}`?* → `ITranslationService.Format`
- *Какое направление текста у этой культуры?* → `TextFlowDirection`

**Адаптеры** (`ProTranslate.Avalonia`) подключают ядро к Avalonia: разметочные
расширения, прикреплённые свойства и обновление привязок.

В этом приложении используется путь **«сгенерированный провайдер»**: значения
переводов компилируются в сборку на этапе сборки проекта, без чтения файлов
в рантайме.

## Какие пакеты подключены и зачем

Все в `src/MouseLabAvaloniaApp/MouseLabAvaloniaApp.csproj`.

| Пакет | Роль | Подключать как |
|---|---|---|
| `ProTranslate.Avalonia` | адаптер Avalonia, `UseProTranslateAvalonia()` | обычная ссылка |
| `ProTranslate.MicrosoftExtensions` | регистрация сервисов в DI | обычная ссылка |
| `ProTranslate.SourceGenerator` | генерация ключей, строк и провайдера | **анализатор** |
| `ProTranslate.Analyzers` | проверки ключей при сборке | **анализатор** |

Два последних подключаются так:

```xml
<PackageReference Include="ProTranslate.SourceGenerator" Version="0.1.0">
  <OutputItemType>Analyzer</OutputItemType>
  <ReferenceOutputAssembly>false</ReferenceOutputAssembly>
</PackageReference>
```

Это не обычные библиотеки: они не дают типов, а запускаются при компиляции
и анализируют проект. Поэтому у них нет `ReferenceOutputAssembly`.

## Как устроены каталоги

Файлы `src/MouseLabAvaloniaApp/Assets/Translations/Strings.<код-культуры>.json`.
Культура берётся **из имени файла**: `Strings.ru-RU.json` → `ru-RU`.

```json
{
  "App": {
    "Title": "MouseLab",
    "Greeting": "Welcome to MouseLab!"
  }
}
```

Объект `"App"` с полем `"Greeting"` даёт ключ `App.Greeting`.

В `csproj` файлы объявлены как `AdditionalFiles`, а **исключены** из
`AvaloniaResource` — в рантайме они не читаются:

```xml
<AvaloniaResource Include="Assets\**" Exclude="Assets\Translations\**" />
<AdditionalFiles Include="Assets\Translations\Strings.*.json" />
```

### Что генерируется

| Тип | Что даёт |
|---|---|
| `ProTranslateKeys.AppGreeting` | константа ключа, `const string` |
| `ProTranslateStrings.AppGreeting` | типизированное свойство с готовым переводом |
| `ProTranslateStrings.Format_AppGreeting(args)` | форматирование (для ключей с `{0}`) |
| `ProTranslateGeneratedTranslationProvider` | провайдер со скомпилированными значениями |
| `ProTranslateProviderManifest` | список ключей, культур и файлов-источников |

Сгенерированный код лежит в `obj/` и в репозиторий не попадает. Посмотреть его
можно так:

```bash
dotnet build src/MouseLabAvaloniaApp/MouseLabAvaloniaApp.csproj -t:Rebuild `
  -p:EmitCompilerGeneratedFiles=true -p:CompilerGeneratedFilesOutputPath=obj\gen
```

## Четыре способа вывести перевод

### 1. Свойство `Strings` — основной способ

```xml
<TextBlock Text="{Binding Strings.AppGreeting}" />
```

* обновляется само при смене культуры;
* компилируемая привязка, работает с AOT;
* ключ проверяется на этапе сборки.

### 2. Строка с параметром — в `ViewModel`

Если текст собирается из перевода и данных предметной области, `ProTranslateStrings`
не поможет: там только готовые ключи. Считаем в модели представления:

```csharp
public class MainWindowViewModel : ViewModelBase
{
    private string _userName = "Alex";

    public string GreetingText =>
        Translations.Format(ProTranslateKeys.AppGreetingWithName, _userName);
}
```

`Translations` — это `ITranslationService`, доступный из `ViewModelBase`.

> **Важно:** такое свойство — обычное CLR-свойство. Переподнимать его нужно вручную:
>
> ```csharp
> private void OnCultureChanged(object? sender, ProTranslate.CultureChangedEventArgs e) =>
>     Dispatcher.UIThread.Post(() =>
>     {
>         SyncSelectedCulture();
>         OnPropertyChanged(nameof(GreetingText));
>         OnPropertyChanged(nameof(CurrentCultureText));
>     });
> ```

### 3. Прикрепленное свойство `Translation.Key` — если привязка не подходит

```xml
<TextBlock Translation.Key="Settings.LanguageDescription" />
```

Работает, потому что адаптер напрямую присваивает `TextBlock.Text`, минуя
систему привязок. Недостаток: ключ не проверяется компилятором, опечатку
поймает только `PTA001` по строковому литералу (а он строковые литералы
не анализирует — так что фактически никак). Использовать точечно.

### 4. Переводимые пункты в `ComboBox`

Пункт списка — это объект, а не строка. Чтобы подпись пункта переводилась
живым языком, нужен `ItemTemplate`:

```xml
<ComboBox ItemsSource="{Binding AvailableThemes}"
          SelectedItem="{Binding SelectedTheme}">
    <ComboBox.ItemTemplate>
        <DataTemplate DataType="models:ThemeOption">
            <TextBlock Text="{Binding DisplayName.Value}" />
        </DataTemplate>
    </ComboBox.ItemTemplate>
</ComboBox>
```

`DisplayName` — это `IObservableLocalizedString` из
`Strings.Observe_SettingsDarkTheme()`. Он сам подписан на смену культуры и
поднимает `PropertyChanged`, поэтому привязка к `DisplayName.Value`
перевычисляется сама.

**Без `ItemTemplate` подписи замерзают.** Avalonia рисует пункт через
`ContentPresenter`, а тот для обычного объекта вызывает `ToString()` и
подставляет результат в `TextBlock.Text`. Это снимок строки на момент
создания контейнера пункта: смена языка его уже не обновляет, и текст
меняется только при пересоздании окна. Именно так выглядел баг
«подписи тем не переводятся, пока не переоткроешь окно настроек».

### `nameof` не переподнимает свойство вложенного объекта

```csharp
// НЕ ДЕЛАЕТ НИЧЕГО
OnPropertyChanged(nameof(SelectedTheme.DisplayName));
```

`nameof` отбрасывает квалификатор, поэтому здесь поднимется
`PropertyChanged("DisplayName")` у `AppSettingsViewModel` — а свойства с
таким именем у него нет. Событие уходит в никуда, и `ThemeOption` о нём
не узнаёт. Для пунктов списка единственный рабочий путь — привязка к
`IObservableLocalizedString`, то есть `ItemTemplate` выше.

## Чего делать нельзя: `{Translate ...}` и `{Format ...}`

```xml
<!-- НЕЛЬЗЯ -->
<TextBlock Text="{Translate Settings.LanguageDescription}" />
```

Эти расширения создают привязку с `Path = "[ключ]"` и рассчитывают, что Avalonia
перевычислит её при смене культуры. Не перевычисляет: адаптер отправляет
`PropertyChanged` с именем `"Item[]"` (соглашение WPF), а Avalonia 12.1.3
такое имя не понимает. Результат: текст замораживается на стартовом языке
до перезапуска приложения. Плюс путь-индексатор — это рефлексия, то есть
несовместимо с NativeAOT.

Разбор и эксперимент — в [troubleshooting.md](troubleshooting.md#известный-баг-перевод-замораживается-при-смене-языка).

## Как добавить новую строку

1. Открыть **все** файлы `Strings.*.json` в
   `src/MouseLabAvaloniaApp/Assets/Translations/`.
2. Добавить ключ в нужную группу во **всех** культурах:

   ```json
   "Settings": {
     "Language": "Language",
     "NewKey": "New value"
   }
   ```

   Пропущенный ключ в одной из культур даст `PTA003` при сборке, а без
   анализатора — тихо покажет английский текст.

3. Пересобрать проект. Новые свойства появятся в `ProTranslateStrings`
   автоматически.
4. Использовать в XAML:

   ```xml
   <TextBlock Text="{Binding Strings.SettingsNewKey}" />
   ```

Ничего в C# менять не нужно. Если после пересборки IntelliSense не предлагает
`Strings.SettingsNewKey` — вернитесь к пункту 1 и проверьте, что ключ есть
**во всех** файлах каталогов.

## Как добавить новый язык

1. Создать `src/MouseLabAvaloniaApp/Assets/Translations/Strings.de-DE.json`
   с **тем же** набором ключей, что и в существующих каталогах.
2. Добавить язык в список выбора в
   `src/MouseLabAvaloniaApp/ViewModels/MainWindowViewModel.cs`:

   ```csharp
   AvailableCultures =
   [
       new CultureOption("en-US", Describe("en-US")),
       new CultureOption("ru-RU", Describe("ru-RU")),
       new CultureOption("de-DE", Describe("de-DE")),
   ];
   ```

3. Пересобрать. `ProTranslateGeneratedTranslationProvider` подхватит новый файл
   автоматически.

`Describe` формирует подпись вроде `German (Germany) (Deutsch (Deutschland))`,
чтобы язык был узнаваем независимо от текущей локали приложения.

> Список языков сейчас задан вручную. Когда языков станет много, его можно
> получать из манифеста, который генерируется из имён файлов:
>
> ```csharp
> using var cultures = ProTranslateProviderManifest.Cultures;
> ```

Если язык не попал в `TranslationFallbackOptions.DefaultCulture` и не имеет
родительской культуры, непокрытые ключи откатятся к `en-US`
(`AppSettingsStore.DefaultCultureName`).

## Строки с параметрами

Плейсхолдеры `{0}`, `{1}` подставляются через `Translations.Format`:

```json
"GreetingWithName": "Hello, {0}. Your localized workspace is ready."
```

```csharp
Translations.Format(ProTranslateKeys.AppGreetingWithName, "Alex");
```

Правила:

* число и порядок плейсхолдеров должны совпадать во **всех** культурах —
  иначе `PTA002`;
* на каждый ключ генератор создаёт четыре члена. Для ключа `App.GreetingWithName`
  это:

  | Член | Тип | Назначение |
  |---|---|---|
  | `Strings.AppGreetingWithName` | `string` | значение без подстановок |
  | `Strings.Get_AppGreetingWithName()` | `LocalizedString` | значение + метаданные (`ResourceNotFound`, `ProviderName`) |
  | `Strings.Format_AppGreetingWithName(args)` | `string` | значение с подстановкой |
  | `Strings.Observe_AppGreetingWithName(args)` | `IObservableLocalizedString` | наблюдаемое значение, само поднимает уведомления |

  Имена `Format_` / `Observe_` — с подчёркиванием, чтобы не совпасть со
  свойством `AppGreetingWithName`. Это сгенерированные имена, менять их нельзя.

* для диагностики удобно проверять `ResourceNotFound`:

  ```csharp
  LocalizedString value = Strings.Get_SettingsLanguage();
  if (value.ResourceNotFound)
      Debug.WriteLine($"Нет перевода: {value.Key} для {value.Culture}");
  ```

## Направление текста (RTL)

В корне `MainWindow.axaml`:

```xml
Translation.AutoFlowDirection="True"
```

Для арабского, иврита и персидского окно автоматически развернётся
справа налево. Отдельно включать не нужно.

## Культура в настройках против культуры в рантайме

Это разные вещи, и их важно не путать.

| | `IApplicationSettingsService` | `ICultureService` |
|---|---|---|
| Отвечает за | выбор пользователя | текущую культуру в рантайме |
| Хранит | да, в `settings.json` | нет |
| Меняется через | `CurrentCultureName = "de-DE"` | `SetCulture(culture)` |

Переключение языка в UI делается в два шага — так оба сервиса остаются
согласованными:

```csharp
_settings.CurrentCultureName = value.CultureName;  // запомнить
_cultures.SetCulture(value.Culture);               // применить
```

`SetCulture` сам поднимает `CultureChanged`, на который подписаны
`ProTranslateStrings` (обновляет строки) и `AppSettingsViewModel`
(переподнимает вычисляемые свойства). Поэтому вручную «обновлять» переводы
не нужно — достаточно правильно переподнять свои собственные свойства.

Ручной вызов `Strings.Refresh()` в обработчике не нужен: `ProTranslateStrings`
подписан на `CultureChanged` сам и вызывает `Refresh()` из своего
`OnCultureChanged`.

### Когда происходит переключение

Сейчас — **по кнопке Save**, а не сразу при выборе пункта. Свойства
`SelectedCulture` и `SelectedTheme` только запоминают выбор
(`AppSettingsViewModel`), а применяют его `SyncSelectedCultureToSettings()`
и `SyncSelectedThemeToSettings()` из `SaveSettingsCommand`.

Это осознанный выбор: язык и тема меняются вместе, одним действием.
Чтобы вернуть мгновенное переключение, достаточно раскомментировать две
строки в сеттерах `AppSettingsViewModel` — остальной механизм (события,
`AppSettingsStore.Save`, обновление подписей) уже готов.
