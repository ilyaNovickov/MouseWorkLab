using Avalonia.Controls.PanAndZoom;
using MouseLabAvaloniaDesktop.Models.Settings;
using MouseLabAvaloniaDesktop.Services.Implementations;
using MouseLabAvaloniaDesktop.ViewModels.Worktop;
using MouseLabAvaloniaDesktop.ViewModels.Worktop.NodesAndMore;
using Xunit;
using ModelsPoint = MouseLabAvaloniaDesktop.Models.Point;
using Vector = Avalonia.Vector;

namespace MouseLabAvaloniaDesktop.Tests;

public class WorkspaceInteractionTests
{
    private static WorktopViewModel CreateViewModel() =>
        new(new FakeTranslationService(), new ModelingEnv(new MatrixRandomizer()), new ModelingSettings());

    [Fact]
    public void DefaultModeIsAdd()
    {
        using WorktopViewModel vm = CreateViewModel();

        Assert.Equal(WorkspaceMode.Add, vm.WorkspaceMode);
    }

    [Fact]
    public void SetModeUpdatesCanInteractForAllNodes()
    {
        using WorktopViewModel vm = CreateViewModel();
        vm.OnCanvasPressed(new ModelsPoint(1, 2));
        var first = vm.Nodes[0];

        vm.SetMode(WorkspaceMode.SelectAndMove);
        Assert.True(first.CanInteract);

        vm.SetMode(WorkspaceMode.Add);
        Assert.False(first.CanInteract);

        vm.OnCanvasPressed(new ModelsPoint(3, 4));
        Assert.False(vm.Nodes[1].CanInteract);
    }

    [Fact]
    public void CanvasPressAddsNodeOnlyInAddMode()
    {
        using WorktopViewModel vm = CreateViewModel();

        vm.OnCanvasPressed(new ModelsPoint(5, 6));
        Assert.Single(vm.Nodes);

        vm.SetMode(WorkspaceMode.SelectAndMove);
        vm.OnCanvasPressed(new ModelsPoint(7, 8));
        Assert.Single(vm.Nodes);
    }

    [Fact]
    public void SelectionKeepsOnlyOneNode()
    {
        using WorktopViewModel vm = CreateViewModel();
        vm.OnCanvasPressed(new ModelsPoint(1, 1));
        vm.OnCanvasPressed(new ModelsPoint(2, 2));
        vm.SetMode(WorkspaceMode.SelectAndMove);

        var first = vm.Nodes[0];
        var second = vm.Nodes[1];

        vm.SelectNode(first);
        Assert.Same(first, vm.SelectedNode);
        Assert.True(first.IsSelected);

        vm.SelectNode(second);
        Assert.Same(second, vm.SelectedNode);
        Assert.False(first.IsSelected);
        Assert.True(second.IsSelected);
    }

    [Fact]
    public void MoveNodeFromOriginWorksOnlyInSelectMode()
    {
        using WorktopViewModel vm = CreateViewModel();
        vm.OnCanvasPressed(new ModelsPoint(10, 10));
        var node = Assert.Single(vm.Nodes);
        int originX = node.X;
        int originY = node.Y;

        vm.MoveNodeFromOrigin(node, originX, originY, new Vector(3, 4));
        Assert.Equal(originX, node.X);
        Assert.Equal(originY, node.Y);

        vm.SetMode(WorkspaceMode.SelectAndMove);
        vm.MoveNodeFromOrigin(node, originX, originY, new Vector(3, 4));
        Assert.Equal(originX + 3, node.X);
        Assert.Equal(originY + 4, node.Y);
    }

    [Fact]
    public void MoveNodeFromOriginKeepsSubPixelOffset()
    {
        using WorktopViewModel vm = CreateViewModel();
        vm.OnCanvasPressed(new ModelsPoint(10, 10));
        var node = Assert.Single(vm.Nodes);
        int originX = node.X;
        int originY = node.Y;
        vm.SetMode(WorkspaceMode.SelectAndMove);

        vm.MoveNodeFromOrigin(node, originX, originY, new Vector(0.4, 0.4));
        Assert.Equal(originX, node.X);
        Assert.Equal(originY, node.Y);

        vm.MoveNodeFromOrigin(node, originX, originY, new Vector(0.6, 0.6));
        Assert.Equal(originX + 1, node.X);
        Assert.Equal(originY + 1, node.Y);
    }

    [Fact]
    public void PanButtonFollowsMode()
    {
        using WorktopViewModel vm = CreateViewModel();

        Assert.Equal(ButtonName.Middle, vm.PanButton);

        vm.SetMode(WorkspaceMode.PanOnly);
        Assert.Equal(ButtonName.Left, vm.PanButton);

        vm.SetMode(WorkspaceMode.SelectAndMove);
        Assert.Equal(ButtonName.Middle, vm.PanButton);
    }

    [Fact]
    public void CanvasPressInSelectModeClearsSelection()
    {
        using WorktopViewModel vm = CreateViewModel();
        vm.OnCanvasPressed(new ModelsPoint(1, 1));
        vm.SetMode(WorkspaceMode.SelectAndMove);
        var node = Assert.Single(vm.Nodes);
        vm.SelectNode(node);

        vm.OnCanvasPressed(new ModelsPoint(50, 50));

        Assert.Null(vm.SelectedNode);
        Assert.False(node.IsSelected);
    }

    [Fact]
    public void LeavingSelectModeClearsSelection()
    {
        using WorktopViewModel vm = CreateViewModel();
        vm.OnCanvasPressed(new ModelsPoint(1, 1));
        vm.SetMode(WorkspaceMode.SelectAndMove);
        var node = Assert.Single(vm.Nodes);
        vm.SelectNode(node);

        vm.SetMode(WorkspaceMode.PanOnly);

        Assert.Null(vm.SelectedNode);
        Assert.False(node.IsSelected);
    }

    [Fact]
    public void SelectingConnectionClearsNodeSelection()
    {
        using WorktopViewModel vm = CreateViewModel();
        vm.OnCanvasPressed(new ModelsPoint(1, 1));
        vm.OnCanvasPressed(new ModelsPoint(50, 50));
        vm.SetMode(WorkspaceMode.SelectAndMove);
        var node = vm.Nodes[0];
        var connection = Assert.Single(vm.Connections);

        vm.SelectNode(node);
        Assert.True(node.IsSelected);

        vm.SelectConnection(connection);

        Assert.Null(vm.SelectedNode);
        Assert.False(node.IsSelected);
        Assert.Same(connection, vm.SelectedConnection);
        Assert.True(connection.IsSelected);
    }

    [Fact]
    public void SelectingNodeClearsConnectionSelection()
    {
        using WorktopViewModel vm = CreateViewModel();
        vm.OnCanvasPressed(new ModelsPoint(1, 1));
        vm.OnCanvasPressed(new ModelsPoint(50, 50));
        vm.SetMode(WorkspaceMode.SelectAndMove);
        var node = vm.Nodes[0];
        var connection = Assert.Single(vm.Connections);

        vm.SelectConnection(connection);
        Assert.True(connection.IsSelected);

        vm.SelectNode(node);

        Assert.Null(vm.SelectedConnection);
        Assert.False(connection.IsSelected);
        Assert.Same(node, vm.SelectedNode);
        Assert.True(node.IsSelected);
    }

    [Fact]
    public void ConnectionSelectionWorksOnlyInSelectMode()
    {
        using WorktopViewModel vm = CreateViewModel();
        vm.OnCanvasPressed(new ModelsPoint(1, 1));
        vm.OnCanvasPressed(new ModelsPoint(50, 50));
        var connection = Assert.Single(vm.Connections);

        vm.SelectConnection(connection);
        Assert.Null(vm.SelectedConnection);

        vm.SetMode(WorkspaceMode.SelectAndMove);
        vm.SelectConnection(connection);
        Assert.Same(connection, vm.SelectedConnection);

        vm.SetMode(WorkspaceMode.Add);
        Assert.Null(vm.SelectedConnection);
        Assert.False(connection.IsSelected);
    }

    [Fact]
    public void ArrowTargetsCenterOfSecondNode()
    {
        using var connection = new ConnectionViewModel(new NodeViewModel(0, 0), new NodeViewModel(100, 0));

        Assert.Equal(8, connection.Start.X);
        Assert.Equal(100, connection.End.X);
        Assert.Equal(104, connection.ArrowTo.X);
        Assert.Equal(4, connection.ArrowTo.Y);
    }
}
