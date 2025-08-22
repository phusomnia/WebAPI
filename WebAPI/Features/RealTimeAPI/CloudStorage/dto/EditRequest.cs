using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Features.RealTimeAPI.CloudStorage.dto;

public class EditRequest
{
    [DefaultValue(CloudProvider.Aws)]
    public CloudProvider cloudProvider { get; set; }
    
    [Required(ErrorMessage = "public id is required")]
    public string publicId { get; set; }
    
    public string transformation { get; set; }
    
    public List<IFormFile> file { get; set; }
}