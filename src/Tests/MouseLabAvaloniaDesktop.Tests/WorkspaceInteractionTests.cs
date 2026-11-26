using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.PanAndZoom;
using MouseLabAvaloniaDesktop.Converters;
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
    public void SeedValidationTreatsEmptyAsValid()
    {
        using WorktopViewModel vm = CreateViewModel();

        Assert.True(vm.IsSeedNumber);
        Assert.False(vm.IsSeedInvalid);

        vm.Seed = string.Empty;
        Assert.True(vm.IsSeedNumber);

        vm.Seed = "   ";
        Assert.True(vm.IsSeedNumber);

        vm.Seed = "123";
        Assert.True(vm.IsSeedNumber);
        Assert.False(vm.IsSeedInvalid);

        vm.Seed = "abc";
        Assert.False(vm.IsSeedNumber);
        Assert.True(vm.IsSeedInvalid);
    }

    [Fact]
    public async Task GenerateUsesParsedSeed()
    {
        var env = new ModelingEnv(new MatrixRandomizer());
        using var vm = new WorktopViewModel(new FakeTranslationService(), env, new ModelingSettings());
        vm.Seed = "42";

        await vm.GenerateWorktopCommand.ExecuteAsync(null);

        Assert.Equal(42, env.Seed);
    }

    [Fact]
    public async Task GenerateWithInvalidSeedUsesGeneratedSeed()
    {
        var env = new ModelingEnv(new MatrixRandomizer());
        using var vm = new WorktopViewModel(new FakeTranslationService(), env, new ModelingSettings());
        vm.Seed = "abc";

        await vm.GenerateWorktopCommand.ExecuteAsync(null);

        Assert.NotNull(env.Seed);
    }

    [Fact]
    public void ModeFlagsAreExclusive()
    {
        using WorktopViewModel vm = CreateViewModel();

        Assert.True(vm.IsAddMode);
        Assert.False(vm.IsPanOnlyMode);
        Assert.False(vm.IsSelectAndMoveMode);
        Assert.False(vm.IsDeleteMode);

        vm.SetMode(WorkspaceMode.SelectAndMove);
        Assert.True(vm.IsSelectAndMoveMode);
        Assert.False(vm.IsAddMode);

        vm.SetMode(WorkspaceMode.Delete);
        Assert.True(vm.IsDeleteMode);
        Assert.False(vm.IsSelectAndMoveMode);

        vm.SetMode(WorkspaceMode.PanOnly);
        Assert.True(vm.IsPanOnlyMode);
        Assert.False(vm.IsDeleteMode);
    }

    [Fact]
    public void ModeFlagSetterOnlyReactsToTrue()
    {
        using WorktopViewModel vm = CreateViewModel();

        vm.IsAddMode = false;
        Assert.Equal(WorkspaceMode.Add, vm.WorkspaceMode);
        Assert.True(vm.IsAddMode);

        vm.IsDeleteMode = true;
        Assert.Equal(WorkspaceMode.Delete, vm.WorkspaceMode);
        Assert.True(vm.IsDeleteMode);
    }

    [Fact]
    public async Task GenerateWorktopRaisesWorktopChanged()
    {
        using WorktopViewModel vm = CreateViewModel();
        bool raised = false;
        vm.WorktopChanged += (_, _) => raised = true;

        await vm.GenerateWorktopCommand.ExecuteAsync(null);

        Assert.True(raised);
    }

    [Fact]
    public void SetModeUpdatesCanInteractForAllNodes()
    {
        using WorktopViewModel vm = CreateViewModel();
        vm.OnCanvasPressed(new ModelsPoint(1, 2));
        var first = vm.Nodes.ElementAt(0);

        vm.SetMode(WorkspaceMode.SelectAndMove);
        Assert.True(first.CanInteract);

        vm.SetMode(WorkspaceMode.Add);
        Assert.False(first.CanInteract);

        vm.OnCanvasPressed(new ModelsPoint(3, 4));
        Assert.False(vm.Nodes.ElementAt(1).CanInteract);
    }

    [Fact]
    public void ModeSetsCursorFlagsOnNodesAndConnections()
    {
        using WorktopViewModel vm = CreateViewModel();
        vm.OnCanvasPressed(new ModelsPoint(1, 2));
        vm.OnCanvasPressed(new ModelsPoint(50, 60));
        var node = vm.Nodes.ElementAt(0);
        var connection = Assert.Single(vm.Connections);

        Assert.False(node.CanInteract);
        Assert.False(node.CanDelete);
        Assert.False(connection.CanInteract);
        Assert.False(connection.CanDelete);

        vm.SetMode(WorkspaceMode.SelectAndMove);
        Assert.True(node.CanInteract);
        Assert.False(node.CanDelete);
        Assert.True(connection.CanInteract);
        Assert.False(connection.CanDelete);

        vm.SetMode(WorkspaceMode.Delete);
        Assert.False(node.CanInteract);
        Assert.True(node.CanDelete);
        Assert.False(connection.CanInteract);
        Assert.True(connection.CanDelete);

        vm.SetMode(WorkspaceMode.PanOnly);
        Assert.False(node.CanInteract);
        Assert.False(node.CanDelete);
        Assert.False(connection.CanInteract);
        Assert.False(connection.CanDelete);
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

        var first = vm.Nodes.ElementAt(0);
        var second = vm.Nodes.ElementAt(1);

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
        var node = vm.Nodes.ElementAt(0);
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
        var node = vm.Nodes.ElementAt(0);
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
    public void CurvedArrowTipLiesOnCurveAndNearSideOfTargetCircle()
    {
        using var connection = new ConnectionViewModel(new NodeViewModel(0, 0), new NodeViewModel(100, 0))
        {
            Kind = ConnectionKinds.Curved
        };

        double relX = connection.ArrowTo.X - 104;
        double relY = connection.ArrowTo.Y - 4;

        Assert.Equal(4d, Math.Sqrt(relX * relX + relY * relY), 6);
        Assert.True(DistanceToCurve(connection, connection.ArrowTo) < 0.1d);
        Assert.True((connection.ArrowTo.X - connection.ArrowFrom.X) * -relX
                    + (connection.ArrowTo.Y - connection.ArrowFrom.Y) * -relY > 0d);
    }

    [Fact]
    public void ArrowStaysOnCurveForStronglyBentConnection()
    {
        var target = new NodeViewModel(135, 456);
        using var connection = new ConnectionViewModel(new NodeViewModel(102, 481), target)
        {
            Kind = ConnectionKinds.Curved
        };
        connection.SetControlPoint(0, new PointD(107.7486, 439.7789));
        connection.SetControlPoint(1, new PointD(140.4384, 457.42638));

        double relX = connection.ArrowTo.X - (target.X + 4);
        double relY = connection.ArrowTo.Y - (target.Y + 4);

        Assert.Equal(4d, Math.Sqrt(relX * relX + relY * relY), 6);
        Assert.True(DistanceToCurve(connection, connection.ArrowTo) < 0.1d);
        Assert.True((connection.ArrowTo.X - connection.ArrowFrom.X) * -relX
                    + (connection.ArrowTo.Y - connection.ArrowFrom.Y) * -relY > 0d);
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

        Assert.True(Math.Abs(connection.ArrowTo.Y - originalTipY) > 1e-6);
        Assert.True(connection.ArrowTo.Y < connection.ArrowFrom.Y);
    }

    [Fact]
    public void DeletingTailNodeRemovesItsLink()
    {
        using WorktopViewModel vm = CreateViewModel();
        AddChain(vm, 3);
        var first = vm.Nodes.ElementAt(0);
        var second = vm.Nodes.ElementAt(1);
        var third = vm.Nodes.ElementAt(2);
        vm.SetMode(WorkspaceMode.Delete);

        vm.DeleteNode(third);

        Assert.Equal(2, vm.Nodes.Count);
        var link = Assert.Single(vm.Connections);
        Assert.Same(first, link.Source);
        Assert.Same(second, link.Target);
    }

    [Fact]
    public void DeletingHeadNodeRemovesItsLink()
    {
        using WorktopViewModel vm = CreateViewModel();
        AddChain(vm, 3);
        var first = vm.Nodes.ElementAt(0);
        var second = vm.Nodes.ElementAt(1);
        var third = vm.Nodes.ElementAt(2);
        vm.SetMode(WorkspaceMode.Delete);

        vm.DeleteNode(first);

        Assert.Equal(2, vm.Nodes.Count);
        var link = Assert.Single(vm.Connections);
        Assert.Same(second, link.Source);
        Assert.Same(third, link.Target);
    }

    [Fact]
    public void DeletingMiddleNodeRewiresChain()
    {
        using WorktopViewModel vm = CreateViewModel();
        AddChain(vm, 3);
        var first = vm.Nodes.ElementAt(0);
        var second = vm.Nodes.ElementAt(1);
        var third = vm.Nodes.ElementAt(2);
        vm.SetMode(WorkspaceMode.Delete);

        vm.DeleteNode(second);

        Assert.Equal(2, vm.Nodes.Count);
        var link = Assert.Single(vm.Connections);
        Assert.Same(first, link.Source);
        Assert.Same(third, link.Target);
        Assert.Equal(ConnectionKinds.Straight, link.Kind);
        Assert.Equal("0", first.Label);
        Assert.Equal("1", third.Label);
    }

    [Fact]
    public void RemovedConnectionsStopTrackingNodes()
    {
        using WorktopViewModel vm = CreateViewModel();
        AddChain(vm, 3);
        var third = vm.Nodes.ElementAt(2);
        var removed = vm.Connections.ElementAt(1);
        double startX = removed.Start.X;
        vm.SetMode(WorkspaceMode.Delete);

        vm.DeleteNode(vm.Nodes.ElementAt(1));
        third.X += 10;

        Assert.Equal(startX, removed.Start.X);
    }

    [Fact]
    public void DeletingOnlyNodeLeavesNoConnections()
    {
        using WorktopViewModel vm = CreateViewModel();
        AddChain(vm, 1);
        vm.SetMode(WorkspaceMode.Delete);

        vm.DeleteNode(vm.Nodes.ElementAt(0));

        Assert.Empty(vm.Nodes);
        Assert.Empty(vm.Connections);
    }

    [Fact]
    public void DeleteNodeIgnoredOutsideDeleteMode()
    {
        using WorktopViewModel vm = CreateViewModel();
        AddChain(vm, 3);

        vm.DeleteNode(vm.Nodes.ElementAt(1));

        Assert.Equal(3, vm.Nodes.Count);
        Assert.Equal(2, vm.Connections.Count);
    }

    [Fact]
    public void NodeAddedAfterDeletionUsesRenumberedLabel()
    {
        using WorktopViewModel vm = CreateViewModel();
        AddChain(vm, 3);
        var third = vm.Nodes.ElementAt(2);
        vm.SetMode(WorkspaceMode.Delete);
        vm.DeleteNode(vm.Nodes.ElementAt(1));
        vm.SetMode(WorkspaceMode.Add);

        vm.OnCanvasPressed(new ModelsPoint(200, 10));

        Assert.Equal("2", vm.Nodes.ElementAt(2).Label);
        Assert.Equal(2, vm.Connections.Count);
        Assert.Same(third, vm.Connections.ElementAt(1).Source);
    }

    private static void AddChain(WorktopViewModel vm, int count)
    {
        for (int i = 0; i < count; i++)
            vm.OnCanvasPressed(new ModelsPoint(10 + i * 50, 10));
    }

    private static double DistanceToCurve(ConnectionViewModel connection, PointD point)
    {
        double minimum = double.MaxValue;

        for (int i = 0; i <= 2000; i++)
        {
            PointD sample = BezierPoint(connection.Start, connection.Control1,
                                        connection.Control2, connection.End, i / 2000d);
            double dx = sample.X - point.X;
            double dy = sample.Y - point.Y;
            minimum = Math.Min(minimum, Math.Sqrt(dx * dx + dy * dy));
        }

        return minimum;
    }

    private static PointD BezierPoint(PointD p0, PointD p1, PointD p2, PointD p3, double t)
    {
        double u = 1d - t;
        double a = u * u * u;
        double b = 3d * u * u * t;
        double c = 3d * u * t * t;
        double d = t * t * t;

        return new PointD(
            p0.X * a + p1.X * b + p2.X * c + p3.X * d,
            p0.Y * a + p1.Y * b + p2.Y * c + p3.Y * d);
    }

    [Fact]
    public void PointDConverterConvertsBothDirections()
    {
        var converter = new PointDToPointConverter();

        object? point = converter.Convert(
            new PointD(3, 4), typeof(Avalonia.Point), null, CultureInfo.InvariantCulture);
        Assert.Equal(new Avalonia.Point(3, 4), point);

        object? back = converter.ConvertBack(
            new Avalonia.Point(3, 4), typeof(PointD), null, CultureInfo.InvariantCulture);
        Assert.Equal(new PointD(3, 4), back);
    }

    [Fact]
    public void ArrowOutlineExtendsTipAndExpandsBackward()
    {
        var from = new Avalonia.Point(100, 0);
        var to = new Avalonia.Point(110, 0);

        var main = ArrowGeometryConverter.TryBuildTriangle(from, to, 0d);
        var outline = ArrowGeometryConverter.TryBuildTriangle(from, to, 2d);

        Assert.NotNull(main);
        Assert.NotNull(outline);
        Assert.Equal(main.Value.Tip.X + 2d, outline.Value.Tip.X);
        Assert.Equal(main.Value.Tip.Y, outline.Value.Tip.Y);
        Assert.Equal(main.Value.Left.X - 2d, outline.Value.Left.X);
        Assert.True(outline.Value.Left.Y > main.Value.Left.Y);
        Assert.True(outline.Value.Right.Y < main.Value.Right.Y);
    }

    [Fact]
    public void ArrowBackConverterAcceptsNumericParameter()
    {
        Assert.Equal(2d, ArrowBackGeometryConverter.GetOutline("2"));
        Assert.Equal(1.5d, ArrowBackGeometryConverter.GetOutline(null));
    }
}
