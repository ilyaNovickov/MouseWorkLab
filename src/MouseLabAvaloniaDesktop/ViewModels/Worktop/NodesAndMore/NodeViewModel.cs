using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MouseLabAvaloniaDesktop.ViewModels.Worktop.NodesAndMore;

public partial class NodeViewModel : ViewModelBase
{
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

    public Guid Id { get; }

    [ObservableProperty]
    private int _x;

    [ObservableProperty]
    private int _y;

    [ObservableProperty]
    private string _label;

    [ObservableProperty]
    private int _width;

    [ObservableProperty]
    private int _height;

    [ObservableProperty]
    private bool _isSelected;



    public object ToModel()
    {
        return new object();
    }
}
