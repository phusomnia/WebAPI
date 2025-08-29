using WebAPI.Annotation;
using WebAPI.Core.shared;
using WebAPI.Entities;
using WebAPI.Infrastructure.Data;

namespace WebAPI.Features.AuthAPI.RefreshToken;

[Repository]
public class RefreshTokenRepository : CrudRepository<Entities.RefreshToken, String>
{
    private readonly DotnetContext _context;
    
    public RefreshTokenRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    
    public Entities.RefreshToken findByToken(string token)
    {
        return _context.RefreshTokens.FirstOrDefault(t => t.Token == token) ?? throw new KeyNotFoundException("Token not found");
    }
}