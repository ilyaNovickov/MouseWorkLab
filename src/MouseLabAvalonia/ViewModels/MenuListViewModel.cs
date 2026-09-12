using CommunityToolkit.Mvvm.Input;
using MouseLabAvalonia.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvalonia.ViewModels
{
    /// <summary>
    /// ViewModel для меню с целью открытия окон
    /// </summary>
    public partial class MenuListViewModel : ViewModelBase
    {
        private readonly IDockFactory factory;

        /// <summary>
        /// Инициализация 
        /// </summary>
        /// <param name="name">Текст, выводимый на интерфейс</param>
        /// <param name="vmName">Имя ViewModel, по которому происходит его открытие на макете</param>
        /// <param name="factory">Фабрика Dockable элементов</param>
        public MenuListViewModel(string name, string vmName, IDockFactory factory)
        {
            Name = name;
            ViewModelName = vmName;
            this.factory = factory;
        }

        /// <summary>
        /// Имя окна (выводится на интерфейс)
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Имя ViewModel, по которому элемент добавляется на Dock макет
        /// </summary>
        private string ViewModelName { get; }

        /// <summary>
        /// Добавление Dockable элемента на интерфейс
        /// </summary>
        [RelayCommand]
        private void AddDockable()
        {
            factory.AddDockable(this.ViewModelName);
        }
    }
}
