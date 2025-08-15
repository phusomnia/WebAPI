using WebAPI.Annotation;
using WebAPI.Context;
using WebAPI.Repositories;

namespace WebAPI.Features.Manga;

[Repository]
public class MangaRepository : CrudRepository<Entities.Manga, string>
{
    public MangaRepository(AppDbContext context) : base(context) { }
}