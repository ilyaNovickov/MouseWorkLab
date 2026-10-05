using MouseLabAvaloniaApp.Models;
using System;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MouseLabAvaloniaApp.Services.AppSettings;

/// <summary>
/// Неизменяемый снимок настроек одного пользователя - ровно те данные, что лежат
/// в его <c>settings.json</c>.
/// </summary>
public sealed class AppSettingsSnapshot
{
    [JsonPropertyName("user")]
    public UserProfile? User { get; init; }

    /// <summary>
    /// Код культуры. <c>null</c> означает «взять умолчание приложения», поэтому
    /// здесь намеренно нет инициализатора: значение по умолчанию, равное
    /// <c>CultureInfo.CurrentCulture.Name</c>, завязало бы снимок на окружающую
    /// культуру процесса.
    /// </summary>
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
///
/// <b>Новый тип сюда забывать нельзя:</b> без строки ниже сборка останется
/// чистой, а тип не будет сериализован и при первом обращении даст исключение в
/// рантайме. Ровно тот класс тихой поломки, что и поиск типа по имени.
///
///
/// Имена записываются escape-последовательностями (обычное поведение
/// System.Text.Json): <c>Ада</c> станет <c>Ада</c>. Файл читается
/// только приложением, так что менять кодировку ради читаемости глазами
/// нецелесообразно.
/// </remarks>
[JsonSourceGenerationOptions(
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(AppSettingsSnapshot))]
[JsonSerializable(typeof(UserProfile))]
public sealed partial class AppSettingsJsonContext : JsonSerializerContext;

/// <summary>
/// Чтение и запись файлов настроек пользователей.
/// </summary>
/// <remarks>
/// Статический класс намеренно: к нему обращается <see cref="AppServices"/>
/// ещё до сборки контейнера DI, когда сервисы недоступны. Путь передаётся
/// параметром - пользователя на старте ещё нет, привязать путь на весь сеанс
/// нельзя.
/// </remarks>
public static class AppSettingsStore
{
    /// <summary>Культура по умолчанию, если в настройках её нет или она неизвестна.</summary>
    public const string DefaultCultureName = "ru-RU";

    /// <summary>Имя файла настроек внутри каталога пользователя.</summary>
    public const string SettingsFileName = "settings.json";

    /// <summary>
    /// Корневой каталог приложения: <c>%LOCALAPPDATA%\MouseLab</c> на Windows,
    /// <c>~/.local/share/MouseLab</c> на Linux. Внутри лежат каталоги пользователей.
    /// </summary>
    public static string RootDirectory { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "MouseLab");

    /// <summary>
    /// Старый общий файл настроек. Больше не читается и не пишется - настройки
    /// принадлежат конкретному пользователю. Оставлен на диске намеренно:
    /// удалять чужие данные без спроса неправильно.
    /// </summary>
    public static string LegacySettingsFilePath { get; } = Path.Combine(RootDirectory, SettingsFileName);

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
            // Битый или несовместимый settings.json не должен мешать запуску -
            // просто стартуем с настройками по умолчанию.
            return new AppSettingsSnapshot();
        }
    }

    public static void Save(string filePath, AppSettingsSnapshot snapshot)
    {
        try
        {
            // Каталога может не быть при первом запуске.
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
    /// Удаляет настройки пользователей, которыми не пользовались дольше
    /// <paramref name="maxAge"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Возраст определяется по времени изменения <b>файла настроек</b>, а не
    /// каталога: перезапись существующего файла не обновляет время изменения
    /// родительского каталога, и проверка по каталогу замерла бы навсегда на
    /// дате первого запуска.
    /// </para>
    /// <para>
    /// Удаляется весь каталог пользователя, а не только файл: оставлять после
    /// себя пустые каталоги незачем. Возраст считается от <c>LastWriteTimeUtc</c>,
    /// то есть активный пользователь старые настройки не потеряет никогда.
    /// </para>
    /// <para>
    /// Это безвозвратное удаление данных, поэтому исключения глушатся: уборка не
    /// имеет права помешать запуску приложения.
    /// </para>
    /// </remarks>
    public static int CleanupOlderThan(TimeSpan maxAge, string? rootDirectory = null)
    {
        try
        {
            string root = rootDirectory ?? RootDirectory;

            if (!Directory.Exists(root))
                return 0;

            DateTime cutoff = DateTime.UtcNow - maxAge;
            int removed = 0;

            foreach (string directory in Directory.EnumerateDirectories(root))
            {
                try
                {
                    // Без проверки на каталог: сортировка по дате каталога была бы
                    // дешевле, но она не отражает последнего реального использования.
                    string settingsPath = Path.Combine(directory, SettingsFileName);
                    if (!File.Exists(settingsPath))
                        continue;

                    if (File.GetLastWriteTimeUtc(settingsPath) < cutoff)
                    {
                        Directory.Delete(directory, recursive: true);
                        removed++;
                    }
                }
                catch (Exception)
                {
                    // Один неудачный каталог не должен отменять уборку остальных.
                }
            }

            return removed;
        }
        catch (Exception)
        {
            return 0;
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