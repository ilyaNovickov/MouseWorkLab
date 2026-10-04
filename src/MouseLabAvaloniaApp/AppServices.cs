using Microsoft.Extensions.DependencyInjection;
using MouseLabAvaloniaApp.Services.AppSettings;
using MouseLabAvaloniaApp.Services.WindowsManager;
using MouseLabAvaloniaApp.ViewModels;
using MouseLabAvaloniaApp.ViewModels.Settings;
using MouseLabAvaloniaApp.ViewModels.Welcome;
using MouseLabAvaloniaApp.Views;
using ProTranslate;
using ProTranslate.Avalonia;
using ProTranslate.Generated;
using System;
using System.Globalization;

namespace MouseLabAvaloniaApp;

/// <summary>
/// Точка сборки графа объектов: читает настройки, регистрирует сервисы и отдаёт
/// контейнер. Вынесена из <see cref="App"/>, чтобы регистрация DI не мешала
/// оркестрации запуска и была доступна вьюхам в режиме дизайнера.
/// </summary>
public sealed class AppServices : IDisposable
{
    private bool _disposed;

    // Контейнер DI держим в поле, а не в локальной переменной: если использовать
    // "using ServiceProvider", контейнер Dispose-ится в момент выхода из
    // App.OnFrameworkInitializationCompleted и все синглтоны (в т.ч. MainWindowViewModel)
    // окажутся уничтоженными, пока окно ещё живо.
    private readonly ServiceProvider _serviceProvider;

    public AppServices(AppSettingsSnapshot snapshot, CultureInfo initialCulture)
    {
        _serviceProvider = BuildServiceProvider(snapshot, initialCulture);
    }

    public IServiceProvider Provider => _serviceProvider;

    private static ServiceProvider BuildServiceProvider(AppSettingsSnapshot snapshot, CultureInfo initialCulture)
    {
        ServiceProvider provider = BuildServicesCollection(snapshot, initialCulture).BuildServiceProvider();

        // Обязательный шаг сразу после BuildServiceProvider: передаёт адаптеру те же
        // сервисы культуры и переводов, которыми пользуется приложение. Без него
        // статический binding source остался бы с пустым InMemory-провайдером
        // по умолчанию. Держим в одном методе с BuildServiceProvider, чтобы
        // порядок "сначала собрать, потом подключить адаптер" нельзя было нарушить.
        provider.UseProTranslateAvalonia();

        return provider;
    }

    private static ServiceCollection BuildServicesCollection(AppSettingsSnapshot snapshot, CultureInfo initialCulture)
    {
        ServiceCollection services = new();

        #region Services

        #region ProTranslate
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
        #endregion

        services.AddKeyedSingleton<IApplicationSettingsService, TemporaryAppSettingsService>
            ("Temporary", (sp, _) => new TemporaryAppSettingsService());

        // Настройки регистрируем готовым экземпляром, потому что они нужны ДО сборки
        // контейнера: стартовая культура определяется из файла и передаётся в AddProTranslate.
        services.AddSingleton<IApplicationSettingsService, ApplicationSettingsService>(sp => new ApplicationSettingsService(snapshot));

        services.AddSingleton<IWindowsManagerService, WindowsManagerService>(sp => new WindowsManagerService(sp));

        #endregion

        #region ViewModels

        services.AddTransient<WelcomeWindowViewModel>(sp => new WelcomeWindowViewModel(sp.GetRequiredService<ITranslationService>(),
            sp.GetRequiredService<ICultureService>(),
            sp.GetKeyedService<IApplicationSettingsService>("Temporary") ?? sp.GetRequiredService<IApplicationSettingsService>()));

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
        services.AddTransient<WelcomeWindow>();
        services.AddTransient<MainWindow>();
        services.AddTransient<SettingsWindow>();
        #endregion

        return services;
    }

    /// <summary>
    /// Читает settings.json и собирает контейнер. Вынесено, чтобы порядок
    /// "прочитать настройки ДО сборки контейнера" был ровно в одном месте.
    /// </summary>
    public static AppServices CreateDefault()
    {
        AppSettingsSnapshot snapshot = AppSettingsStore.Load();
        return new AppServices(snapshot, AppSettingsStore.ResolveCulture(snapshot.Culture));
    }

    // Финализатора здесь намеренно нет. Единственный владелец - App, который
    // освобождает контейнер в Shutdown() по desktop.Exit. Финализатор добавил бы
    // Dispose на потоке финализации в произвольный момент - в том числе когда
    // контейнер ещё нужен, - а исключение оттуда завершило бы процесс без внятного
    // стека. Если desktop.Exit не сработает, контейнер утечёт, но процесс всё равно
    // завершается и память освобождает операционная система.
    public void Dispose()
    {
        if (_disposed)
            return;

        _serviceProvider.Dispose();
        _disposed = true;
    }

#if DEBUG
    // Контейнер дизайнера намеренно ОТДЕЛЬНЫЙ от рабочего: у него собственный
    // экземпляр IApplicationSettingsService, на который App не подписан, поэтому
    // смена темы в превью не доедет до App.ApplyTheme. Общий контейнер означал бы,
    // что превью влияет на живое приложение.
    //
    // Кэш обязателен: без него каждое обращение создавало бы новый контейнер,
    // перечитывало settings.json и переустанавливало статический binding source
    // ProTranslate (последнее обращение побеждало бы).
    private static AppServices? _instance;

    public static AppServices Instance => _instance ??= CreateDefault();
#endif
}