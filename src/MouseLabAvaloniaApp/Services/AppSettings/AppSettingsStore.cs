using MouseLabAvaloniaApp.Models;
using System;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MouseLabAvaloniaApp.Services.AppSettings;

/// <summary>
/// Неизменяемый снимок настроек приложения - ровно те данные, что лежат в settings.json.
/// </summary>
public sealed class AppSettingsSnapshot
{
    [JsonPropertyName("culture")]
    public string? Culture { get; init; }

    [JsonPropertyName("theme")]
    public Themes Theme { get; init; } = Themes.Default;
}

/// <summary>
/// Контекст сериализации, генерируемый компилятором (System.Text.Json source generation).
/// </summary>
/// <remarks>
/// Обязателен для NativeAOT: обычные JsonSerializer.Serialize/Deserialize помечены
/// IL2026/IL3050, потому что опираются на рефлексию, и в AOT-сборке настройки
/// молча перестали бы читаться. С этим контекстом сериализатор работает по
/// сгенерированному коду - без рефлексии и без предупреждений.
/// </remarks>
[JsonSourceGenerationOptions(
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(AppSettingsSnapshot))]
public sealed partial class AppSettingsJsonContext : JsonSerializerContext;

/// <summary>
/// Чтение и запись файла настроек. Статический класс намеренно: к нему обращается
/// App ещё до сборки контейнера DI, когда сервисы недоступны.
/// </summary>
public static class AppSettingsStore
{
    /// <summary>Культура по умолчанию, если в настройках её нет или она неизвестна.</summary>
    public const string DefaultCultureName = "en-US";

    // %LOCALAPPDATA%\MouseLab\settings.json на Windows, ~/.local/share/MouseLab на Linux.
    public static string SettingsFilePath { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "MouseLab",
        "settings.json");

    public static AppSettingsSnapshot Load()
    {
        try
        {
            if (!File.Exists(SettingsFilePath))
                return new AppSettingsSnapshot();

            AppSettingsSnapshot? snapshot = JsonSerializer.Deserialize(
                File.ReadAllText(SettingsFilePath),
                AppSettingsJsonContext.Default.AppSettingsSnapshot);

            return snapshot ?? new AppSettingsSnapshot();
        }
        catch (Exception)
        {
            // Битый или несовместимый settings.json не должен мешать запуску -
            // просто стартуем с настройками по умолчанию.
            return new AppSettingsSnapshot();
        }
    }

    public static void Save(AppSettingsSnapshot snapshot)
    {
        try
        {
            // Каталога может не быть при первом запуске.
            string? directory = Path.GetDirectoryName(SettingsFilePath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(
                SettingsFilePath,
                JsonSerializer.Serialize(snapshot, AppSettingsJsonContext.Default.AppSettingsSnapshot));
        }
        catch (Exception)
        {
            // Сохранение настроек не должно ронять приложение.
        }
    }

    /// <summary>
    /// Превращает строку из настроек в CultureInfo, подставляя культуру по умолчанию
    /// при любой проблеме. cultureName хранится строкой специально: CultureInfo
    /// не сериализуется в JSON "человеческим" образом.
    /// </summary>
    public static CultureInfo ResolveCulture(string? cultureName)
    {
        if (string.IsNullOrWhiteSpace(cultureName))
            return CultureInfo.GetCultureInfo(DefaultCultureName);

        try
        {
            return CultureInfo.GetCultureInfo(cultureName);
        }
        catch (CultureNotFoundException)
        {
            return CultureInfo.GetCultureInfo(DefaultCultureName);
        }
    }
}
