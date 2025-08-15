using System.Text.Json.Serialization;

namespace WebAPI.Core;

public class APIResponse<T> : BaseResponse
{
    [JsonPropertyName("data")] 
    public T data { get; set; }
    [JsonPropertyName("metadata")]
    public Dictionary<string, Object> metadata { get; set; }

    public APIResponse()
    {
    }

    public APIResponse(BaseStatus status, string message,T data, Dictionary<string, Object> metata = null) : base(status, message)
    {
        this.data = data;
        this.metadata = metata;
    }
}