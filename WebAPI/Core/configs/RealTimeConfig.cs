using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using WebAPI.Example.Identity;

namespace WebAPI.Config;

public class RealTimeConfig
{
    public static void Configure(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IEmailSender, EmailSender>();
        builder.Services.AddSignalR();
    }
}