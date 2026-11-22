using MouseLabAvaloniaDesktop.Models.Settings;
using MouseLabAvaloniaDesktop.Services.Implementations;
using MouseLabAvaloniaDesktop.ViewModels.Worktop;
using Xunit;
using ModelsPoint = MouseLabAvaloniaDesktop.Models.Point;
using Vector = Avalonia.Vector;

namespace MouseLab.Tests;

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
    public void MoveNodeAppliesDeltaOnlyInSelectMode()
    {
        using WorktopViewModel vm = CreateViewModel();
        vm.OnCanvasPressed(new ModelsPoint(10, 10));
        var node = Assert.Single(vm.Nodes);

        vm.MoveNode(node, new Vector(3, 4));
        Assert.Equal(10, node.X);
        Assert.Equal(10, node.Y);

        vm.SetMode(WorkspaceMode.SelectAndMove);
        vm.MoveNode(node, new Vector(3, 4));
        Assert.Equal(13, node.X);
        Assert.Equal(14, node.Y);
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
}
