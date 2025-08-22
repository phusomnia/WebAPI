using System.Net.Mime;
using WebAPI.Features.RealTimeAPI.CloudStorage.controller;
using WebAPI.Features.RealTimeAPI.CloudStorage.dto;

namespace WebAPI.Features.RealTimeAPI.CloudStorage;

public interface CloudStorage
{
    Object upload(IFormFile fileName);
    Object edit(EditRequest req);
    Object delete(String publicId);
}