using CommunityToolkit.Mvvm.Input;
using MouseLabAvalonia.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvalonia.ViewModels
{
    public partial class MenuListViewModel : ViewModelBase
    {
        private readonly IDockFactory factory;
        public MenuListViewModel(string name, string vmName, IDockFactory factory)
        {
            Name = name;
            ViewModelName = vmName;
            this.factory = factory;
        }
        public string Name { get; }

        private string ViewModelName { get; }

        [RelayCommand]
        private void AddDockable()
        {
            factory.AddDockable(this.ViewModelName);
        }
    }
}
