using MouseLabAvaloniaDesktop.Models.Settings;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MouseLabAvaloniaDesktop.Services.Implementations;

[JsonSerializable(typeof(AppSnapshot))]
[JsonSourceGenerationOptions(
WriteIndented = true,                    // красивый вывод
PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]
internal partial class AppSettingsJsonContext : JsonSerializerContext
{
}

public class SettingsService
{
    private readonly string _filePath;

    public AppSnapshot Current { get; private set; }

    public SettingsService()
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        string dir = Path.Combine(appData, "MouseLab");

        Directory.CreateDirectory(dir);

        _filePath = Path.Combine(dir, "settings.json");

        Current = LoadFromDisk(_filePath);
    }

    private static AppSnapshot LoadFromDisk(string filePath)
    {
        if (!File.Exists(filePath))
            return new AppSnapshot();

        try
        {
            string json = File.ReadAllText(filePath);

            return JsonSerializer.Deserialize<AppSnapshot>(json, AppSettingsJsonContext.Default.AppSnapshot) ?? new AppSnapshot();
        }
        catch (JsonException)
        {
            return new AppSnapshot();
        }
    }

    public AppSnapshot CreateClone()
    {
        // Самый надёжный deep-clone для POCO:
        // сериализация → десериализация.
        // Для маленького объекта настроек это мгновенно.
        string json = JsonSerializer.Serialize(Current, AppSettingsJsonContext.Default.AppSnapshot);
        return JsonSerializer.Deserialize<AppSnapshot>(json, AppSettingsJsonContext.Default.AppSnapshot)!;
    }

    public void ApplyAndSave(AppSnapshot clone)
    {
        Current.CopyFrom(clone);
        SaveToDisk();
    }
    private void SaveToDisk()
    {
        string json = JsonSerializer.Serialize(Current, AppSettingsJsonContext.Default.AppSnapshot);
        File.WriteAllText(_filePath, json);
    }
}
