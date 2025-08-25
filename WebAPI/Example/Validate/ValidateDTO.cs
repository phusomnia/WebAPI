using System.ComponentModel.DataAnnotations;

namespace WebAPI.Example.Validate;

public class ValidateDTO
{
    [MangaTitle] 
    public String title { get; set; }
    
    [Volume(1)] 
    public int volume { get; set; }
    
    public NestedDTO test { get; set; }
}

public class NestedDTO
{
    [Required] 
    public String value1 { get; set; } 
}