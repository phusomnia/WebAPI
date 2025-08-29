namespace WebAPI.Core;
using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BaseStatus
{
    None,
    Success,
    Error,
    Warning,
    NotFound,
    Unauthorized,
}