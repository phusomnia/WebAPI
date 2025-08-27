using CloudinaryDotNet;
using WebAPI.Annotation;
using WebAPI.Features.Shared;

namespace WebAPI.Features.RealTimeAPI.CloudStorage;

[Configuration]
public class CloudinaryConfig
{
    private readonly IConfiguration _config;
    private static ILogger<CloudinaryConfig> _logger;
    
    public CloudinaryConfig(
        IConfiguration config
        )
    {
        _config = config;
        _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CloudinaryConfig>();
        _logger.LogInformation("Cloudinary...");
    }

    public Cloudinary cloudinary()
    {
        var config = _config["Cloudinary:url"];
        Cloudinary cloud = new Cloudinary(config);
        cloud.Api.Secure = true;
        
        _logger.LogInformation(CustomJson.json(cloud, CustomJsonOptions.WriteIndented));
        
        return cloud;
    }
}