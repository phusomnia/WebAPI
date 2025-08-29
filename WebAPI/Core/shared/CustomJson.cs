using System.Text.Json;

namespace WebAPI.Core.shared;

public static class CustomJson
{
    public static String json(Object value, CustomJsonOptions options)
    {
        switch (options)
        {
            case CustomJsonOptions.WriteIndented:
                return JsonSerializer.Serialize(value, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            case CustomJsonOptions.None:
                return JsonSerializer.Serialize(value);      
            default:
                throw new Exception("Gyatt");
        }
        return JsonSerializer.Serialize(value);
    }
}

public enum CustomJsonOptions
{
    WriteIndented,
    None
}