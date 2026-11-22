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
using Vector = Avalonia.Vector;

namespace MouseLabAvaloniaDesktop.ViewModels.Worktop
{
    public partial class WorktopViewModel : TranslatedViewModelBase
    {
        private readonly ModelingEnv _modelingEnv;
        private readonly ModelingSettings _modelingSettings;

        public WorktopViewModel(
            ITranslationService translation,
            //WorktopCanvasViewModel worktopCanvasViewModel,
            ModelingEnv modelingEnv,
            ModelingSettings modelingSettings
            ) : base(translation)
        {
            //WorktopCanvasViewModel = worktopCanvasViewModel;
            _modelingEnv = modelingEnv;
            _modelingSettings = modelingSettings;

            //RelayCommand<int, int> relayCommand = new RelayCommand<int, int>(AddNode);

            Nodes.CollectionChanged += Nodes_CollectionChanged;

            _modelingSettings.PropertyChanged += _modelingSettings_PropertyChanged;

            OnWorktopChanged();
        }

        private void _modelingSettings_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            IsNeedGeneration = _modelingEnv.Worktop.Width == _modelingEnv.Worktop.Height
                && _modelingEnv.Worktop.Width == _modelingSettings.WorktopResolution;
        }

        //public WorktopCanvasViewModel WorktopCanvasViewModel { get; }

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

        private void OnSeedChanged()
        {
            IsSeedNumber = Seed switch
            {
                null => true,
                not null => int.TryParse(Seed, out _)
            };
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

        [ObservableProperty]
        private bool isNeedGeneration;

        [RelayCommand]
        private async Task GenerateWorktop()
        {
            await Task.Run(() =>
            {
                _modelingEnv.GenerateNewWorktop(_modelingSettings.WorktopResolution, GetSeedFromString(Seed));
                //Thread.Sleep(2000);
            });


            //OnPropertyChanged(nameof(Worktop));
            OnWorktopChanged();
        }

        private const double Marging = 0.10d;

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

        // использовать double ?
        [ObservableProperty]
        public partial int WorktopPosition
        {
            get;
            private set;
        }

        //private Matrix matrix = new(500, 500);
        public IReadOnlyMatrix Worktop
        {
            //get => matrix;
            get => _modelingEnv.Worktop;
        }

        private void OnWorktopChanged()
        {
            IsNeedGeneration = false;

            int diff = (int)(_modelingEnv.Worktop.Size.Width * Marging);

            AvailableResolution = _modelingEnv.Worktop.Width + (2 * diff);
            WorktopPosition = diff;

            //AvailableResolution = (int)(_modelingEnv.Worktop.Size.Width * (1.00d + 2d * Marging));
            //WorktopPosition = (AvailableResolution - _modelingEnv.Worktop.Width) / 2;

            OnPropertyChanged(nameof(Worktop));
        }

        public const int MaxNodes = 1024;

        public ObservableCollection<NodeViewModel> Nodes { get; } = new();

        private void Nodes_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
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

        [ObservableProperty]
        private WorkspaceMode workspaceMode = WorkspaceMode.Add;

        [ObservableProperty]
        private NodeViewModel? selectedNode;

        [RelayCommand]
        public void SetMode(WorkspaceMode mode)
        {
            WorkspaceMode = mode;

            bool canInteract = mode == WorkspaceMode.SelectAndMove;
            foreach (NodeViewModel node in Nodes)
                node.CanInteract = canInteract;

            if (!canInteract)
                ClearSelection();
        }

        public void OnCanvasPressed(Point point)
        {
            if (WorkspaceMode == WorkspaceMode.Add)
                AddNode(point);
            else
                ClearSelection();
        }

        public void SelectNode(NodeViewModel node)
        {
            if (WorkspaceMode != WorkspaceMode.SelectAndMove || SelectedNode == node)
                return;

            if (SelectedNode is not null)
                SelectedNode.IsSelected = false;

            SelectedNode = node;
            node.IsSelected = true;
        }

        public void ClearSelection()
        {
            if (SelectedNode is null)
                return;

            SelectedNode.IsSelected = false;
            SelectedNode = null;
        }

        public void MoveNode(NodeViewModel node, Vector delta)
        {
            if (WorkspaceMode != WorkspaceMode.SelectAndMove)
                return;

            node.X = (int)Math.Round(node.X + delta.X);
            node.Y = (int)Math.Round(node.Y + delta.Y);
        }

        [RelayCommand]
        private void AddNode(Point point)
        {
            if (Nodes.Count >= MaxNodes)
                return;

            NodeViewModel nodeViewModel = new(point.X - (NodeViewModel.DefaultSize / 2) + 1,
                        point.Y - (NodeViewModel.DefaultSize / 2) + 1,
                        Nodes.Count.ToString())
            {
                CanInteract = WorkspaceMode == WorkspaceMode.SelectAndMove
            };

            Nodes.Add(nodeViewModel);
        }
    }
}
