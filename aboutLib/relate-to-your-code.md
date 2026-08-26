# Связь с вашим кодом (DockFactory и ViewLocator)

Здесь разобрано, как ваши существующие классы вписываются в архитектуру Dock
и какие возможности библиотеки вы уже используете, а какие — можете добавить.

## Ваш `DockFactory.cs`

Файл: `src/MouseLabAvalonia/ViewModels/DockFactory.cs`

Вы используете **code-first подход** через `Dock.Model.Mvvm.Factory`:

- `CreateLayout()` строит дерево `RootDock → DocumentDock → Document`.
- Документу `PlotDocument` назначен `Context = plotViewModel` — это ваша
  модель данных `PlotViewModel`. Именно через `Context` контент находит
  нужное view (см. `ViewLocator`).
- `CanClose = false` запрещает закрытие документа, `CanFloat = true` разрешает
  перетаскивание во всплывающее окно.
- `CanCreateDocument = false` на `DocumentDock` означает, что новые вкладки не
  создаются автоматически (нет кнопки «+»). Если захотите динамические
  вкладки — либо включите `CanCreateDocument = true` с реализацией создания,
  либо перейдите на `ItemsSource` (см. layout-creation.md).
- `DockableLocator` связывает строковые `Id` (`"Root"`, `"Documents"`) с
  реальными объектами — нужно для восстановления ссылок при сериализации.
- `HostWindowLocator` указывает, какой класс окна создавать для плавающих
  окон (`HostWindow` из `Dock.Avalonia.Controls`).
- `CreateWindowFrom` задаёт заголовок плавающего окна.

**Что можно добавить:**

- Несколько `ToolDock` сбоку (панель свойств/навигации) — добавить ещё
  dockable в `VisibleDockables` корня с `Splitter` между ними.
- Динамические документы через `ItemsSource` (коллекция `ObservableCollection`
  документов вместо одного `PlotDocument`).
- Сохранение раскладки (см. floating-windows-and-persistence.md).

## Ваш `ViewLocator.cs`

Файл: `src/MouseLabAvalonia/ViewLocator.cs`

Это **конвенционный локатор view** (вариант «ViewModel → View»):

- `Build()` берёт данные; если это `IDockable` — использует `Context`
  (или сам dockable, если `Context == null`); затем ищет тип view по имени,
  заменяя `"ViewModel"` на `"View"` (например, `PlotViewModel` → `PlotView`).
- `Match()` возвращает `true` для `ViewModelBase` или для `IDockable`, чей
  `Context`/сам объект имеет соответствующий view.

Это означает: чтобы добавить новый документ/инструмент, достаточно создать
пару `FooViewModel` + `FooView`, и Dock через `Context` сам подставит view.
Это метод №2 из content-methods.md.

**Рекомендация:** существующий `ViewLocator` использует рефлексию
(`Activator.CreateInstance`), о чём предупреждает атрибут
`RequiresUnreferencedCode` (проблема при тримминге/AOT). Если планируете
публикацию с триммингом, рассмотрите `StaticViewLocator` (source generator),
который генерирует словарь view без рефлексии в рантайме.

## Как это всё соединяется в рантайме

1. `DockFactory.CreateLayout()` создаёт модель раскладки.
2. `InitLayout()` регистрирует локаторы (`DockableLocator`, `HostWindowLocator`).
3. `<DockControl>` в XAML привязывается к `Dock.Layout` и отрисовывает модель.
4. Для каждого отображаемого dockable вызывается `ViewLocator.Build()`,
   который по `Context` находит нужное `View` и ставит `DataContext`.

## Идеи для расширения вашего приложения

- Добавить боковую `ToolDock` с панелью свойств выделенного объекта.
- Включить `DeferredContentControl`, если `PlotView` тяжёлый.
- Сохранять раскладку (включая положение плавающего окна графика) между
  запусками через `Dock.Serializer.*`.
- Перейти на `ItemsSource`, если планируется несколько графиков/документов
  одновременно.
