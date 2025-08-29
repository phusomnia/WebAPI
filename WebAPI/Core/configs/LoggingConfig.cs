using Serilog;

namespace WebAPI;

public static class LoggingConfig
{
    public static void configure(WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        // builder.Logging.AddConsole();
        // builder.Logging.AddFile("app.log");

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        builder.Host.UseSerilog();
        builder.Logging.SetMinimumLevel(LogLevel.Information);
    }
}