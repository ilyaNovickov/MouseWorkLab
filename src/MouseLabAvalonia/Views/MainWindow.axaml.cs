using Avalonia.Controls;
using MouseLabAvalonia.ViewModels;
using System;

namespace MouseLabAvalonia.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Prevent the previewer's DataContext from being set when the application is run.
            if (Design.IsDesignMode)
            {
                // This can be before or after InitializeComponent.
                Design.SetDataContext(this, MainWindowViewModel.Instance);
            }
            InitializeComponent();
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            if (this.DataContext is MainWindowViewModel vm)
            {
                this.Closing += (_, _) => vm.CloseLayout();
            }
            base.OnDataContextChanged(e);
        }
    }
}