# Методы наполнения документов и инструментов контентом

Контент для `Document` и `Tool` можно задавать четырьмя способами. В вашем
проекте используется способ №2 (ViewModel + ViewLocator), где контент
подставляется через `Context` и соглашение об именовании `ViewModel` → `View`.

## 1. ItemsSource (коллекция) — рекомендуемый

`DocumentDock.ItemsSource` + `DocumentTemplate`. Каждый элемент коллекции
становится `Document`, а его свойства (`Title`, `CanClose` и т.д.)
автоматически проецируются. Ваша модель становится `Context` созданного
документа. См. [layout-creation.md](layout-creation.md).

## 2. ViewModel + DataTemplate (ваш способ через ViewLocator)

Dockable хранит в `Context` объект-модель. Система шаблонов Avalonia по
соглашению подбирает view. Ваш `ViewLocator` делает это по имени:

```csharp
var content = data is IDockable dockable ? (dockable.Context ?? dockable) : data;
var type = ResolveViewType(content); // "ViewModel" -> "View"
```

Для `Document { Context = plotViewModel }` локатор найдёт
`PlotViewModel` → `PlotView` и установит `DataContext = plotViewModel`.

Преимущество подхода: dockable сам по себе — лёгкая обёртка модели Dock, а вся
логика и данные живут в `Context`. View остаётся пассивным.

Важно для компилируемых привязок (compiled bindings): `Document.Context`
имеет тип `object?`, поэтому внутри `DocumentTemplate` нужно «пере-привязать»
поддерево к `Context` и указать `x:DataType` вашей модели:

```xml
<DocumentTemplate>
  <StackPanel x:DataType="Document">
    <StackPanel DataContext="{Binding Context}" x:DataType="vm:PlotViewModel">
      <TextBox Text="{Binding SomeProperty}" />
    </StackPanel>
  </StackPanel>
</DocumentTemplate>
```

## 3. Function-based контент

Контент создаётся функцией (удобно для DI):

```csharp
var document = new Document
{
    Id = "FuncDoc",
    Title = "Function Document",
    Content = new Func<IServiceProvider, object>(_ => new MyUserControl())
};
```

## 4. Прямой XAML-контент

Статический контент прямо внутри `Document`/`Tool`:

```xml
<dock:Document Id="Welcome" Title="Welcome" CanClose="False">
  <StackPanel Margin="10">
    <TextBlock Text="Привет!" />
  </StackPanel>
</dock:Document>
```

⚠️ Такой контент — шаблонный: Dock материализует его при показе и может
отсоединить/переиспользовать. Не ищите вложенные контролы через
`FindControl` из родительского окна сразу после `InitializeComponent()` —
они ещё не созданы. Жизненный цикл рендереров (OpenGL, WebView, медиа)
лучше держать внутри самого контента (кастомный контрол/поведение).

## Работа с инструментами (ToolDock)

Инструменты (`Tool`) работают аналогично документам, но обычно располагаются
сбоку и не закрываются (например, панель свойств). `ToolDock` имеет
`Alignment` (Left/Right/Top/Bottom) и поддерживает `ToolTemplate` для
ItemsSource-подхода.

```csharp
public class PropertiesToolViewModel : Tool
{
    public PropertiesToolViewModel()
    {
        Id = "PropertiesTool";
        Title = "Properties";
        CanClose = false;
    }
}
```

## Частые ошибки

- **«Unexpected content»** — вы задали `Content` = view-model без шаблона.
  Используйте DataTemplate/ViewLocator или передавайте `Control`/функцию.
- **Пустые вкладки при ItemsSource** — неверный `x:DataType` или модель не
  реализует `INotifyPropertyChanged`.
- **Ошибки компилируемых привязок `Context.*`** — пере-привяжите поддерево к
  `Context` и задайте `x:DataType` нужной модели.
