using WebAPI.Annotation;

namespace WebAPI.Features.RealTimeAPI.CloudStorage;

[Service]
public class CloudStorageFactory
{
    private readonly CloudinaryConfig _cloudinaryConfig;

    public CloudStorageFactory(
        CloudinaryConfig cloudinaryConfig
    )
    {
        _cloudinaryConfig = cloudinaryConfig;
    }

    public CloudStorage createService(CloudProvider provider)
    {
        switch (provider)
        {
            case CloudProvider.Cloudinary:
                return new CloudinaryStorage(_cloudinaryConfig);
            default:
                throw new ArgumentException("Unknown cloud" + provider);
        }
    }

}