using Dock.Model.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvalonia.Core.Interfaces
{
    /// <summary>
    /// Фабрика создания Dockable компонентов
    /// </summary>
    public interface IDockFactory : IFactory
    {
        /// <summary>
        /// Добавить Dockable-элемент по его имени
        /// </summary>
        /// <param name="vmName">Имя ViewModel элемента</param>
        void AddDockable(string vmName);
    }
}
