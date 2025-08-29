using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using WebAPI.Entities;
using WebAPI.Infrastructure.Data;

namespace WebAPI.Core.configs;

public class DatabaseConfig
{
    public static void configure(WebApplicationBuilder builder)
    {   
        // Config database context
        var defaultConnection = builder.Configuration.GetConnectionString("defaultConnection");
        var identityConnection = builder.Configuration.GetConnectionString("demoConnection");
        
        builder.Services.AddDbContext<DotnetContext>(options => options.UseMySql(
            defaultConnection,
            ServerVersion.AutoDetect(defaultConnection)
        ));
        builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(
            defaultConnection,
            ServerVersion.AutoDetect(defaultConnection)
        ));

        builder.Services.AddDbContext<CustomIdentityDbContext>(options => options.UseMySql(
            identityConnection,
            ServerVersion.AutoDetect(identityConnection)
        ));
        
        builder.Services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<CustomIdentityDbContext>()
            .AddDefaultTokenProviders();
        
        // Register caching
        // builder.Services.AddMemoryCache();
        builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(builder.Configuration.GetValue<string>("Redis")!));
        
        builder.Services.AddScoped<PasswordHasher<Account>>();
    }
}