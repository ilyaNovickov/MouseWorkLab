using MouseLabAvaloniaApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MouseLabAvaloniaApp.Services.AppSettings;

/// <summary>
/// Превращает данные пользователя в путь к его настройкам.
/// </summary>
/// <remarks>
/// <para>
/// Идентичность пользователя - это его имя: отдельного логина в приложении нет,
/// поэтому «ключ» выводится из введённых данных. Чтобы один и тот же человек не
/// получил два разных каталога из-за лишнего пробела или регистра, все поля
/// перед вычислением нормализуются.
/// </para>
/// <para>
/// Класс ничего не знает о файловой системе и ничего не пишет: только чистые
/// функции. Их легко проверить и они безопасны под NativeAOT -
/// <c>SHA256.HashData</c> не требует ни кодогенерации, ни рефлексии.
/// </para>
/// </remarks>
public static class UserIdentity
{
    /// <summary>Сколько hex-символов хеша берём в имя каталога.</summary>
    public const int HashLength = 16;

    /// <summary>
    /// Предел длины читаемой части имени. Нужен, потому что Windows ограничивает
    /// путь 260 символами, а имя каталога получает и префикс, и хеш.
    /// </summary>
    public const int MaxSlugLength = 40;

    /// <summary>
    /// Разделитель полей в материале для хеша: управляющий символ 0x1F, который не
    /// может встретиться в имени и потому не создаёт неоднозначности между
    /// соседними полями. Записан приведением к типу, а не escape-последовательностью,
    /// чтобы в исходнике не было невидимого символа.
    /// </summary>
    private const char FieldSeparator = (char)0x1F;

    private const string FallbackSlug = "user";

    /// <summary>
    /// Имя каталога пользователя: читаемая часть плюс короткий хеш.
    /// </summary>
    /// <remarks>
    /// Хеш в конце не декорация: без него два студента с одинаковым именем в одной
    /// группе получили бы общие настройки.
    /// </remarks>
    public static string ResolveDirectoryName(UserProfile profile)
    {
        string slug = BuildSlug(profile);
        if (slug.Length == 0)
            slug = FallbackSlug;

        return $"{slug}-{ComputeHash(profile)}";
    }

    /// <summary>Полный путь к <c>settings.json</c> пользователя.</summary>
    public static string ResolveSettingsPath(UserProfile profile) =>
        Path.Combine(AppSettingsStore.RootDirectory, ResolveDirectoryName(profile), AppSettingsStore.SettingsFileName);

    /// <summary>Короткий хеш личности пользователя.</summary>
    public static string ComputeHash(UserProfile profile)
    {
        string material = string.Join(
            FieldSeparator,
            Normalize(profile.LastName),
            Normalize(profile.FirstName),
            Normalize(profile.MiddleName),
            Normalize(profile.Group));

        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(material));

        return Convert.ToHexString(hash)[..HashLength].ToLowerInvariant();
    }

    /// <summary>
    /// Приводит значение к сравнимому виду: убирает края, схлопывает пробелы и
    /// переводит в верхний регистр.
    /// </summary>
    /// <remarks>
    /// Именно <c>ToUpperInvariant()</c>, а не <c>ToUpper()</c>: последний зависит
    /// от текущей культуры и дал бы разные хеши для одной фамилии в зависимости от
    /// языка интерфейса - ровно та ошибка, которую здесь и устраняем.
    /// </remarks>
    public static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        string[] words = value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

        return string.Join(' ', words).ToUpperInvariant();
    }

    private static string BuildSlug(UserProfile profile)
    {
        List<string> parts = [];

        Append(parts, Normalize(profile.LastName));
        Append(parts, Normalize(profile.FirstName));

        // Отчество необязательное, и в имя каталога попадает только когда заполнено.
        string middle = Normalize(profile.MiddleName);
        if (middle.Length > 0)
            Append(parts, middle);

        Append(parts, Normalize(profile.Group));

        string slug = string.Join('_', parts);

        return slug.Length <= MaxSlugLength
            ? slug
            : slug[..MaxSlugLength].TrimEnd('_', '-');
    }

    private static void Append(List<string> parts, string normalized)
    {
        string token = Sanitize(normalized);

        if (token.Length > 0)
            parts.Add(token);
    }

    /// <summary>
    /// Оставляет в имени каталога буквы и цифры любого алфавита, а всё
    /// остальное заменяет подчёркиванием.
    /// </summary>
    /// <remarks>
    /// Именно <see cref="char.IsLetterOrDigit"/>, а не
    /// <see cref="char.IsAsciiLetterOrDigit(char)"/>: вторая проверка выбросила бы
    /// кириллицу, и каталог русского пользователя назывался бы одной группой -
    /// <c>42-ab12cd34ef56</c>. Побочный эффект в пользу: подчёркиванием
    /// заменяются и управляющие символы, и все запрещённые в имени файла
    /// <c>: * ? " &lt; &gt; | \ /</c>, так что отдельно их перечислять не нужно.
    /// </remarks>
    private static string Sanitize(string normalized)
    {
        char[] buffer = new char[normalized.Length];

        for (int i = 0; i < normalized.Length; i++)
        {
            char c = normalized[i];

            buffer[i] = char.IsLetterOrDigit(c) ? c : '_';
        }

        return new string(buffer).Trim('_');
    }
}