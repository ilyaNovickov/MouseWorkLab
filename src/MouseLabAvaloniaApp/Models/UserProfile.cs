namespace MouseLabAvaloniaApp.Models;

/// <summary>
/// Данные пользователя, введённые в окне приветствия.
/// </summary>
/// <remarks>
/// Хранятся в <c>settings.json</c> рядом с настройками пользователя, в блоке
/// <c>"user"</c>. Поля <c>init</c>, а не <c>set</c>: профиль не меняется после
/// создания, и такой вид честнее отражает то, что он попадает на диск один раз.
/// </remarks>
public sealed class UserProfile
{
    public UserProfile(string firstName, string lastName, string? middleName, string group)
    {
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
        Group = group;
    }

    [System.Text.Json.Serialization.JsonPropertyName("firstName")]
    public string FirstName { get; init; }

    [System.Text.Json.Serialization.JsonPropertyName("lastName")]
    public string LastName { get; init; }

    [System.Text.Json.Serialization.JsonPropertyName("middleName")]
    public string? MiddleName { get; init; }

    [System.Text.Json.Serialization.JsonPropertyName("group")]
    public string Group { get; init; }
}