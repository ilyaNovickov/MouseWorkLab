using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using MouseLabAvalonia.Core;
using MouseLabAvalonia.Core.Interfaces;
using MouseLabAvalonia.ViewModels;
using MouseLabAvalonia.Views;
using System;
using System.Linq;

namespace MouseLabAvalonia
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

#if DEBUG
            this.AttachDeveloperTools();
#endif
        }
        private void CreateServices()
        {
            // 1. Создаём коллекцию для регистрации
            var services = new ServiceCollection();

            // 2. Регистрируем сервисы
            services.AddSingleton<IWindowLocator, WindowLocator>();
            services.AddSingleton<IMessageDialogService, MessegeDialogService>();
            services.AddTransient<MainWindowViewModel>();

            // 3. Строим провайдер
            Services = services.BuildServiceProvider();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            CreateServices();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = Services.GetRequiredService<MainWindowViewModel>(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}