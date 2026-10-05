using MouseLabAvaloniaApp.Models;
using System;

namespace MouseLabAvaloniaApp.Services.AppSettings;

/// <summary>
/// Общее состояние пользовательских настроек: значения, события и правило
/// «событие только при реальном изменении».
/// </summary>
/// <remarks>
/// <para>
/// Вынесено отдельно от <see cref="ApplicationSettingsService"/>, потому что
/// <see cref="TemporaryAppSettingsService"/> отличается от него ровно одним:
/// сохранять или нет. Держать два почти одинаковых класса нельзя - правки
/// разъезжаются, и через месяц они уже ведут себя по-разному.
/// </para>
/// <para>
/// Порядок в конструкторе базы значим: значения выставляются <b>до</b> того, как
/// производный класс подпишется на события. Иначе первая же запись в
/// settings.json произошла бы просто при создании сервиса.
/// </para>
/// </remarks>
public abstract class AppSettingsServiceBase : IApplicationSettingsService
{
    private string _currentCultureName;

    protected AppSettingsServiceBase(AppSettingsSnapshot snapshot)
    {
        User = snapshot.User;
        CurrentAppTheme = snapshot.Theme;
        _currentCultureName = AppSettingsStore.ResolveCulture(snapshot.Culture).Name;
    }

    /// <summary>
    /// Данные текущего пользователя. Приходят из снимка и сохраняются при любой
    /// записи, иначе первая же смена темы стёрла бы личность из файла.
    /// </summary>
    protected UserProfile? User { get; private set; }

    /// <summary>
    /// Куда сохранять настройки. <c>null</c>, пока файл не привязан через
    /// <see cref="LoadFrom"/>: на старте пользователя ещё нет.
    /// </summary>
    protected string? FilePath { get; private set; }

    public Themes CurrentAppTheme
    {
        get;
        set
        {
            // Игнорируем повторную установку того же значения, иначе событие
            // сработает без причины.
            if (field == value)
                return;

            field = value;
            ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(value));
        }
    }

    /// <summary>
    /// Код выбранной культуры ("ru-RU"). Значение нормализуется через
    /// <see cref="AppSettingsStore.ResolveCulture"/>, поэтому неизвестная или
    /// пустая строка превращается в культуру по умолчанию, а не в ошибку.
    /// </summary>
    public string CurrentCultureName
    {
        get => _currentCultureName;
        set
        {
            string resolved = AppSettingsStore.ResolveCulture(value).Name;
            if (_currentCultureName == resolved)
                return;

            _currentCultureName = resolved;
            CultureChanged?.Invoke(this, new CultureChangedEventArgs(AppSettingsStore.ResolveCulture(resolved)));
        }
    }

    public event ThemeChangedEventHandler? ThemeChanged;

    public event CultureChangedEventHandler? CultureChanged;

    /// <summary>
    /// Заменяет текущие значения на значения из снимка, поднимая события только
    /// для реально изменившихся полей.
    /// </summary>
    /// <param name="snapshot">Новые значения.</param>
    /// <param name="filePath">
    /// Куда сохранять. Постоянные настройки создаются на старте, когда
    /// пользователя ещё нет и путь неизвестен, поэтому файл привязывается здесь -
    /// в момент, когда личность уже установлена.
    /// </param>
    /// <remarks>
    /// Значения, равные текущим, молча игнорируются: перезаписывать файл без
    /// причины незачем. Поэтому для нового пользователя, принявшего значения по
    /// умолчанию, не изменится ничего - и без <see cref="PersistIfMissing"/> его
    /// файл не появился бы вовсе.
    /// </remarks>
    public void LoadFrom(AppSettingsSnapshot snapshot, string filePath)
    {
        User = snapshot.User;
        FilePath = filePath;

        CurrentAppTheme = snapshot.Theme;
        CurrentCultureName = AppSettingsStore.ResolveCulture(snapshot.Culture).Name;

        PersistIfMissing();
    }

    /// <summary>
    /// Создаёт файл, если его ещё нет. Вызывается один раз - в момент привязки
    /// пути.
    /// </summary>
    /// <remarks>
    /// Пустая реализация в базе, потому что <see cref="TemporaryAppSettingsService"/>
    /// не должен ничего писать. Переопределяет только постоянный сервис.
    /// </remarks>
    protected virtual void PersistIfMissing() { }
}