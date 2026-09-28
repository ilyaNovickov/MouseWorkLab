# Диагностика и решение проблем

## Диагностика сборки

`ProTranslate.SourceGenerator` и `ProTranslate.Analyzers` — анализаторы.
Они сообщают о проблемах с переводами на этапе сборки, до запуска приложения.

### Ошибки и предупреждения генератора

| Код | Что означает | Что делать |
|---|---|---|
| `PTSG001` | файл каталога не распознан как JSON-объект | исправить синтаксис JSON. Частая причина — комментарии: JSON их не поддерживает |
| `PTSG002` | один и тот же ключ дважды в одном файле | удалить дубликат. Один ключ в разных культурах — это норма, а не ошибка |

### Диагностика анализатора

| Код | Что означает | Что делать |
|---|---|---|
| `PTA001` | статический ключ есть в коде, но отсутствует в каталогах | добавить ключ в `Strings.*.json` |
| `PTA002` | в вызове `Format` аргументов больше или меньше, чем плейсхолдеров в каталоге | привести в соответствие; проверить все культуры |
| `PTA003` | в каталоге какой-то культуры не хватает ключа, который есть в другом | дописать перевод; это же означает, что пользователь увидит чужой язык |
| `PTA004` | ключ передаётся вычислением, проверить его невозможно | заменить на константу `ProTranslateKeys.Ключ` или свойство `Strings.Ключ` |
| `PTA005` | файл каталога повреждён или не поддерживается | см. `PTSG001` |

`PTA001`–`PTA003` имеет смысл поднять до ошибок в `.editorconfig`, когда
набор ключей стабилизируется:

```ini
dotnet_diagnostic.PTA001.severity = error
dotnet_diagnostic.PTA002.severity = error
dotnet_diagnostic.PTA003.severity = error
```

В проекте сейчас подавлен только `PTA004` — на два метода `T(...)` в
`ViewModelBase`, которые существуют как аварийный выход для ключей,
известных только в рантайме.

## Симптомы в рантайме

| Симптом | Причина | Решение |
|---|---|---|
| В интерфейсе виден сам ключ, например `App.Greeting` | ключа нет ни в одной культуре либо есть опечатка | найти ключ в каталоге; сверить имя вложенности |
| Часть строк перевелась, часть осталась на стартовом языке | в `OnCultureChanged` не переподнято вычисляемое свойство | добавить `OnPropertyChanged(nameof(...))` |
| Текст из `{Translate ...}` не меняется при смене языка | известный баг адаптера, см. следующий раздел | заменить на `{Binding Strings.Ключ}` |
| Перевод виден на старте, но язык не переключается | не вызвано `UseProTranslateAvalonia()` после `BuildServiceProvider()` | см. [architecture.md](architecture.md#порядок-запуска) |
| `NullReferenceException` в моделях представления при старте | `MainWindowViewModel` в контейнере, который уже `Dispose` | контейнер должен жить в поле, а не в `using` |
| Настройки не сохраняются, сбрасываются при запуске | исключение внутри `AppSettingsStore.Save` проглочено | см. ниже |
| Сборка падает на `AVLN1001` | комментарий внутри списка атрибутов элемента | вынести комментарий выше или ниже элемента |
| Много `IL2026` / `IL3050` при `PublishAot` | рефлексия, вырезаемая триммером | см. [architecture.md](architecture.md#требования-aot) |

### Как посмотреть «проглоченные» ошибки настроек

`AppSettingsStore` намеренно ловит все исключения, чтобы битый файл
настроек не мешал запуску. Чтобы временно увидеть причину, в
`Services/AppSettings/AppSettingsStore.cs` замените пустой `catch` на:

```csharp
catch (Exception ex)
{
    System.Diagnostics.Debug.WriteLine($"settings.json: {ex}");
    return new AppSettingsSnapshot();
}
```

## Известный баг: перевод замораживается при смене языка

**Симптом.** `{Translate Ключ}` и `{Format Ключ}` показывают правильный текст
при запуске, но не обновляются при смене языка. Помогает только перезапуск
приложения. При этом `{Binding Strings.Ключ}` в том же окне работает верно.

**Причина.** `TranslateExtension.ProvideValue` создаёт привязку с
`Path = "[ключ]"` и `Source = TranslationBindingSource`. Чтобы обновить такие
привязки, адаптер шлёт:

```csharp
PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
```

`"Item[]"` — соглашение WPF для «индексатор изменился». Avalonia его не
понимает и не перевычисляет привязку. Проверено экспериментом в headless
Avalonia: ни одно из имён — `"Item[]"`, `""`, `null`, точное имя ключа —
не заставляет Avalonia перечитать путь-индексатор.

```csharp
// ProTranslate.Avalonia.TranslationBindingSource.Refresh()
PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
```

Дополнительно путь-индексатор — это рефлексия, поэтому расширение
несовместимо с NativeAOT в любом случае.

**Решение.** Не использовать `{Translate ...}` и `{Format ...}`.
Применять:

| Нужно | Использовать |
|---|---|
| Статичный текст | `{Binding Strings.Ключ}` |
| Текст с параметрами | свойство в `ViewModel` через `Translations.Format` |
| Разовый текст на существующем контроле | `Translation.Key="Ключ"` |

В `ProTranslate.Avalonia` 0.1.0 собственных тестов на `TranslateExtension`
нет — тестируются `Translation.Key`, `FormatExtension` и направление текста,
поэтому баг не был замечен. Стоит сообщить о нём в
[репозитории ProTranslate](https://github.com/wieslawsoltes/ProTranslate/issues).

## Как проверить, что генератор отработал

Посмотреть сгенерированный код:

```bash
dotnet build src/MouseLabAvaloniaApp/MouseLabAvaloniaApp.csproj -t:Rebuild `
  -p:EmitCompilerGeneratedFiles=true -p:CompilerGeneratedFilesOutputPath=obj\gen
```

В `obj\gen\ProTranslate.SourceGenerator\` появятся четыре файла:

```
ProTranslateKeys.g.cs
ProTranslateStrings.g.cs
ProTranslateGeneratedTranslationProvider.g.cs
ProTranslateProviderManifest.g.cs
```

В `ProTranslateGeneratedTranslationProvider.g.cs` должны быть видны все
культуры и значения — например, так:

```csharp
new ProTranslateGeneratedTranslationEntry("App.Greeting", "en-US", "Welcome to MouseLab!"),
new ProTranslateGeneratedTranslationEntry("App.Greeting", "ru-RU", "Добро пожаловать в MouseLab!"),
```

Если файлов нет или в них пусто — проверьте, что каталоги подключены
как `AdditionalFiles` в `csproj` и что имена файлов совпадают с шаблоном
`Strings.*.json`.

> Сгенерированный каталог `obj\gen` создаётся только этой командой и в
> репозиторий не попадает. Обычная сборка его не трогает.

## Чек-лист «перевод не работает»

1. Строка в интерфейсе показывает сам ключ? → ключа нет в каталоге.
2. Ключ есть, но показан чужой язык? → не хватает перевода в текущей
   культуре, проверьте `PTA003` при сборке.
3. Используется `{Translate ...}`? → замените на `{Binding Strings.Ключ}`.
4. Текст вычисляется в `ViewModel`? → есть ли `OnPropertyChanged` в
   `OnCultureChanged`?
5. Строка в XAML есть, а в `ProTranslateStrings` нет? → не пересобрали
   проект после правки каталога.
6. Ничего не помогло → включите вывод в лог и проверьте, что
   `UseProTranslateAvalonia()` вызывается (см. `architecture.md`).
