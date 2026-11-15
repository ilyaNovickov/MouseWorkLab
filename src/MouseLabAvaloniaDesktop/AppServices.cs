using System;
using Microsoft.Extensions.DependencyInjection;
using MouseLabAvaloniaDesktop.Services.Abstractions;
using MouseLabAvaloniaDesktop.Services.Implementations;

namespace MouseLabAvaloniaDesktop;

public class AppServices : IDisposable, IServiceProvider
{
    private bool _disposed;

    private readonly ServiceProvider _serviceProvider;

    public AppServices()
    {
        _serviceProvider = GetServiceProvider();
    }

    private static ServiceProvider GetServiceProvider()
    {
        ServiceProvider provider = GetServicesCollection().BuildServiceProvider();

        // Обязательный шаг сразу после BuildServiceProvider: передаёт адаптеру те же
        // сервисы культуры и переводов, которыми пользуется приложение. Без него
        // статический binding source остался бы с пустым InMemory-провайдером
        // по умолчанию. Держим в одном методе с BuildServiceProvider, чтобы
        // порядок "сначала собрать, потом подключить адаптер" нельзя было нарушить.
        //provider.UseProTranslateAvalonia();

        return provider;
    }

    private static ServiceCollection GetServicesCollection()
    {
        ServiceCollection services = new();

        #region Services

        #region ProTranslate
        /*
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
            culture: CultureInfo.CurrentCulture,//initialCulture,
            cultureOptions: new CultureServiceOptions
            {
                ApplyToCurrentThread = true,
                ApplyToDefaultThread = true
            },
            translationOptions: new TranslationFallbackOptions
            {
                DefaultCulture = CultureInfo.CurrentCulture//CultureInfo.GetCultureInfo(AppSettingsStore.DefaultCultureName)
            },
            cacheOptions: new TranslationCacheOptions
            {
                MaximumEntries = 2048
            });
*/
        // Адаптер Avalonia: подключает TranslationBindingSource, через который работают
        // разметочные расширения и статический ProTranslate.Avalonia.TranslationService.
        //services.AddProTranslateAvalonia();
        #endregion

        /*
                services.AddSingleton<IDialogService>(sp => new DialogService(sp));

                // Путь к файлу не привязан: настройки — будущий артефакт «проекта»
                // пользователя, имя и каталог выберет проектный слой. До того — только
                // память, дефолтный снимок.
                services.AddSingleton<IApplicationSettingsService>(_ =>
                    new ApplicationSettingsService(new AppSettingsSnapshot
                    {
                        Theme = Themes.Light,

                        // Совпадает с культурой, которой ProTranslate стартует ниже, иначе
                        // ComboBox языка показывает не то, что применено, а первый же Save
                        // молча переключил бы UI на культуру по умолчанию.
                        Culture = CultureInfo.CurrentCulture.Name,
                        WorktopResolution = 8192
                    }));
                    */

        services.AddSingleton<IMatrixCutter, MatrixCutter>();
        services.AddSingleton<IMatrixRandomizer, MatrixRandomizer>();
        services.AddSingleton<IMoveFinderFactory, MoveFinderFactory>();
        services.AddSingleton<IMoveFinderManager, MoveFinderManager>();
        //services.AddSingleton<ModelingEnv, ModelingEnv>();
        #endregion

        #region ViewModels
        //services.AddTransient<WorktopCanvasViewModel>();
        //services.AddTransient<WorktopViewModel>();

        //services.AddTransient<ModelingSettingsViewModel>();
        //services.AddTransient<AppearanceSettingsViewModel>();
        //services.AddTransient<SettingsWindowViewModel>();

        //services.AddTransient<MainViewModel>();
        #endregion


        return services;
    }

    public static AppServices CreateDefault()
    {
        return new AppServices();
    }

    public object? GetService(Type serviceType)
    {
        return _serviceProvider.GetService(serviceType);
    }

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
