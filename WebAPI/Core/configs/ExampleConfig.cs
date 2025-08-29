using System.Runtime.InteropServices.JavaScript;
using WebAPI.Core.shared;
using WebAPI.Demo;

namespace WebAPI.Core.configs;

public static class ExampleConfig
{
    public static void configure(WebApplicationBuilder builder)
    {
        builder.Services.AddMemoryCache();
        // builder.Services.AddAuthorization(options =>
        // {
        //     options.AddPolicy("Permission", policy =>
        //     {
        //         policy.Requirements.Add(new PermissionRequirement());
        //     });
        // });

        var baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Entities");
        var folder = Path.GetFullPath(Path.Combine(baseDir, @"../../Entities"));
        Console.WriteLine(CustomJson.json(folder, CustomJsonOptions.None));
    }
}