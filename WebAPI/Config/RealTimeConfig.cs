using Microsoft.AspNetCore.Identity;

namespace WebAPI.Config;

public class RealTimeConfig
{
    public static void Configure(WebApplicationBuilder builder)
    {
        builder.Services.AddSignalR();
    }
}