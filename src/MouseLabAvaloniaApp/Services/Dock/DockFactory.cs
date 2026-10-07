using System;
using Dock.Model.Mvvm.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm;

namespace MouseLabAvaloniaApp.Services.Dock;

/// <summary>
/// Собирает макет доков и создаёт их элементы.
/// </summary>
/// <remarks>
/// Макет строится здесь, а не в XAML: <c>DockControl</c> получает готовый
/// <see cref="IRootDock"/> через привязку <c>Layout</c>. Инлайновое дерево
/// доков в разметке создавало бы второй, не связанный с этим макет.
/// </remarks>
public class DockFactory : Factory
{
    private IRootDock? _rootDock;

    private IDocumentDock? _documentDock;

    /// <summary>
    /// Док документов, к которому добавляются новые окна.
    /// </summary>
    public IDocumentDock? DocumentDock => _documentDock;

    /// <inheritdoc/>
    public override IRootDock CreateLayout()
    {
        // Пустой док: вкладки появляются только после явного добавления.
        _documentDock = CreateDocumentDock();
        _documentDock.Id = "Documents";
        _documentDock.Title = "Документы";
        _documentDock.IsCollapsable = false;
        _documentDock.CanCreateDocument = false;
        _documentDock.EmptyContent = "Нет открытых документов";

        _rootDock = CreateRootDock();
        _rootDock.Id = "Root";
        _rootDock.IsCollapsable = false;
        _rootDock.ActiveDockable = _documentDock;
        _rootDock.DefaultDockable = _documentDock;
        _rootDock.VisibleDockables = CreateList<IDockable>(_documentDock);

        return _rootDock;
    }

    /// <summary>
    /// Создаёт документ. Заголовки задаются только здесь, поэтому перевод
    /// заголовков дока в дальнейшем затронет только этот метод.
    /// </summary>
    /// <remarks>
    /// Имя <c>NewDocument</c>, а не <c>CreateDocument</c>: у <see cref="Factory"/>
    /// уже есть одноимённый виртуальный метод без параметров, и перегрузка с
    /// другим смыслом сделала бы вызовы <c>CreateDocument()</c> и
    /// <c>CreateDocument("...")</c> неразличимыми на глаз.
    /// </remarks>
    public Document NewDocument(string title) => new()
    {
        Id = Guid.NewGuid().ToString(),
        Title = title,

        // Должен быть всегда `false`, иначе страшный баг.
        CanFloat = false
    };
}