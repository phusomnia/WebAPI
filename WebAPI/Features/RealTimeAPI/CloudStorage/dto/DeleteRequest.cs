namespace WebAPI.Features.RealTimeAPI.CloudStorage.dto;

public class DeleteRequest
{
    public CloudProvider provider { get; set; }
    public string publicId { get; set; }

    public DeleteRequest()
    {
    }

    public DeleteRequest(
        CloudProvider provider,
        string publicId
    )
    {
        this.provider = provider;
        this.publicId = publicId;
    }
}