using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using MouseLabAvaloniaApp.ViewModels.Settings;
using MouseLabAvaloniaApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MouseLabAvaloniaApp.Services.WindowsManager
{
    public class WindowsManagerService : IWindowsManagerService
    {
        private readonly IServiceProvider _serviceProvider;

        private List<Window> openedWindows = new();

        public WindowsManagerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ShowSettingsAsync()
        {
            if (openedWindows.Any<Window>((win) => win is SettingsWindow))
            {
                return;
            }

            // 1. Создаём VM через DI (все зависимости подтянутся автоматически)
            var viewModel = _serviceProvider.GetRequiredService<SettingsWindowViewModel>();

            // 2. Создаём окно через DI
            var window = _serviceProvider.GetRequiredService<SettingsWindow>();
            window.DataContext = viewModel;

            // 3. Находим главное окно как owner
            //var owner = GetMainWindow();

            //if (owner is not null)
            //    await window.ShowDialog(owner);   // модальное
            //else

            

            openedWindows.Add(window);

            window.Closed += (sender, e) => { openedWindows.Remove(window); viewModel.Dispose(); };

            window.Show();                    // немодальное (fallback)

            await Task.CompletedTask;
        }

        private Window? GetMainWindow()
        {
            if (Application.Current?.ApplicationLifetime
                is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return desktop.MainWindow;
            }
            return null;
        }
    }
}
