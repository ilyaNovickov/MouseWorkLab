using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MouseLabAvaloniaDesktop.Models;
using MouseLabAvaloniaDesktop.Models.Settings;
using MouseLabAvaloniaDesktop.Services.Implementations;
using ProTranslate;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MouseLabAvaloniaDesktop.ViewModels.Worktop.NodesAndMore;
using System.ComponentModel;
using System.Collections.Specialized;
using Avalonia.Controls.PanAndZoom;
using Vector = Avalonia.Vector;
using System.Linq;

namespace MouseLabAvaloniaDesktop.ViewModels.Worktop
{
    public partial class WorktopViewModel : TranslatedViewModelBase
    {
        public const int MaxNodes = 1024;
        private const double Marging = 0.10d;

        private readonly ModelingEnv _modelingEnv;
        private readonly ModelingSettings _modelingSettings;
        private readonly List<ConnectionKindOption> connectionKindOptions;

        public WorktopViewModel(
            ITranslationService translation,
            ModelingEnv modelingEnv,
            ModelingSettings modelingSettings
            ) : base(translation)
        {;
            _modelingEnv = modelingEnv;
            _modelingSettings = modelingSettings;

            connectionKindOptions =
            [
                new ConnectionKindOption(Strings.Observe_WorktopConnectionKindStraight(), ConnectionKinds.Straight),
                new ConnectionKindOption(Strings.Observe_WorktopConnectionKindCurved(), ConnectionKinds.Curved)
            ];

            _nodes.CollectionChanged += Nodes_CollectionChanged;

            _modelingSettings.PropertyChanged += _modelingSettings_PropertyChanged;

            OnWorktopChanged();
        }

        #region Properties
        public string? Seed
        {
            get;
            set
            {
                if (SetProperty(ref field, value))
                {
                    OnSeedChanged();
                }

            }
        }

        [ObservableProperty]
        private bool isSeedNumber;

        [ObservableProperty]
        private bool isNeedGeneration;

        // использовать double ?
        [ObservableProperty]
        public partial int WorktopPosition
        {
            get;
            private set;
        }

        public IReadOnlyMatrix Worktop
        {
            get => _modelingEnv.Worktop;
        }

        public int AvailableResolution
        {
            get;
            private set
            {
                if (SetProperty(ref field, value))
                {
                    //WorktopCanvasViewModel.SetCanvasDiff(field, _modelingEnv.Worktop.Size.Width);
                }
            }
        }

        [ObservableProperty]
        private WorkspaceMode workspaceMode = WorkspaceMode.Add;

        [ObservableProperty]
        private NodeViewModel? selectedNode;

        [ObservableProperty]
        private ConnectionViewModel? selectedConnection;

        [ObservableProperty]
        private ButtonName panButton = ButtonName.Middle;

        public IReadOnlyList<ConnectionKindOption> ConnectionKindOptions => connectionKindOptions;

        public bool HasSelectedConnection => SelectedConnection is not null;

        public ConnectionKindOption? SelectedConnectionKindOption
        {
            get => SelectedConnection is null
                ? null
                : connectionKindOptions.FirstOrDefault(option => option.Kind == SelectedConnection.Kind);
            set
            {
                if (value is null || SelectedConnection is null || SelectedConnection.Kind == value.Kind)
                    return;

                SelectedConnection.Kind = value.Kind;
                OnPropertyChanged();
            }
        }
        #endregion

        #region OnPropertiesChanged
        private void OnSeedChanged()
        {
            IsSeedNumber = Seed switch
            {
                null => true,
                not null => int.TryParse(Seed, out _)
            };
        }

        public event EventHandler? WorktopChanged;

        private void OnWorktopChanged()
        {
            IsNeedGeneration = false;

            int diff = (int)(_modelingEnv.Worktop.Size.Width * Marging);

            AvailableResolution = _modelingEnv.Worktop.Width + (2 * diff);
            WorktopPosition = diff;

            OnPropertyChanged(nameof(Worktop));
            WorktopChanged?.Invoke(this, EventArgs.Empty);
        }

        partial void OnSelectedConnectionChanged(ConnectionViewModel? value)
        {
            OnPropertyChanged(nameof(SelectedConnectionKindOption));
            OnPropertyChanged(nameof(HasSelectedConnection));
        }
        #endregion

        #region Commands
        [RelayCommand]
        private async Task GenerateWorktop()
        {
            await Task.Run(() =>
            {
                _modelingEnv.GenerateNewWorktop(
                    _modelingSettings.WorktopResolution, 
                    GetSeedFromString(Seed)
                    );
            });

            OnWorktopChanged();
        }

        private static int? GetSeedFromString(string? seed)
        {
            if (seed is null)
                return null;

            if (int.TryParse(seed, out int result))
                return result;
            else
                return null;
        }

        [RelayCommand]
        public void SetMode(WorkspaceMode mode)
        {
            WorkspaceMode = mode;
            PanButton = mode == WorkspaceMode.PanOnly ? ButtonName.Left : ButtonName.Middle;

            bool canInteract = mode == WorkspaceMode.SelectAndMove;
            bool canDelete = mode == WorkspaceMode.Delete;

            foreach (NodeViewModel node in Nodes)
            {
                node.CanInteract = canInteract;
                node.CanDelete = canDelete;
            }

            foreach (ConnectionViewModel connection in Connections)
            {
                connection.CanInteract = canInteract;
                connection.CanDelete = canDelete;
            }

            if (!canInteract)
                ClearSelection();
        }
        #endregion

        #region EventHandlers
        private void _modelingSettings_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            IsNeedGeneration = _modelingEnv.Worktop.Width == _modelingEnv.Worktop.Height
                && _modelingEnv.Worktop.Width == _modelingSettings.WorktopResolution;
        }


        #endregion

        #region Nodes       

        private readonly ObservableCollection<NodeViewModel> _nodes = new();

        public IReadOnlyCollection<NodeViewModel> Nodes => _nodes;

        [RelayCommand]
        private void AddNode(Point point)
        {
            if (Nodes.Count >= MaxNodes)
                return;

            NodeViewModel nodeViewModel = new(point.X - (NodeViewModel.DefaultSize / 2) + 1,
                        point.Y - (NodeViewModel.DefaultSize / 2) + 1,
                        Nodes.Count.ToString())
            {
                CanInteract = WorkspaceMode == WorkspaceMode.SelectAndMove,
                CanDelete = WorkspaceMode == WorkspaceMode.Delete
            };

            _nodes.Add(nodeViewModel);
        }

        public void DeleteNode(NodeViewModel node)
        {
            if (WorkspaceMode != WorkspaceMode.Delete || !Nodes.Contains(node))
                return;

            ConnectionViewModel? incoming = Connections.FirstOrDefault(c => c.Target == node);
            ConnectionViewModel? outgoing = Connections.FirstOrDefault(c => c.Source == node);

            if (SelectedNode == node
                || (incoming is not null && SelectedConnection == incoming)
                || (outgoing is not null && SelectedConnection == outgoing))
            {
                ClearSelection();
            }

            RemoveConnection(incoming);
            RemoveConnection(outgoing);

            _nodes.Remove(node);

            if (incoming is not null && outgoing is not null)
                AddConnection(incoming.Source, outgoing.Target);

            for (int i = 0; i < _nodes.Count; i++)
                _nodes[i].Label = i.ToString();
        }

        public void MoveNodeFromOrigin(NodeViewModel node, int originX, int originY, Vector offset)
        {
            if (WorkspaceMode != WorkspaceMode.SelectAndMove)
                return;

            node.X = originX + (int)Math.Round(offset.X);
            node.Y = originY + (int)Math.Round(offset.Y);
        }

        public void SelectNode(NodeViewModel node)
        {
            if (WorkspaceMode != WorkspaceMode.SelectAndMove || SelectedNode == node)
                return;

            ClearConnectionSelection();

            if (SelectedNode is not null)
                SelectedNode.IsSelected = false;

            SelectedNode = node;
            node.IsSelected = true;
        }

        private void Nodes_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems is not null && e.NewItems.Count == 1)
                    {
                        if (e.NewItems[0] is NodeViewModel nodeVM && _nodes.Count != 1)
                        {
                            AddConnection(_nodes[^2], nodeVM);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
            }
        }

        #endregion


        #region Connectionds
        private readonly ObservableCollection<ConnectionViewModel> _connections = new();

        public IReadOnlyCollection<ConnectionViewModel> Connections => _connections;

        private void RemoveConnection(ConnectionViewModel? connection)
        {
            if (connection is null)
                return;

            _connections.Remove(connection);
            connection.Dispose();
        }

        private void AddConnection(NodeViewModel source, NodeViewModel target)
        {
            var conn = new ConnectionViewModel(source, target)
            {
                CanInteract = WorkspaceMode == WorkspaceMode.SelectAndMove,
                CanDelete = WorkspaceMode == WorkspaceMode.Delete
            };
            _connections.Add(conn);
        }

        public void SelectConnection(ConnectionViewModel connection)
        {
            if (WorkspaceMode != WorkspaceMode.SelectAndMove || SelectedConnection == connection)
                return;

            ClearSelection();

            SelectedConnection = connection;
            connection.IsSelected = true;
        }

        public void SetConnectionControlPoint(ConnectionViewModel connection, int index, PointD point)
        {
            if (WorkspaceMode != WorkspaceMode.SelectAndMove
                || SelectedConnection != connection
                || connection.Kind != ConnectionKinds.Curved)
                return;

            connection.SetControlPoint(index, point);
        }

        private void ClearConnectionSelection()
        {
            if (SelectedConnection is null)
                return;

            SelectedConnection.IsSelected = false;
            SelectedConnection = null;
        }

        public void ClearSelection()
        {
            ClearConnectionSelection();

            if (SelectedNode is null)
                return;

            SelectedNode.IsSelected = false;
            SelectedNode = null;
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (ConnectionKindOption option in connectionKindOptions)
                    option.Dispose();
            }

            base.Dispose(disposing);
        }

        public void OnCanvasPressed(Point point)
        {
            if (WorkspaceMode == WorkspaceMode.Add)
                AddNode(point);
            else
                ClearSelection();
        }
    }
}
