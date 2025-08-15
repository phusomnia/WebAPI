using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Features.Manga;

[ApiController]
[Route("api/v1/[controller]")]
public class MangaController : ControllerBase
{
    
    [HttpGet]
    public async Task<ActionResult> getMangaAPI()
    {
        await Task.Delay(10);
        return Ok("Get manga");
    }
    
    [HttpPost]
    public async Task<ActionResult<dynamic>> uploadMangaAPI()
    {
        await Task.Delay(10);
        return Ok("Upload manga");
    }
    
    [HttpPut]
    public async Task<ActionResult<dynamic>> editMangaAPI()
    {
        await Task.Delay(10);
        return Ok("Edit manga");
    }
    
    [HttpDelete]
    public async Task<ActionResult<dynamic>> deleteMangaAPI()
    {
        await Task.Delay(10);
        return Ok("Delete Manga");
    }
}