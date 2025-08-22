using System.Text.Json;

namespace WebAPI.Features.Shared;

public static class CustomJson
{
    public static String json(Object value)
    {
        return JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }
}