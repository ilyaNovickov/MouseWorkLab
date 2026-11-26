using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MouseLabAvaloniaDesktop.ViewModels.Worktop.NodesAndMore;

public partial class NodeViewModel : ViewModelBase
{
    public const int DefaultSize = 8;

    public NodeViewModel() : this(0, 0, "-1")
    {

    }

    public NodeViewModel(int x, int y) : this(x, y, "-1")
    {


    }

    public NodeViewModel(string label) : this(0, 0, label)
    {

    }

    public NodeViewModel(int x, int y, string label)
    {
        Id = Guid.NewGuid();
        Label = label;
        X = x;
        Y = y;
    }
    #region Properties
    public Guid Id { get; }

    [ObservableProperty]
    private int _x;

    [ObservableProperty]
    private int _y;

    [ObservableProperty]
    private string _label;

    [ObservableProperty]
    private int _width = DefaultSize;

    [ObservableProperty]
    private int _height = DefaultSize;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private bool _canInteract;

    [ObservableProperty]
    private bool _canDelete;
    #endregion


    #region PublicMethods
    public object ToModel()
    {
        return new object();
    }

    #endregion
}
