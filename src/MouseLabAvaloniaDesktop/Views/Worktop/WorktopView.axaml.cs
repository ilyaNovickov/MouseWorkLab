using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using MouseLabAvaloniaDesktop.ViewModels;
using MouseLabAvaloniaDesktop.ViewModels.Worktop;

namespace MouseLabAvaloniaDesktop.Views
{
    public partial class WorktopView : UserControl
    {
        public WorktopView()
        {
            InitializeComponent();
#if DEBUG
            if (Design.IsDesignMode)
            {
                var vm = AppServices.Instance.GetRequiredService<WorktopViewModel>();
                if (vm is not null)
                    Design.SetDataContext(this, vm);
            }
#endif
        }
    }
}