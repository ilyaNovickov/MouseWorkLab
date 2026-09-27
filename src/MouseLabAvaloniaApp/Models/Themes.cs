using System.Text.Json.Serialization;

namespace MouseLabAvaloniaApp.Models;

[JsonConverter(typeof(JsonStringEnumConverter<Themes>))]
public enum Themes
{
    Light,
    Dark,

    Default
}
