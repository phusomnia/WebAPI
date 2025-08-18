using WebAPI.Annotation;

namespace WebAPI.Features.Manga;

[Service]
public class MangaService
{
    private readonly MangaRepository _mangaRepository;

    public MangaService(MangaRepository mangaRepository)
    {
        _mangaRepository = mangaRepository;
    }

    public dynamic getManga(string mangaId)
    {
        return _mangaRepository.FindById(mangaId);
    }
    
    public dynamic uploadManga(MangaDTO manga)
    {
        // Entities.Manga m = new Entities.Manga();
        // m.Id = Guid.NewGuid().ToString();
        // m.Title = manga.Title;
        // if(_mangaRepository.Add(m) < 0) throw new Exception($"Upload fail");
        return "";
    }

    public dynamic editManga(String id, MangaDTO dto)
    {
        // var m = _mangaRepository.FindById(id);
        // m.Title = dto.Title;
        // if(_mangaRepository.Update(m) < 0) throw new ApplicationException("Manga not found");
        return "";
    }

    public dynamic deleteManga(String id)
    {
        var m = _mangaRepository.FindById(id);
        if(_mangaRepository.Delete(m) < 0) throw new ApplicationException("Manga not found");
        return m;
    }
}