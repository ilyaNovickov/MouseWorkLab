# Способы создания раскладки (layout)

В Dock есть два основных подхода к построению раскладки. В вашем проекте уже
реализован первый (code-first через `Factory`).

## 1. Code-first (через фабрику `Factory`) — то, что у вас

Вы создаёте класс, наследующий `Dock.Model.Mvvm.Factory`, и перегружаете
`CreateLayout()` и `InitLayout(...)`. Это даёт полный контроль над деревом
раскладки в коде C#.

Пример из вашего `DockFactory.cs`:

```csharp
public override IRootDock CreateLayout()
{
    var document = new Document
    {
        Id = "PlotDocument",
        Title = "Параметры",
        Context = plotViewModel,   // ваша модель-данных
        CanClose = false,
        CanFloat = true
    };

    var documentDock = new DocumentDock
    {
        Id = "Documents",
        Title = "Documents",
        IsCollapsable = false,
        CanCreateDocument = false,
        ActiveDockable = document,
        VisibleDockables = CreateList<IDockable>(document)
    };

    var rootDock = CreateRootDock();
    rootDock.Id = "Root";
    rootDock.ActiveDockable = documentDock;
    rootDock.DefaultDockable = documentDock;
    rootDock.VisibleDockables = CreateList<IDockable>(documentDock);
    return rootDock;
}
```

`InitLayout` нужен, чтобы связать идентификаторы с реальными объектами
(`DockableLocator`) и указать, как создавать плавающие окна
(`HostWindowLocator`):

```csharp
public override void InitLayout(IDockable layout)
{
    DockableLocator = new Dictionary<string, Func<IDockable?>>
    {
        ["Root"] = () => _rootDock,
        ["Documents"] = () => _documentDock
    };
    HostWindowLocator = new Dictionary<string, Func<IHostWindow?>>
    {
        [nameof(IDockWindow)] = () => new HostWindow()
    };
    base.InitLayout(layout);
}
```

Инициализация в окне (code-behind):

```csharp
var layout = _factory.CreateLayout();
_factory.InitLayout(layout);
Dock.Layout = layout;
```

Где `Dock` — это `<DockControl x:Name="Dock" />` в XAML.

## 2. ItemsSource (декларативно в XAML) — рекомендуемый для динамики

Подходит, когда документы/инструменты приходят из коллекции (например, список
открытых файлов). `DocumentDock`/`ToolDock` сами создают dockable из элементов
`ObservableCollection`, отслеживая изменения.

```xml
<DockControl InitializeLayout="True" InitializeFactory="True">
  <DockControl.Factory>
    <dockFactory:Factory />
  </DockControl.Factory>
  <dockModel:RootDock>
    <dockModel:DocumentDock CanCreateDocument="False"
        ItemsSource="{Binding #RootWindow.((vm:MainWindowViewModel)DataContext).Documents}">
      <dockModel:DocumentDock.DocumentTemplate>
        <dockModel:DocumentTemplate>
          <StackPanel x:DataType="dockModel:Document" Margin="10">
            <TextBlock Text="{Binding Title}" />
            <ContentControl DataContext="{Binding Context}">
              <!-- шаблон контента вашей модели -->
            </ContentControl>
          </StackPanel>
        </dockModel:DocumentTemplate>
      </dockModel:DocumentDock.DocumentTemplate>
    </dockModel:DocumentDock>
  </dockModel:RootDock>
</DockControl>
```

Преимущества: автоматическое создание/удаление документов при изменении
коллекции, меньше «канцелярии» (boilerplate), чистое разделение бизнес-моделей
и UI.

## Какой подход выбрать

- **Code-first (`Factory`)** — когда раскладка фиксированная и задаётся в коде
  (ваш случай с одним документом «Параметры» + `PlotViewModel`).
- **ItemsSource** — когда список документов динамический (открытые файлы,
  вкладки, сгенерированные из плагинов).

Оба подхода совместимы: можно в `Factory.CreateLayout()` создать корневую
структуру, а отдельные `DocumentDock` питать через `ItemsSource`.
