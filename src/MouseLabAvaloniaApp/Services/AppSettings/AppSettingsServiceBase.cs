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
        CurrentAppTheme = snapshot.Theme;
        _currentCultureName = AppSettingsStore.ResolveCulture(snapshot.Culture).Name;
    }

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
    /// <remarks>
    /// Существует для смены настроек на лету. Сейчас постоянные настройки
    /// заполняются один раз в конструкторе, но окно приветствия изменит их
    /// позже: когда у каждого пользователя будут свои настройки, сюда попадёт
    /// снимок из <c>%LOCALAPPDATA%\MouseLab\{userhash}\settings.json</c>.
    /// Значения, равные текущим, молча игнорируются - перезаписывать файл
    /// без причины незачем.
    /// </remarks>
    public void LoadFrom(AppSettingsSnapshot snapshot)
    {
        CurrentAppTheme = snapshot.Theme;
        CurrentCultureName = AppSettingsStore.ResolveCulture(snapshot.Culture).Name;
    }
}