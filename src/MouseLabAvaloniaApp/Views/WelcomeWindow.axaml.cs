using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using MouseLabAvaloniaApp.ViewModels.Settings;
using MouseLabAvaloniaApp.ViewModels.Welcome;

namespace MouseLabAvaloniaApp.Views
{
    public partial class WelcomeWindow : Window
    {
        public WelcomeWindow()
        {
            InitializeComponent();

#if DEBUG
            if (Design.IsDesignMode)
            {
                Design.SetDataContext(this, AppServices.Instance.Provider.GetRequiredService<WelcomeWindowViewModel>());
            }
#endif
        }
    }
}