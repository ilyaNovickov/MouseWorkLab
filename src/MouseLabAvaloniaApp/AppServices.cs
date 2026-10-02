using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Microsoft.Extensions.DependencyInjection;
using MouseLabAvaloniaApp.Models;
using MouseLabAvaloniaApp.Services.AppSettings;
using MouseLabAvaloniaApp.Services.WindowsManager;
using MouseLabAvaloniaApp.ViewModels;
using MouseLabAvaloniaApp.ViewModels.Settings;
using MouseLabAvaloniaApp.Views;
using ProTranslate;
using ProTranslate.Avalonia;
using ProTranslate.Generated;
using System;
using System.Globalization;

namespace MouseLabAvaloniaApp
{
    public sealed class AppServices : IDisposable
    {
        private bool _disposed = false;

        // Контейнер DI держим в поле, а не в локальной переменной: если использовать
        // "using ServiceProvider", контейнер Dispose-ится в момент выхода из
        // OnFrameworkInitializationCompleted и все синглтоны (в т.ч. MainWindowViewModel)
        // окажутся уничтоженными, пока окно ещё живо.
        private ServiceProvider _serviceProvider;

        public AppServices(AppSettingsSnapshot snapshot, CultureInfo initialCulture)
        {
            _serviceProvider = BuildServiceProvider(snapshot, initialCulture);
        }

        public IServiceProvider Provider
        {
            get => _serviceProvider;
        }

        private static ServiceProvider BuildServiceProvider(AppSettingsSnapshot snapshot, CultureInfo initialCulture)
        {
            ServiceProvider provider = BuildServicesCollection(snapshot, initialCulture).BuildServiceProvider();

            // Обязательный шаг сразу после BuildServiceProvider: передаёт адаптеру те же
            // сервисы культуры и переводов, которыми пользуется приложение...
            provider.UseProTranslateAvalonia();

            return provider;
        }

        private static ServiceCollection BuildServicesCollection(AppSettingsSnapshot snapshot, CultureInfo initialCulture)
        {
            ServiceCollection services = new();

            // Настройки регистрируем готовым экземпляром, потому что они нужны ДО сборки
            // контейнера: стартовая культура определяется из файла и передаётся в AddProTranslate.
            services.AddSingleton<IApplicationSettingsService, ApplicationSettingsService>(sp => new ApplicationSettingsService(snapshot));

            // Ядро ProTranslate: культура, поиск переводов, fallback, форматирование, кэш.
            //
            // provider - сгенерированный провайдер. ProTranslate.SourceGenerator на этапе сборки
            //   компилирует содержимое Assets\Translations\Strings.*.json прямо в сборку, поэтому
            //   в рантайме нет ни чтения файлов, ни рефлексии по JSON. Без явного provider
            //   подставляется пустой InMemoryTranslationProvider и любой ключ возвращает сам себя.
            //
            // culture - культура из сохранённых настроек (или en-US по умолчанию).
            //
            // cultureOptions - ApplyToCurrentThread/ApplyToDefaultThread разрешают смене культуры
            //   менять CultureInfo текущего и будущих потоков. Без этого обычные биндинги Avalonia
            //   (числа, даты) продолжали бы форматироваться по инвариантной культуре.
            //
            // translationOptions - если ключ не найден в текущей культуре, поиск идёт по родительским
            //   культурам, а затем по DefaultCulture. Это страховка от "дырявого" перевода.
            //
            // cacheOptions - кэш результатов поиска очищается при смене культуры
            //   (ClearOnCultureChanged), MaximumEntries ограничивает его рост.
            services.AddProTranslate(
                provider: new ProTranslateGeneratedTranslationProvider("MouseLabCatalog"),
                culture: initialCulture,
                cultureOptions: new CultureServiceOptions
                {
                    ApplyToCurrentThread = true,
                    ApplyToDefaultThread = true
                },
                translationOptions: new TranslationFallbackOptions
                {
                    DefaultCulture = CultureInfo.GetCultureInfo(AppSettingsStore.DefaultCultureName)
                },
                cacheOptions: new TranslationCacheOptions
                {
                    MaximumEntries = 2048
                });

            // Адаптер Avalonia: подключает TranslationBindingSource, через который работают
            // разметочные расширения и статический ProTranslate.Avalonia.TranslationService.
            services.AddProTranslateAvalonia();

            #region ViewModels
            services.AddSingleton<IWindowsManagerService, WindowsManagerService>(sp => new WindowsManagerService(sp));

            services.AddTransient<AppSettingsViewModel>(sp => new AppSettingsViewModel(sp.GetRequiredService<ITranslationService>(), 
                sp.GetRequiredService<ICultureService>(), 
                sp.GetRequiredService<IApplicationSettingsService>()));

            services.AddTransient<SettingsWindowViewModel>(sp => new SettingsWindowViewModel(sp.GetRequiredService<ITranslationService>(),
                sp.GetRequiredService<AppSettingsViewModel>()));

            // MainWindowViewModel - синглтон, т.к. он же DataContext главного окна.
            services.AddSingleton<MainWindowViewModel>(sp => new MainWindowViewModel(sp.GetRequiredService<ITranslationService>(),
                sp.GetRequiredService<IWindowsManagerService>()));
            #endregion

            #region Views
            services.AddTransient<MainWindow>();
            services.AddTransient<SettingsWindow>();
            #endregion

            return services;
        }


        public void Dispose()
        {
            if (_disposed)
                return;

            _serviceProvider.Dispose();
            _disposed = true;
        }

        public static AppServices CreateDefault()
        {
            AppSettingsSnapshot snapshot = AppSettingsStore.Load();
            return new AppServices(snapshot, AppSettingsStore.ResolveCulture(snapshot.Culture));
        }

#if DEBUG
        private static AppServices? _instance;      

        public static AppServices Instance
        {
            get
            {
                return _instance ??= CreateDefault();
            }
        }
#endif
    }
}
