using WebAPI.Annotation;
using WebAPI.Core.shared;
using WebAPI.Infrastructure.Data;
using WebAPI.Entities;

namespace WebAPI.Features.AccAPI;

[Repository]
public class AccountRepository : CrudRepository<Entities.Account, string>
{
    private readonly AppDbContext _context;

    public AccountRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    
    public Account? findByUsername(string username)
    {
        return _context.Accounts.FirstOrDefault(a => a.Username == username);
    }
    
    public async Task<ICollection<Dictionary<string, object>>> findByUsernameAndRole(string username)
    {
        var query = @"SELECT acc.id as id, acc.username, acc.passwordHash, r.name as roleName
                    FROM Account acc
                    LEFT JOIN Role r ON acc.roleId = r.id
                    WHERE acc.username = ?";
        var result = await _context.executeSqlRawAsync(query, username);
        return result;
    }
    
    public async Task<ICollection<Dictionary<String, Object>>> findByUsernameAndPermission(string username)
    {
        var query = @"SELECT p.name
                    FROM Account acc
                    LEFT JOIN Role r ON acc.roleId = r.id
                    LEFT JOIN RolePermission rp ON rp.roleId = r.id
                    LEFT JOIN Permission p ON rp.permissionId = p.id
                    WHERE acc.username = ?";
        var result = await _context.executeSqlRawAsync(query, username);
        return result;
    }
}