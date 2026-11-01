using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using MouseLabAvaloniaApp.ViewModels;

namespace MouseLabAvaloniaApp.Views;

public partial class WorktopView : UserControl
{
    public WorktopView()
    {
        InitializeComponent();

#if DEBUG
        /*
                if (Design.IsDesignMode)
                {
                    Design.SetDataContext(this, AppServices.Instance.Provider.GetRequiredService<WorktopViewModel>());
                }
                */
#endif
    }
}