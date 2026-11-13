using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using MouseLabAvaloniaDesktop.ViewModels;

namespace MouseLabAvaloniaDesktop.Views;

public partial class ModelingSettingsView : UserControl
{
    public ModelingSettingsView()
    {
        InitializeComponent();
#if DEBUG
        if (Design.IsDesignMode)
        {
            var vm = AppServices.Instance.GetRequiredService<ModelingSettingsViewModel>();
            if (vm is not null)
                Design.SetDataContext(this, vm);

            //Design.SetDesignStyle(this, App.Current!.Styles[3]);
        }
#endif
    }
}