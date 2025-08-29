using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Features.MangaAPI;

[ApiController]
[Route("api/v1/manga/")]
public class MangaController : ControllerBase
{
    private MangaService _mangaService;

    public MangaController(MangaService mangaService)
    {
        _mangaService = mangaService;
    }

    [HttpGet]
    public async Task<IActionResult> getMangaAPI(String id)
    {
        var res = _mangaService.getManga(id);
        return Ok(res);
    }
    
    [HttpPost]
    public async Task<ActionResult> uploadMangaAPI(
        [FromBody] MangaDTO dto    
    )
    {
        var res = _mangaService.uploadManga(dto);
        return Ok(res);
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult> editMangaAPI(
        [FromBody] MangaDTO dto
    )
    {
        var id = RouteData.Values["id"]?.ToString();
        Console.WriteLine(id);
        var res = _mangaService.editManga(id, dto);
        return Ok("Manga updated");
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> deleteMangaAPI()
    {
        var id = RouteData.Values["id"]?.ToString();
        _mangaService.deleteManga(id);
        return Ok("Manga deleted");
    }
}