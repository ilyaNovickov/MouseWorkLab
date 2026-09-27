using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Microsoft.Extensions.DependencyInjection;
using MouseLabAvaloniaApp.Models;
using MouseLabAvaloniaApp.Services.AppSettings;
using MouseLabAvaloniaApp.ViewModels;
using MouseLabAvaloniaApp.Views;
using ProTranslate;
using ProTranslate.Avalonia;
using ProTranslate.Generated;
using System;
using System.Globalization;

namespace MouseLabAvaloniaApp;

public partial class App : Application
{
    // Контейнер DI держим в поле, а не в локальной переменной: если использовать
    // "using ServiceProvider", контейнер Dispose-ится в момент выхода из
    // OnFrameworkInitializationCompleted и все синглтоны (в т.ч. MainWindowViewModel)
    // окажутся уничтоженными, пока окно ещё живо.
    private ServiceProvider? _serviceProvider;

    // Обработчик храним в поле, чтобы можно было отписаться от него при выходе.
    private ThemeChangedEventHandler? _themeChangedHandler;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private static ServiceCollection GetServicesCollection(AppSettingsSnapshot snapshot, CultureInfo initialCulture)
    {
        ServiceCollection services = new();

        // Настройки регистрируем готовым экземпляром, потому что они нужны ДО сборки
        // контейнера: стартовая культура определяется из файла и передаётся в AddProTranslate.
        services.AddSingleton<IApplicationSettingsService>(new ApplicationSettingsService(snapshot));

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

        // MainWindowViewModel - синглтон, т.к. он же DataContext главного окна.
        services.AddSingleton<MainWindowViewModel>();

        return services;
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Читаем настройки до сборки контейнера: культура обязана быть известна заранее.
        AppSettingsSnapshot snapshot = AppSettingsStore.Load();
        CultureInfo initialCulture = AppSettingsStore.ResolveCulture(snapshot.Culture);

        _serviceProvider = GetServicesCollection(snapshot, initialCulture).BuildServiceProvider();

        // Обязательный шаг после BuildServiceProvider: передаёт адаптеру те же сервисы
        // культуры и переводов, которыми пользуется приложение. Без него статический
        // binding source остался бы с пустым InMemory-провайдером по умолчанию.
        _serviceProvider.UseProTranslateAvalonia();

        IApplicationSettingsService settings = _serviceProvider.GetRequiredService<IApplicationSettingsService>();

        // Тема применяется и сразу при старте, и на каждое изменение настроек.
        _themeChangedHandler = (_, e) => ApplyTheme(e.NewTheme);
        settings.ThemeChanged += _themeChangedHandler;
        ApplyTheme(settings.CurrentAppTheme);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = _serviceProvider.GetRequiredService<MainWindowViewModel>(),
            };

            desktop.Exit += (_, _) => Shutdown();
        }

        base.OnFrameworkInitializationCompleted();
    }

    // Themes.Default - это "следовать системной теме" (ThemeVariant.Default).
    private void ApplyTheme(Themes theme) =>
        RequestedThemeVariant = theme switch
        {
            Themes.Light => ThemeVariant.Light,
            Themes.Dark => ThemeVariant.Dark,
            _ => ThemeVariant.Default
        };

    private void Shutdown()
    {
        if (_serviceProvider is null)
            return;

        // Отписываемся от события перед Dispose, иначе обработчик остался бы висеть
        // на освобождаемом объекте.
        if (_themeChangedHandler is not null &&
            _serviceProvider.GetService<IApplicationSettingsService>() is { } settings)
        {
            settings.ThemeChanged -= _themeChangedHandler;
        }

        _themeChangedHandler = null;
        _serviceProvider.Dispose();
        _serviceProvider = null;
    }
}
