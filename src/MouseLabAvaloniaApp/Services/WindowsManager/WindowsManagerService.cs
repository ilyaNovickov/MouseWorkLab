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

        // Метод намеренно НЕ async: пока модальный путь закомментирован, ждать
        // тут нечего, а async без await даёт CS1998. При возврате ShowDialog
        // (шаг 3) async придётся вернуть - и перестроить порядок шагов, см. там.
        public Task ShowSettingsAsync()
        {
            // Проверяем ДО создания VM и окна. Иначе второй клик по меню создал бы
            // ещё одну SettingsWindowViewModel с подпиской на CultureChanged,
            // которую никто не отпишет.
            if (openedWindows.Any(win => win is SettingsWindow))
            {
                return Task.CompletedTask;
            }

            // 1. Создаём VM через DI (все зависимости подтянутся автоматически)
            var viewModel = _serviceProvider.GetRequiredService<SettingsWindowViewModel>();

            // 2. Создаём окно через DI
            var window = _serviceProvider.GetRequiredService<SettingsWindow>();
            window.DataContext = viewModel;

            // 3. Находим главное окно как owner
            //var owner = GetMainWindow();

            // ВНИМАНИЕ при включении модального режима: вернуть await
            // window.ShowDialog(owner), перенести после него openedWindows.Add,
            // подписку на Closed и Show(), и оставить проверку в начале метода -
            // иначе после закрытия диалога сработает Closed (снимет окно из
            // списка и освободит VM), а затем Add вернёт мёртвое окно в список и
            // Show() покажет его второй раз.
            //if (owner is not null)
            //    await window.ShowDialog(owner);   // модальное
            //else

            openedWindows.Add(window);

            window.Closed += (sender, e) =>
            {
                openedWindows.Remove(window);

                // Модель представления переживает окно, поэтому освобождаем её здесь.
                // Повторное освобождение при выходе из приложения безопасно:
                // ViewModelBase.Dispose выставляет _disposed.
                viewModel.Dispose();
            };

            window.Show();                    // немодальное (fallback)

            return Task.CompletedTask;
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
