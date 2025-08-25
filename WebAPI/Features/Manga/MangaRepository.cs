using WebAPI.Annotation;
using WebAPI.Context;
using WebAPI.Shared;

namespace WebAPI.Features.Manga;

[Repository]
public class MangaRepository : CrudRepository<Entities.Manga, string>
{
    public MangaRepository(AppDbContext context) : base(context) { }
}