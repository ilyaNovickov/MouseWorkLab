using System;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using MouseLabAvaloniaDesktop.Models;
using ProTranslate.Generated;

namespace MouseLabAvaloniaDesktop.Services.AppSettings;

/// <summary>
/// Контекст сериализации, генерируемый компилятором (System.Text.Json source generation).
/// </summary>
/// <remarks>
/// Обязателен для NativeAOT: обычные JsonSerializer.Serialize/Deserialize помечены
/// IL2026/IL3050, потому что опираются на рефлексию. Новый тип в снимке настроек
/// забывать здесь нельзя — сборка останется чистой, а сериализация упадёт в рантайме.
/// </remarks>
[JsonSourceGenerationOptions(
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(AppSettingsSnapshot))]
public sealed partial class AppSettingsJsonContext : JsonSerializerContext;

/// <summary>
/// Чтение и запись файла настроек. Путь передаётся параметром: имя и расположение
/// файла выбирает будущий проектный слой («проект» пользователя), поэтому хранить
/// путь здесь нельзя.
/// </summary>
public static class AppSettingsStore
{
    /// <summary>Культура по умолчанию, если в настройках её нет или она неизвестна.</summary>
    public const string DefaultCultureName = "ru-RU";

    public static AppSettingsSnapshot Load(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
                return new AppSettingsSnapshot();

            AppSettingsSnapshot? snapshot = JsonSerializer.Deserialize(
                File.ReadAllText(filePath),
                AppSettingsJsonContext.Default.AppSettingsSnapshot);

            return snapshot ?? new AppSettingsSnapshot();
        }
        catch (Exception)
        {
            // Битый или несовместимый файл настроек не должен мешать запуску —
            // стартуем с настройками по умолчанию.
            return new AppSettingsSnapshot();
        }
    }

    public static void Save(string filePath, AppSettingsSnapshot snapshot)
    {
        try
        {
            // Каталога может не быть при первой записи.
            string? directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(
                filePath,
                JsonSerializer.Serialize(snapshot, AppSettingsJsonContext.Default.AppSettingsSnapshot));
        }
        catch (Exception)
        {
            // Сохранение настроек не должно ронять приложение.
        }
    }

    /// <summary>
    /// Превращает строку из настроек в CultureInfo, подставляя культуру по умолчанию
    /// при любой проблеме. Строка, а не CultureInfo: CultureInfo не сериализуется в
    /// JSON «человеческим» образом.
    /// </summary>
    /// <remarks>
    /// Проверки на CultureNotFoundException недостаточно: под ICU (Linux) .NET
    /// принимает произвольные корректные BCP-47 имена вроде «xx-XX». Источник
    /// истины — сгенерированный манифест каталогов переводов: язык без каталога
    /// для пользователя бесполезен, поэтому неизвестный ему код означает опечатку
    /// в настройках и должен откатываться к умолчанию.
    /// </remarks>
    public static CultureInfo ResolveCulture(string? cultureName)
    {
        if (string.IsNullOrWhiteSpace(cultureName))
            return CultureInfo.GetCultureInfo(DefaultCultureName);

        try
        {
            CultureInfo culture = CultureInfo.GetCultureInfo(cultureName);

            ReadOnlySpan<string> supported = ProTranslateProviderManifest.Cultures;
            for (int i = 0; i < supported.Length; i++)
            {
                if (string.Equals(supported[i], culture.Name, StringComparison.OrdinalIgnoreCase))
                    return culture;
            }

            return CultureInfo.GetCultureInfo(DefaultCultureName);
        }
        catch (CultureNotFoundException)
        {
            return CultureInfo.GetCultureInfo(DefaultCultureName);
        }
    }
}
