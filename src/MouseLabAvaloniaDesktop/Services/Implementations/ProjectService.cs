using MouseLabAvaloniaDesktop.Models.DTO;
using MouseLabAvaloniaDesktop.Models.Settings;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MouseLabAvaloniaDesktop.Services.Implementations;

[JsonSerializable(typeof(ProjectSnapshot))]
[JsonSerializable(typeof(UiSettingsDto))]
[JsonSerializable(typeof(ModelingSettingsDto))]
[JsonSourceGenerationOptions(
WriteIndented = true,                    // красивый вывод
PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]
internal partial class ProjectJsonContext : JsonSerializerContext
{
}

public class ProjectService
{

    public ProjectService(UiSettings uiSettings, ModelingSettings modelingSettings)
    {
        UiSettings = uiSettings;
        ModelingSettings = modelingSettings;

        //CurrentFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "snapshot.json");
    }

    public string? CurrentFilePath { get; private set; }

    public bool IsLoading { get; private set; } = false;


    public event EventHandler? ProjectLoaded;

    public UiSettings UiSettings { get; }

    public ModelingSettings ModelingSettings { get; }

    private ProjectSnapshot GetProjectSnapshot()
    {
        ProjectSnapshot projectSnapshot = new ProjectSnapshot();

        projectSnapshot.UiSettings = this.UiSettings.ToDto();
        projectSnapshot.ModelingSettings = this.ModelingSettings.ToDto();

        return projectSnapshot;
    }

    public void Save()
    {
        SaveProject(CurrentFilePath, GetProjectSnapshot());
    }

    public void SaveTo(string filePath)
    {
        SaveProject(filePath, GetProjectSnapshot());
        CurrentFilePath = filePath;
    }

    public static void SaveProject(string? filePath, ProjectSnapshot snapshot)
    {
        if (!File.Exists(filePath))
            throw new Exception("File doesn't exist");

        string json = JsonSerializer.Serialize(snapshot, ProjectJsonContext.Default.ProjectSnapshot);
        File.WriteAllText(filePath, json);
    }

    public void Load(string filePath)
    {
        if (!File.Exists(filePath))
            throw new Exception("File doesn't exist");

        string json = File.ReadAllText(filePath); 
        ProjectSnapshot? dto = JsonSerializer.Deserialize(json, ProjectJsonContext.Default.ProjectSnapshot);

        IsLoading = true;

        if (dto is not null)
        {
            this.UiSettings.LoadDto(dto.UiSettings ?? new UiSettingsDto());
            this.ModelingSettings.LoadDto(dto.ModelingSettings ?? new ModelingSettingsDto());
        }


        this.ModelingSettings.ValidateProperties();

        CurrentFilePath = filePath;

        IsLoading = false;

        ProjectLoaded?.Invoke(this, EventArgs.Empty);
    }

    /*
    private static ProjectSnapshot LoadFromDisk(string filePath)
    {
        if (!File.Exists(filePath))
            return new ProjectSnapshot();

        try
        {
            string json = File.ReadAllText(filePath);

            return JsonSerializer.Deserialize<ProjectSnapshot>(json, AppSettingsJsonContext.Default.AppSnapshot) ?? new ProjectSnapshot();
        }
        catch (JsonException)
        {
            return new ProjectSnapshot();
        }
    }

    public ProjectSnapshot CreateClone()
    {
        // Самый надёжный deep-clone для POCO:
        // сериализация → десериализация.
        // Для маленького объекта настроек это мгновенно.
        string json = JsonSerializer.Serialize(Current, AppSettingsJsonContext.Default.AppSnapshot);
        return JsonSerializer.Deserialize<ProjectSnapshot>(json, AppSettingsJsonContext.Default.AppSnapshot)!;
    }

    public void ApplyAndSave(ProjectSnapshot clone)
    {
        Current.CopyFrom(clone);
        SaveToDisk();
    }

    */
}
