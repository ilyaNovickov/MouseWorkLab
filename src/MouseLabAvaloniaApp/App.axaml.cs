using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Microsoft.Extensions.DependencyInjection;
using MouseLabAvaloniaApp.Models;
using MouseLabAvaloniaApp.Services.AppSettings;
using MouseLabAvaloniaApp.ViewModels;
using MouseLabAvaloniaApp.ViewModels.Welcome;
using MouseLabAvaloniaApp.Views;
using ProTranslate;
using System;

namespace MouseLabAvaloniaApp;

public partial class App : Application
{
    // Контейнер в поле, а не в локальной переменной: см. AppServices.
    // Освобождается в Shutdown() по desktop.Exit.
    private AppServices? _appServices;

    private ViewLocator? viewLocator;

    // Обработчик храним в поле, чтобы можно было отписаться от него при выходе.
    private ThemeChangedEventHandler? _themeChangedHandler;

    

    // Пользователь подтвердил данные. Различать подтверждение и закрытие
    // окна важно: подтверждение ведёт к главному окну, закрытие крестиком или
    // кнопкой выхода - к завершению приложения.
    private bool _welcomeConfirmed;

    // Защита от двойного перехода: кнопка подтверждения закрывает окно, а
    // закрытие окна само вызывает переход. Без флага окно показалось бы дважды.
    private bool _mainWindowStarted;

    // Контейнер гарантированно создан в OnFrameworkInitializationCompleted до
    // вызова ShowWelcome, но компилятор не может это доказать через границы
    // методов - поэтому доступ идёт через это свойство с явной проверкой.
    private AppServices Services =>
        _appServices ?? throw new InvalidOperationException("AppServices ещё не создан: OnFrameworkInitializationCompleted не был вызван.");

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this); 
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Настройки читаются ДО сборки контейнера - этим занимается
        // AppServices.CreateDefault(). Постоянные настройки здесь ещё не
        // применяются: пользователь на этом шаге неизвестен, его выбор будет
        // разобран в окне приветствия.
        _appServices = AppServices.CreateDefault();

        viewLocator = new ViewLocator(_appServices.Provider);
        DataTemplates.Add(viewLocator);
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Пока открыто окно приветствия, закрытие окна не должно завершать
            // приложение: иначе закрытие крестиком убило бы приложение, а
            // закрытие после подтверждения - убило бы раньше времени.
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            desktop.Exit += (_, _) => Shutdown();

            ShowWelcome(desktop);
        }

        base.OnFrameworkInitializationCompleted();
    }

#region ForWelcome
    // Обработчик темы временных настроек. Нужен, чтобы окно приветствия
    // показывало выбранную тему сразу, и снимается при переходе к главному окну.
    private ThemeChangedEventHandler? _welcomeThemeChangedHandler;

    private void ShowWelcome(IClassicDesktopStyleApplicationLifetime desktop)
    {
        IApplicationSettingsService temporary =
            Services.Provider.GetRequiredKeyedService<IApplicationSettingsService>(AppServices.TemporarySettingsKey);

        // Окно приветствия намеренно игнорирует settings.json, поэтому и культура
        // в рантайме должна совпадать с его выбором по умолчанию. Без этого
        // ComboBox покажет "русский", а весь интерфейс останется на языке файла -
        // пользователь увидит несоответствие прямо в первой строке.
        // Когда появятся настройки конкретного пользователя, культура придёт
        // сюда из них на шаге AdvanceToMainWindow.
        ICultureService cultures = Services.Provider.GetRequiredService<ICultureService>();
        cultures.SetCulture(AppSettingsStore.ResolveCulture(temporary.CurrentCultureName));

        // Тему на время окна берём из временного хранилища: пользователь должен
        // видеть результат до подтверждения, а постоянные настройки на этом шаге
        // ещё не тронуты.
        _welcomeThemeChangedHandler = (_, e) => ApplyTheme(e.NewTheme);
        temporary.ThemeChanged += _welcomeThemeChangedHandler;
        ApplyTheme(temporary.CurrentAppTheme);

        WelcomeWindowViewModel vm = Services.Provider.GetRequiredService<WelcomeWindowViewModel>();
//for debug
#if DEBUG
        vm.FirstName = "TestName";
        vm.LastName = "SurNameTest";
        vm.MiddleName = "Patronim";
        vm.Group = "TestGroup";
#endif
        WelcomeWindow window = Services.Provider.GetRequiredService<WelcomeWindow>();
        window.DataContext = vm;

        // Обе кнопки только закрывают окно. Различие - в том, закрыли его после
        // подтверждения или нет, а решает уже обработчик Closed. Так невозможно
        // получить двойной переход из двух независимых вызовов Shutdown.
        vm.Confirmed += (_, _) =>
        {
            _welcomeConfirmed = true;
            window.Close();
        };

        vm.ExitRequested += (_, _) => window.Close();

        // Данные пользователя снимаем ДО Dispose модели представления: после
        // освобождения она уже недоступна.
        window.Closed += (_, _) =>
        {
            UserProfile? profile = _welcomeConfirmed ? vm.Profile : null;

            vm.Dispose();
            UnsubscribeWelcomeTheme(temporary);

            if (profile is not null)
                AdvanceToMainWindow(desktop, profile);
            else
                // Закрытие крестиком или кнопкой выхода: главного окна не будет,
                // а при OnExplicitShutdown приложение осталось бы висеть без окон.
                desktop.Shutdown();
        };

        // Показываем явно: desktop.MainWindow назначается только главному окну,
        // поэтому lifetime ничего не покажет.
        window.Show();
    }

    private void UnsubscribeWelcomeTheme(IApplicationSettingsService temporary)
    {
        if (_welcomeThemeChangedHandler is null)
            return;

        temporary.ThemeChanged -= _welcomeThemeChangedHandler;
        _welcomeThemeChangedHandler = null;
    }
    #endregion

#region ForMainWindow
    private void AdvanceToMainWindow(IClassicDesktopStyleApplicationLifetime desktop, UserProfile profile)
    {
        if (_mainWindowStarted)
            return;

        _mainWindowStarted = true;

        // Переносим выбор из временного хранилища в постоянное. Это единственное
        // место, где настройки попадают на диск, и единственный момент, когда
        // известен пользователь: путь %LOCALAPPDATA%\MouseLab\{slug}-{hash}
        // выводится из введённых данных. До подтверждения файла не существует,
        // поэтому стартовое состояние постоянных настроек никуда не пишется.
        IApplicationSettingsService temporary =
            Services.Provider.GetRequiredKeyedService<IApplicationSettingsService>(AppServices.TemporarySettingsKey);
        IApplicationSettingsService stored = Services.Provider.GetRequiredService<IApplicationSettingsService>();

        stored.LoadFrom(
            new AppSettingsSnapshot
            {
                User = profile,
                Culture = temporary.CurrentCultureName,
                Theme = temporary.CurrentAppTheme,
            },
            UserIdentity.ResolveSettingsPath(profile));

        // Тема постоянных настроек применяется здесь же: подписка на stored
        // появляется только теперь, а до этого применялась тема из temporary.
        _themeChangedHandler = (_, e) => ApplyTheme(e.NewTheme);
        stored.ThemeChanged += _themeChangedHandler;
        ApplyTheme(stored.CurrentAppTheme);

        // Культуру применяем ещё раз безусловно: SetCulture внутри сравнивает
        // культуру и ничего не делает, если она уже такая, так что повторный
        // вызов безопасен и фиксирует именно сохранённый выбор.
        ICultureService cultures = Services.Provider.GetRequiredService<ICultureService>();
        cultures.SetCulture(AppSettingsStore.ResolveCulture(stored.CurrentCultureName));

        MainWindowViewModel mainVm = Services.Provider.GetRequiredService<MainWindowViewModel>();
        MainWindow mainWindow = Services.Provider.GetRequiredService<MainWindow>();
        mainWindow.DataContext = mainVm;
        mainWindow.Closed += (_, _) => mainVm.Dispose();

        desktop.MainWindow = mainWindow;
        desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;

        // Обязательно: ShowMainWindow() внутри lifetime уже отработал при старте,
        // и само присваивание desktop.MainWindow окно не показывает. Проверка
        // IsVisible страхует от двойного Show в другой версии Avalonia.
        if (!mainWindow.IsVisible)
            mainWindow.Show();
    }
#endregion
    

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
        if (_appServices is null)
            return;

        // Отписываемся от событий перед Dispose, иначе обработчик остался бы
        // висеть на освобождаемом объекте.
        if (_themeChangedHandler is not null &&
            Services.Provider.GetService<IApplicationSettingsService>() is { } settings)
        {
            settings.ThemeChanged -= _themeChangedHandler;
        }

        if (viewLocator is not null)
        {
            DataTemplates.Remove(viewLocator);
            viewLocator = null;
        }

        _themeChangedHandler = null;
        _appServices.Dispose();
        _appServices = null;
    }
}
