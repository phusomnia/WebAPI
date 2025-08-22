using WebAPI.Annotation;
using WebAPI.Features.RealTimeAPI.CloudStorage.controller;
using WebAPI.Features.RealTimeAPI.CloudStorage.dto;
using WebAPI.Features.Shared;

namespace WebAPI.Features.RealTimeAPI.CloudStorage.service;

[Service]
public class CloudStorageService
{
    private readonly CloudStorageFactory _cloudStorageFactory;

    public CloudStorageService(CloudStorageFactory cloudStorageFactory)
    {
        _cloudStorageFactory = cloudStorageFactory;
    }

    public Object upload(UploadRequest req)
    {
        var instance = _cloudStorageFactory.createService(req.provider);
        return instance.upload(req.file[0]);
    }

    public Object edit(EditRequest req)
    {
        Console.WriteLine($"{CustomJson.json(req.file)}\n{req.publicId}\n{req.transformation}");
        var instance = _cloudStorageFactory.createService(req.cloudProvider);
        return instance.edit(req);
    }

    public Object delete(DeleteRequest req)
    {
        var instance = _cloudStorageFactory.createService(req.provider);
        return instance.delete(req.publicId);
    }
}