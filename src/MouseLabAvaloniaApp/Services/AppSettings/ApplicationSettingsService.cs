using MouseLabAvaloniaApp.Models;
using System;
using System.IO;

namespace MouseLabAvaloniaApp.Services.AppSettings;

/// <summary>
/// Постоянные настройки конкретного пользователя: состояние в памяти плюс
/// автосохранение в его <c>settings.json</c>.
/// </summary>
/// <remarks>
/// Экземпляр создаётся на старте, когда пользователя ещё нет, поэтому файл
/// появляется только после <see cref="AppSettingsServiceBase.LoadFrom"/>.
/// До этого момента любые изменения остаются в памяти и никуда не пишутся.
/// </remarks>
public sealed class ApplicationSettingsService : AppSettingsServiceBase
{
    public ApplicationSettingsService(AppSettingsSnapshot snapshot) : base(snapshot)
    {
        // Подписка после base: стартовые значения выставлены в конструкторе базы
        // и не должны приводить к записи файла при создании сервиса.
        ThemeChanged += OnSettingChanged;
        CultureChanged += OnSettingChanged;
    }

    // Файл появляется при первой же привязке пути, даже если пользователь принял
    // значения по умолчанию и не поменял ни одной настройки. Иначе личность
    // осталась бы только в памяти: подтвердил окно, закрыл программу - и на
    // следующем запуске всё сначала.
    protected override void PersistIfMissing()
    {
        string? path = FilePath;

        if (!string.IsNullOrEmpty(path) && !File.Exists(path))
            Write(path);
    }

    // Одна и та же реакция на оба события: перезаписать файл настроек целиком.
    // User обязателен в каждом снимке - без него первая же смена темы удалила бы
    // личность пользователя из его файла.
    private void OnSettingChanged(object sender, EventArgs e)
    {
        string? path = FilePath;
        if (string.IsNullOrEmpty(path))
            return;

        Write(path);
    }

    private void Write(string path) =>
        AppSettingsStore.Save(path, new AppSettingsSnapshot
        {
            User = User,
            Culture = CurrentCultureName,
            Theme = CurrentAppTheme,
        });
}