using WebAPI.Annotation;
using WebAPI.Entities;

namespace WebAPI.Features.MangaAPI;

[Service]
public class MangaService
{
    private readonly MangaRepository _mangaRepository;

    public MangaService(MangaRepository mangaRepository)
    {
        _mangaRepository = mangaRepository;
    }

    public Manga? getManga(string mangaId)
    {
        return _mangaRepository.findById(mangaId);
    }
    
    public Object uploadManga(MangaDTO manga)
    {
        Manga m = new Manga();
        m.Id = Guid.NewGuid().ToString();
        m.Title = manga.Title;
        var affectedRows = _mangaRepository.add(m);
        if(affectedRows < 0) throw new Exception($"Upload fail");
        return m;
    }

    public Object editManga(String id, MangaDTO dto)
    {
        var m = _mangaRepository.findById(id);
        m.Title = dto.Title;
        var affectedRows = _mangaRepository.update(m);
        if(affectedRows < 0) throw new ApplicationException("Manga not found");
        return m;
    }

    public Object deleteManga(String id)
    {
        var m = _mangaRepository.findById(id);
        var affectedRows = _mangaRepository.delete(m);
        if(affectedRows < 0) throw new ApplicationException("Manga not found");
        return m;
    }
}