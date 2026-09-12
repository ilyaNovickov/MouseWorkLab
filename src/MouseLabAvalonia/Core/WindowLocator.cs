using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using MouseLabAvalonia.Core.Interfaces;
using System;
using System.Linq;

namespace MouseLabAvalonia.Core
{
    /// <summary>
    /// Локатор, позволяющий получить активное или главное окно приложения
    /// </summary>
    public class WindowLocator : IWindowLocator
    {
        /// <summary>
        /// Возвращает активное окно приложения, либо главное окно, если активного нет
        /// </summary>
        /// <returns>Активное или главное окно приложения</returns>
        /// <exception cref="InvalidOperationException">Приложение не запущено в desktop lifetime или не найдено ни одно окно</exception>
        public Window GetRequiredWindow()
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
                throw new InvalidOperationException("Приложение не работает в desktop lifetime.");

            return desktop.Windows.FirstOrDefault(w => w.IsActive)
                   ?? desktop.MainWindow
                   ?? throw new InvalidOperationException("Не найдено активное окно.");
        }
    }
}
