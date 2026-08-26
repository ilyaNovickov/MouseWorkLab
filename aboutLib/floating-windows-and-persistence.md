# Плавающие окна, сложные раскладки и сохранение состояния

Помимо базовой раскладки, Dock предоставляет продвинутые сценарии, которые
стоит учитывать при расширении вашего приложения.

## Плавающие окна (Floating windows)

Любой dockable можно «отпустить» в отдельное окно. В вашем `DockFactory`
документ уже имеет `CanFloat = true`, поэтому пользователь сможет перетащить
вкладку в плавающее окно мышью. Программно это делается так:

```csharp
var document = factory.GetDockable<IDocument>("PlotDocument");
if (document is not null)
{
    factory.FloatDockable(document);
}
```

Заголовок/размеры плавающего окна настраиваются в `CreateWindowFrom`:

```csharp
public override IDockWindow? CreateWindowFrom(IDockable dockable)
{
    var window = base.CreateWindowFrom(dockable);
    if (window is not null)
        window.Title = "Dock Avalonia Demo";
    return window;
}
```

(Этот метод уже переопределён у вас в `DockFactory.cs`.)

## Split-раскладки (несколько панелей)

Чтобы добавить боковые инструменты рядом с документами, в `RootDock`
добавляют несколько дочерних доков (например, `ToolDock` + `DocumentDock`),
а `Splitter`/`ProportionalStackPanel` задаёт пропорции. В code-first это
выглядит как вложенные `VisibleDockables` с `Splitter` между ними.

## Многоконные раскладки (multi-window)

Фабрика по умолчанию может открывать dockable в отдельном верхнеуровневом
окне. Это позволяет разнести приложение по нескольким окнам ОС, сохраняя
единое дерево докинга.

## Плагины (загрузка dockable в рантайме)

Приложение может догружать документы/инструменты из внешних сборок:

```csharp
public interface IPlugin
{
    IDockable CreateDockable();
}

// Загрузка:
var assembly = Assembly.LoadFrom(path);
foreach (var plugin in assembly.GetTypes()
    .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract)
    .Select(t => (IPlugin)Activator.CreateInstance(t)!))
{
    var dockable = plugin.CreateDockable();
    factory.AddDockable(rootLayout, dockable);
}
```

## Сохранение и восстановление раскладки (persistence)

Состояние раскладки полностью сериализуется. Выберите один сериализатор:

```
dotnet add package Dock.Serializer.Newtonsoft
dotnet add package Dock.Serializer.SystemTextJson
dotnet add package Dock.Serializer.Protobuf
dotnet add package Dock.Serializer.Xml
dotnet add package Dock.Serializer.Yaml
```

Сохранение:

```csharp
var serializer = new DockSerializer(new NewtonsoftJsonSerializer());
await using var write = File.OpenWrite("layout.json");
serializer.Save(write, dockControl.Layout);
```

Восстановление (обязательно вызвать `InitLayout` заново):

```csharp
await using var read = File.OpenRead("layout.json");
var layout = serializer.Load<IDock?>(read);
if (layout is { })
{
    dockControl.Factory?.InitLayout(layout);
    dockControl.Layout = layout;
}
```

Таким образом позиции вкладок, плавающих окон и активных элементов
восстановятся при следующем запуске — удобно для IDE-подобных приложений.

## Производительность: отложенный контент и recycling

- **Deferred content** (`Dock.Controls.DeferredContentControl`) — контент
  документа/инструмента создаётся только при его активации, а не сразу для
  всех вкладок.
- **Recycling** (`Dock.Controls.Recycling`) — переиспользование визуальных
  контролов при переключении вкладок, снижая потребление памяти.

Это важно, если вкладок много или контент «тяжёлый» (графики, рендереры).

## Темы и стили

Подключаются в `App.axaml`:

```xml
<Application.Styles>
  <FluentTheme Mode="Dark" />
  <DockFluentTheme />
</Application.Styles>
```

Доступны темы: `Dock.Avalonia.Themes.Fluent`, `Dock.Avalonia.Themes.Browser`
(для WASM/браузера), `Dock.Avalonia.Themes.Simple`.

## Диагностика

Пакет `Dock.Avalonia.Diagnostics` добавляет оверлеи/инспекторы для отладки
раскладки (полезно при сложных сценариях).
