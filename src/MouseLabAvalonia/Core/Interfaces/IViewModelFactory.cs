using MouseLabAvalonia.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvalonia.Core.Interfaces
{
    public interface IViewModelFactory
    {
        public IWindowLocator WindowsLocator { get; }

        public IFileDialogService FileDialogService { get; }

        public IMessageDialogService MessageDialogService { get; }

        public PlotViewModel PlotViewModel { get; }
    }
}
