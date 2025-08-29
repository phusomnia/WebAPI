using Microsoft.AspNetCore.Identity;

namespace WebAPI.Core.configs;

public static class EmailConfig
{
    public static void configure(WebApplicationBuilder builder)
    {
        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.SignIn.RequireConfirmedEmail = true;
        });
    }
}