using MouseLabAvaloniaApp.Models;
using System;

namespace MouseLabAvaloniaApp.Services.AppSettings;

/// <summary>
/// Постоянные пользовательские настройки: состояние в памяти плюс
/// автосохранение в <c>%LOCALAPPDATA%\MouseLab\settings.json</c>.
/// </summary>
public sealed class ApplicationSettingsService : AppSettingsServiceBase
{
    public ApplicationSettingsService(AppSettingsSnapshot snapshot) : base(snapshot)
    {
        // Подписка после base: стартовые значения выставлены в конструкторе базы
        // и не должны приводить к записи файла при создании сервиса.
        ThemeChanged += OnSettingChanged;
        CultureChanged += OnSettingChanged;
    }

    // Одна и та же реакция на оба события: перезаписать файл настроек целиком.
    // Сам снимок хранит и тему, и культуру, поэтому сохраняются оба значения.
    private void OnSettingChanged(object sender, EventArgs e) =>
        AppSettingsStore.Save(new AppSettingsSnapshot
        {
            Culture = CurrentCultureName,
            Theme = CurrentAppTheme,
        });
}