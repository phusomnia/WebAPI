using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Context;

namespace WebAPI.Features.Manga;

[ApiController]
[Route("api/v1/[controller]")]
public class MangaController : ControllerBase
{
    private MangaService _mangaService;

    public MangaController(MangaService mangaService)
    {
        _mangaService = mangaService;
    }

    [HttpGet]
    public IActionResult getMangaAPI(
        String id
    )
    {
        var res = _mangaService.getManga(id);
        return Ok(res);
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