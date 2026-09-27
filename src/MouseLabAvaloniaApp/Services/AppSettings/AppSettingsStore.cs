using MouseLabAvaloniaApp.Models;
using System;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MouseLabAvaloniaApp.Services.AppSettings;

public sealed class AppSettingsSnapshot
{
    [JsonPropertyName("culture")]
    public string? Culture { get; init; }

    [JsonPropertyName("theme")]
    public Themes Theme { get; init; } = Themes.Default;
}

[JsonSourceGenerationOptions(
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(AppSettingsSnapshot))]
public sealed partial class AppSettingsJsonContext : JsonSerializerContext;

public static class AppSettingsStore
{
    public const string DefaultCultureName = "en-US";

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
            return new AppSettingsSnapshot();
        }
    }

    public static void Save(AppSettingsSnapshot snapshot)
    {
        try
        {
            string? directory = Path.GetDirectoryName(SettingsFilePath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(
                SettingsFilePath,
                JsonSerializer.Serialize(snapshot, AppSettingsJsonContext.Default.AppSettingsSnapshot));
        }
        catch (Exception)
        {
            // Settings persistence must never take the application down.
        }
    }

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
