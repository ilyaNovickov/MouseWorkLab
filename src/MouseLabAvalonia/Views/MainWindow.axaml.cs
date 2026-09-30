using Avalonia.Controls;
using MouseLabAvalonia.ViewModels;
using System;
using System.Threading.Tasks;

namespace MouseLabAvalonia.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
#if DEBUG
            // Prevent the previewer's DataContext from being set when the application is run.
            if (Design.IsDesignMode)
            {
                // This can be before or after InitializeComponent.
                Design.SetDataContext(this, MainWindowViewModel.Instance);
            }
#endif
            InitializeComponent();
        }
        protected override void OnDataContextChanged(EventArgs e)
        {
            if (this.DataContext is MainWindowViewModel vm)
            {
                this.Closing += (_, args) => Closing_MainWindow(args, vm);

            }
            base.OnDataContextChanged(e);
        }

        #region Cancaling
        private bool closing = false;
        private bool confirmPending = false;

        

        private void Closing_MainWindow(WindowClosingEventArgs e, MainWindowViewModel vm)
        {
            if (confirmPending)
            {
                e.Cancel = true;
                return;
            }
            if (closing)
            {
                vm.CloseLayout();
                return;
            }
            confirmPending = true;
            e.Cancel = true;

            _ = ConfirmCancel(vm);
        }

        private async Task ConfirmCancel(MainWindowViewModel vm)
        {
            bool confirm = await vm.ConfirmCloseAsync();

            confirmPending = false;

            if (confirm)
            {
                closing = true;
                Close();
            }
           
        }
        #endregion

        private void foo()
        {

        }
    }
}