using WebAPI.Annotation;
using WebAPI.Context;
using WebAPI.Shared;

namespace WebAPI.Features.AuthService.RefreshToken;

[Repository]
public class RefreshTokenRepository : CrudRepository<Entities.RefreshToken, String>
{
    private readonly AppDbContext _context;
    
    public RefreshTokenRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public Entities.RefreshToken findByToken(string token)
    {
        return _context.RefreshToken.FirstOrDefault(t => t.Token == token) ?? throw new KeyNotFoundException("Token not found");
    }
}