using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvalonia.Core.Interfaces
{
    /// <summary>
    /// Локатор, позволяющий получить активное или главное окно приложения
    /// </summary>
    public interface IWindowLocator
    {
        /// <summary>
        /// Возвращает активное окно приложения, либо главное окно, если активного нет
        /// </summary>
        /// <returns>Активное или главное окно приложения</returns>
        /// <exception cref="InvalidOperationException">Приложение не запущено в desktop lifetime или не найдено ни одно окно</exception>
        Window GetRequiredWindow();
    }
}
