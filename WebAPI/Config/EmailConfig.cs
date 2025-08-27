using Microsoft.AspNetCore.Identity;

namespace WebAPI.Config;

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