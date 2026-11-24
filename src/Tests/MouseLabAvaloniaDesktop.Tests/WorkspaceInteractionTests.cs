using System;
using Avalonia.Controls.PanAndZoom;
using MouseLabAvaloniaDesktop.Models;
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
    public void ArrowSitsOnTargetBoundaryAndPointsToCenter()
    {
        using var connection = new ConnectionViewModel(new NodeViewModel(0, 0), new NodeViewModel(100, 0));

        Assert.Equal(7, connection.Start.X);
        Assert.Equal(101, connection.End.X);
        Assert.Equal(100, connection.ArrowTo.X);
        Assert.Equal(4, connection.ArrowTo.Y);
        Assert.True(connection.ArrowFrom.X < connection.ArrowTo.X);
    }

    [Fact]
    public void CurvedArrowTipRestsOnTargetCircle()
    {
        using var connection = new ConnectionViewModel(new NodeViewModel(0, 0), new NodeViewModel(100, 0))
        {
            Kind = ConnectionKinds.Curved
        };

        Assert.Equal(7, connection.Start.X);
        Assert.Equal(101, connection.End.X);
        double relX = connection.ArrowTo.X - 104;
        double relY = connection.ArrowTo.Y - 4;
        Assert.Equal(4d, Math.Sqrt(relX * relX + relY * relY), 6);
        Assert.True(connection.ArrowFrom.X < connection.ArrowTo.X);
    }

    [Fact]
    public void SelectedKindOptionChangesSelectedConnectionKind()
    {
        using WorktopViewModel vm = CreateViewModel();
        vm.OnCanvasPressed(new ModelsPoint(1, 1));
        vm.OnCanvasPressed(new ModelsPoint(50, 50));
        vm.SetMode(WorkspaceMode.SelectAndMove);
        var connection = Assert.Single(vm.Connections);
        vm.SelectConnection(connection);

        Assert.True(vm.HasSelectedConnection);
        Assert.Equal(ConnectionKinds.Straight, vm.SelectedConnectionKindOption!.Kind);

        ConnectionKindOption curved = Assert.Single(
            vm.ConnectionKindOptions, option => option.Kind == ConnectionKinds.Curved);
        vm.SelectedConnectionKindOption = curved;

        Assert.Equal(ConnectionKinds.Curved, connection.Kind);
        Assert.True(connection.ShowHandles);
        Assert.Same(curved, vm.SelectedConnectionKindOption);

        vm.ClearSelection();
        Assert.False(vm.HasSelectedConnection);
        Assert.Null(vm.SelectedConnectionKindOption);
    }

    [Fact]
    public void KindOptionWithoutSelectedConnectionIsIgnored()
    {
        using WorktopViewModel vm = CreateViewModel();
        ConnectionKindOption curved = Assert.Single(
            vm.ConnectionKindOptions, option => option.Kind == ConnectionKinds.Curved);

        vm.SelectedConnectionKindOption = curved;

        Assert.Null(vm.SelectedConnectionKindOption);
    }

    [Fact]
    public void DraggedControlPointSurvivesNodeMoveAndKindToggle()
    {
        using var connection = new ConnectionViewModel(new NodeViewModel(0, 0), new NodeViewModel(100, 0))
        {
            Kind = ConnectionKinds.Curved
        };

        connection.SetControlPoint(0, new PointD(20, 30));
        Assert.True(connection.HasManualControlPoints);
        Assert.Equal(20, connection.Control1.X);
        Assert.Equal(30, connection.Control1.Y);

        connection.Target.Y = 50;
        Assert.Equal(20, connection.Control1.X);
        Assert.Equal(30, connection.Control1.Y);
        Assert.NotEqual(4, connection.Start.Y);

        connection.Kind = ConnectionKinds.Straight;
        connection.Kind = ConnectionKinds.Curved;
        Assert.Equal(20, connection.Control1.X);
        Assert.Equal(30, connection.Control1.Y);
    }

    [Fact]
    public void ControlPointEditRequiresCurvedKindAndValidIndex()
    {
        using var connection = new ConnectionViewModel(new NodeViewModel(0, 0), new NodeViewModel(100, 0));

        connection.SetControlPoint(0, new PointD(20, 30));
        Assert.False(connection.HasManualControlPoints);

        connection.Kind = ConnectionKinds.Curved;
        connection.SetControlPoint(5, new PointD(20, 30));
        Assert.False(connection.HasManualControlPoints);
    }

    [Fact]
    public void HandleMoveRequiresSelectedCurvedConnection()
    {
        using WorktopViewModel vm = CreateViewModel();
        vm.OnCanvasPressed(new ModelsPoint(1, 1));
        vm.OnCanvasPressed(new ModelsPoint(50, 50));
        vm.SetMode(WorkspaceMode.SelectAndMove);
        var connection = Assert.Single(vm.Connections);
        connection.Kind = ConnectionKinds.Curved;
        vm.SelectConnection(connection);

        vm.SetConnectionControlPoint(connection, 0, new PointD(25, 35));
        Assert.Equal(25, connection.Control1.X);
        Assert.Equal(35, connection.Control1.Y);

        vm.ClearSelection();
        vm.SetConnectionControlPoint(connection, 0, new PointD(1, 1));
        Assert.Equal(25, connection.Control1.X);
        Assert.Equal(35, connection.Control1.Y);
    }

    [Fact]
    public void CurvedArrowTipLiesOnTargetCircleAlongCurveTangent()
    {
        using var connection = new ConnectionViewModel(new NodeViewModel(0, 0), new NodeViewModel(100, 0))
        {
            Kind = ConnectionKinds.Curved
        };

        double relX = connection.ArrowTo.X - 104;
        double relY = connection.ArrowTo.Y - 4;
        double tipFromEndX = connection.ArrowTo.X - connection.End.X;
        double tipFromEndY = connection.ArrowTo.Y - connection.End.Y;
        double tangentX = connection.End.X - connection.Control2.X;
        double tangentY = connection.End.Y - connection.Control2.Y;

        Assert.Equal(4d, Math.Sqrt(relX * relX + relY * relY), 6);
        Assert.Equal(0d, tangentX * tipFromEndY - tangentY * tipFromEndX, 6);
    }

    [Fact]
    public void DraggingControlPointRotatesCurvedArrow()
    {
        using var connection = new ConnectionViewModel(new NodeViewModel(0, 0), new NodeViewModel(100, 0))
        {
            Kind = ConnectionKinds.Curved
        };
        double originalTipY = connection.ArrowTo.Y;

        connection.SetControlPoint(1, new PointD(61, 50));

        Assert.True(connection.ArrowTo.Y < originalTipY);
        Assert.True(connection.ArrowTo.Y < connection.ArrowFrom.Y);
    }
}
