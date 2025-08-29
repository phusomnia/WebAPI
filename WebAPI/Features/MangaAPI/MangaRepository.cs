using WebAPI.Annotation;
using WebAPI.Core.shared;
using WebAPI.Entities;
using WebAPI.Infrastructure.Data;

namespace WebAPI.Features.MangaAPI;

[Repository]
public class MangaRepository : CrudRepository<Manga, String>
{
    public MangaRepository(AppDbContext context) : base(context)
    {
    }
}