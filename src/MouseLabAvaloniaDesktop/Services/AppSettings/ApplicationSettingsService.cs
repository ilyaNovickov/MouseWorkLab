using MouseLabAvaloniaDesktop.Models;
using System;
using System.IO;

namespace MouseLabAvaloniaDesktop.Services.AppSettings;

/// <summary>
/// Пользовательские настройки приложения: состояние в памяти плюс
/// автосохранение в привязанный файл настроек.
/// </summary>
/// <remarks>
/// Путь к файлу не привязан до <see cref="LoadFrom"/>: имя и расположение файла
/// выберет проектный слой («проект» пользователя). До привязки изменения живут
/// только в памяти и никуда не пишутся.
/// </remarks>
public sealed class ApplicationSettingsService : IApplicationSettingsService
{
    private Themes _currentAppTheme;
    private string _currentCultureName;
    private int _worktopResolution;
    private bool _experimental;

    private string? _filePath;

    public ApplicationSettingsService(AppSettingsSnapshot snapshot)
    {
        // Поля, а не свойства: стартовые значения не должны поднимать события
        // и приводить к записи файла при создании сервиса.
        _currentAppTheme = snapshot.Theme;
        _currentCultureName = AppSettingsStore.ResolveCulture(snapshot.Culture).Name;
        _worktopResolution = snapshot.WorktopResolution;
        _experimental = snapshot.Experimental;

        // Одна и та же реакция на все три события: перезаписать файл настроек
        // целиком. До привязки пути запись молча пропускается.
        ThemeChanged += OnSettingChanged;
        CultureChanged += OnSettingChanged;
        ModelingChanged += OnSettingChanged;
    }

    public Themes CurrentAppTheme
    {
        get => _currentAppTheme;
        set
        {
            // Игнорируем повторную установку того же значения, иначе событие
            // сработает без причины.
            if (_currentAppTheme == value)
                return;

            _currentAppTheme = value;
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

    public int WorktopResolution
    {
        get => _worktopResolution;
        set
        {
            if (_worktopResolution == value)
                return;

            _worktopResolution = value;
            ModelingChanged?.Invoke(this, new ModelingChangedEventArgs(value, _experimental));
        }
    }

    public bool ExperimentalOptions
    {
        get => _experimental;
        set
        {
            if (_experimental == value)
                return;

            _experimental = value;
            ModelingChanged?.Invoke(this, new ModelingChangedEventArgs(_worktopResolution, value));
        }
    }

    public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

    public event EventHandler<CultureChangedEventArgs>? CultureChanged;

    public event EventHandler<ModelingChangedEventArgs>? ModelingChanged;

    public void LoadFrom(AppSettingsSnapshot snapshot, string? filePath)
    {
        _filePath = filePath;

        // События поднимутся только для реально изменившихся значений; каждый
        // обработчик перезапишет файл — цена несущественная, а без привязки
        // (FilePath == null) записи пропускаются целиком.
        CurrentAppTheme = snapshot.Theme;
        CurrentCultureName = AppSettingsStore.ResolveCulture(snapshot.Culture).Name;
        WorktopResolution = snapshot.WorktopResolution;
        ExperimentalOptions = snapshot.Experimental;

        // Файл появляется при привязке пути, даже если значения не изменились:
        // иначе настройки существовали бы только в памяти.
        if (!string.IsNullOrEmpty(filePath) && !File.Exists(filePath))
            Write(filePath);
    }

    private void OnSettingChanged(object? sender, EventArgs e)
    {
        string? path = _filePath;
        if (string.IsNullOrEmpty(path))
            return;

        Write(path);
    }

    private void Write(string path) =>
        AppSettingsStore.Save(path, new AppSettingsSnapshot
        {
            Theme = _currentAppTheme,
            Culture = _currentCultureName,
            WorktopResolution = _worktopResolution,
            Experimental = _experimental
        });
}
