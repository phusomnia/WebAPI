using System.Reflection;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using WebAPI.Core.shared;
using WebAPI.Features.RealTimeAPI.CloudStorage.controller;
using WebAPI.Features.RealTimeAPI.CloudStorage.dto;

namespace WebAPI.Features.RealTimeAPI.CloudStorage;

public class CloudinaryStorage : CloudStorage
{
    private readonly CloudinaryConfig _config;
    private readonly Cloudinary _cloundinary;
    private static ILogger<CloudinaryConfig> _logger;

    public CloudinaryStorage(
        CloudinaryConfig config
        )
    {
        _config = config;
        _cloundinary = _config.cloudinary();
        _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CloudinaryConfig>();
    }

    public Object upload(IFormFile file)
    {
        var stream = file.OpenReadStream();
        var customFileName = Guid.NewGuid().ToString();
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(file.FileName, stream),
            PublicId = customFileName
        };
        var uploadResult = _cloundinary.Upload(uploadParams);
        
        _logger.LogInformation(CustomJson.json(uploadResult, CustomJsonOptions.WriteIndented));
        
        return "OK";
    }
    
    // edit image with transformation (width, height, etc...) read more in Cloudinary SDK 
    public Object edit(EditRequest req)
    {
        var editParams = new ImageUploadParams()
        {
            File = new FileDescription(req.file[0].FileName, req.file[0].OpenReadStream()),
            PublicId = req.publicId,
            Transformation = String.IsNullOrEmpty(req.transformation) ? null : new Transformation().RawTransformation(req.transformation),
            Overwrite = true,
            Invalidate = true
        };
        var editResult = _cloundinary.Upload(editParams);
        _logger.LogInformation(CustomJson.json(editResult, CustomJsonOptions.WriteIndented));
        return "Ok";
    }

    public Object delete(string publicId)
    {
        var deletionParams = new DeletionParams(publicId);
        var deletionResult = _cloundinary.Destroy(deletionParams);
        _logger.LogInformation(CustomJson.json(deletionResult, CustomJsonOptions.WriteIndented));
        return "Ok";
    }
}