using WebAPI.Annotation;

namespace WebAPI.Features.Manga;

[Service]
public class MangaService
{
    private readonly MangaRepository _mangaRepository;

    public dynamic getManga(string mangaId)
    {
        return _mangaRepository.FindById(mangaId);
    }
}