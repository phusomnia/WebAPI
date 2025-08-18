using WebAPI.Annotation;
using WebAPI.Context;
using WebAPI.Repositories;

namespace WebAPI.Features.Account;

[Repository]
public class AccountRepository : CrudRepository<Entities.Account, string>
{   
    private readonly AppDbContext _context;

    public AccountRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    
    public Entities.Account? findByUsername(string username)
    {
        return _context.Account.FirstOrDefault(a => a.Username == username);
    }
}