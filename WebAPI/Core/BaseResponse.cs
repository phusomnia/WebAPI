namespace WebAPI.Core;

public class BaseResponse
{
    public BaseStatus status { get; set; }
    public string message { get; set; }

    public BaseResponse() { }

    public BaseResponse(BaseStatus status, string message = null)
    {
        this.status = status;
        this.message = message;
    }
}