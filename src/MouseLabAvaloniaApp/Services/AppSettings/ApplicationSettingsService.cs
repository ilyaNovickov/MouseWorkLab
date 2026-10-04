using MouseLabAvaloniaApp.Models;
using System;

namespace MouseLabAvaloniaApp.Services.AppSettings;

/// <summary>
/// Состояние пользовательских настроек в памяти + автоматическое сохранение в файл.
/// </summary>
public sealed class ApplicationSettingsService : TemporaryAppSettingsService //IApplicationSettingsService
{
    public ApplicationSettingsService(AppSettingsSnapshot snapshot)
    {
        // Стартовые значения выставляем напрямую в поля, не поднимая события:
        // до подписки обработчиков это всё равно никто не услышит, а лишняя
        // запись в settings.json при каждом запуске не нужна.
        CurrentAppTheme = snapshot.Theme;
        CurrentCultureName = AppSettingsStore.ResolveCulture(snapshot.Culture).Name;

        // Одна и та же реакция на оба события: перезаписать файл настроек целиком.
        // Сам снимок хранит и тему, и культуру, поэтому сохраняются оба значения.
        ThemeChanged += OnSettingChanged;
        CultureChanged += OnSettingChanged;
    }

    private void OnSettingChanged(object sender, EventArgs e) =>
        AppSettingsStore.Save(new AppSettingsSnapshot
        {
            Culture = CurrentCultureName,
            Theme = CurrentAppTheme,
        });
}
