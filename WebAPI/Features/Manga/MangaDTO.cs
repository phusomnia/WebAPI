using System.ComponentModel.DataAnnotations;

namespace WebAPI.Features.Manga;

public class MangaDTO
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 255 chars")]
    public string Title { get; set; } = null!;
}