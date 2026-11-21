using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using MouseLabAvaloniaDesktop.ViewModels.Worktop.NodesAndMore;

namespace MouseLabAvaloniaDesktop.Views;

public partial class NodeView : UserControl
{
    public NodeView()
    {
        InitializeComponent();
#if DEBUG
        if (Design.IsDesignMode)
        {
            var vm = AppServices.Instance.GetRequiredService<NodeViewModel>();
            if (vm is not null)
                Design.SetDataContext(this, vm);
        }
#endif
    }
}