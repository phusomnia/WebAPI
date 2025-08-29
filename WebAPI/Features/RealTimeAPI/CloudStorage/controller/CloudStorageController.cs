using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mime;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Features.RealTimeAPI.CloudStorage.dto;
using WebAPI.Features.RealTimeAPI.CloudStorage.service;

namespace WebAPI.Features.RealTimeAPI.CloudStorage.controller;

[ApiController]
[Route("/api/v1/cloud-storage/")]
public class CloudStorageController : ControllerBase
{
    private readonly CloudinaryConfig _config;
    private readonly CloudStorageService _service;
    
    public CloudStorageController(
        CloudinaryConfig config,
        CloudStorageService service
        )
    {
        _config  = config;
        _service = service;
    }

    [HttpGet("check-connection")]
    public ActionResult checkConnection()
    {
        Cloudinary cloudninary = _config.cloudinary();
        PingResult? conn = cloudninary.Ping();
        var result = conn.StatusCode == HttpStatusCode.OK ? "Connected" : throw new Exception("Something wrong...");
        return Ok(result);
    }

    [HttpPost("post")]
    // [Consumes("multipart/form-data")]
    public ActionResult uploadImageAPI(UploadRequest req)
    {
        try
        {
            if (req.file.Count == 0) return BadRequest("No files uploaded.");
            if(!req.file[0].ContentType.Contains("image")) return BadRequest("File is not image");
            
            var result = _service.upload(req);
            
            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("edit")]
    public ActionResult editImageAPI(EditRequest req)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.Values);
            var response = _service.edit(req);
            return Ok(response);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete]
    public ActionResult deleteImageAPI([FromQuery] DeleteRequest req)
    {
        try
        {
            if (String.IsNullOrEmpty(req.publicId)) return BadRequest("public id is required");
            
            var result = _service.delete(req);
            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}

