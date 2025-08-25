using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using WebAPI.Context;

namespace WebAPI;

public class DatabaseConfig
{
    public static void configure(WebApplicationBuilder builder)
    {
        // Config database context
        var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
        
        builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(
            defaultConnection,
            ServerVersion.AutoDetect(defaultConnection)
        ));
        builder.Services.AddDbContext<CustomIdentityDbContext>(options => options.UseMySql(
            defaultConnection,
            ServerVersion.AutoDetect(defaultConnection)
        ));

        // Register Redis connection
        builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(builder.Configuration.GetValue<string>("Redis")!));
    }
}