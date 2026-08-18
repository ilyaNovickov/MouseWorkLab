using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using MouseLabAvalonia.Core.Interfaces;
using System;
using System.Linq;

namespace MouseLabAvalonia.Core
{
    public class WindowLocator : IWindowLocator
    {
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
