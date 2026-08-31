using MouseLabAvalonia.Core;
using MouseLabAvalonia.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvalonia.ViewModels
{
    public class ViewModelFactory
    {
        public ViewModelFactory()
        {
            this.WindowsLocator = new WindowLocator();
            this.MessageDialogService = new MessegeDialogService(WindowsLocator);
            this.FileDialogService = new FileDialogService(WindowsLocator);
            this.PlotViewModel = new PlotViewModel(MessageDialogService, FileDialogService);
        }

        public IWindowLocator WindowsLocator { get; private set; }

        public IFileDialogService FileDialogService { get; private set; }

        public IMessageDialogService MessageDialogService { get; private set; }

        public PlotViewModel PlotViewModel { get; private set; }
    }
}
