using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using MouseLabAvaloniaDesktop.ViewModels.Worktop.NodesAndMore;

namespace MouseLabAvaloniaDesktop.Views;

public partial class ConnectionView : UserControl
{
    public ConnectionView()
    {
        InitializeComponent();
#if DEBUG

        if (Design.IsDesignMode)
        {
            //var vm = AppServices.Instance.GetRequiredService<ConnectionViewModel>();
            var s = new NodeViewModel(100, 100);
            var t = new NodeViewModel(200, 200);
            var vm = new ConnectionViewModel(s, t);
            vm.Kind = ConnectionKinds.Curved;
            vm.IsSelected = true;
            vm.Control1 = new PointD(200, 100);
            vm.Control1 = new PointD(300, 200);
            if (vm is not null)
                Design.SetDataContext(this, vm);
        }
#endif
    }
}