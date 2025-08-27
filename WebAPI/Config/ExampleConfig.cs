using WebAPI.Example;
using WebAPI.Example.Cache;

namespace WebAPI.Config;

public static class ExampleConfig
{
    public static void configure(WebApplicationBuilder builder)
    {
        builder.Services.AddMemoryCache();
        // builder.Services.AddScoped<CacheResourceFilter>();
        // builder.Services.AddScoped<CacheManager>();
        // builder.Services.AddControllers(opts =>
        // {
        //     opts.Filters.Add<CacheResourceFilter>();
        // });
    }
}