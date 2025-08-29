using WebAPI.Annotation;
using WebAPI.Core.shared;
using WebAPI.Entities;
using WebAPI.Infrastructure.Data;

namespace WebAPI.Features.PolicyAPI;

[Repository]
public class PermissionRepository : CrudRepository<Permission, String>
{
    private readonly AppDbContext _context;
    
    public PermissionRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ICollection<Dictionary<String, Object>>> getPermissionByUserId(String id)
    {
        Console.WriteLine(id);
        var query = @"SELECT p.name
                    FROM dotnet.Account acc
                    LEFT JOIN dotnet.Role r ON r.id = acc.roleId
                    LEFT JOIN dotnet.RolePermission rp ON rp.roleId = r.id
                    LEFT JOIN dotnet.Permission p ON p.id = rp.permissionId
                    WHERE acc.id = '5516f109-f2e6-4014-b415-cdad01c6258d'";
        return await _context.executeSqlRawAsync(query);
    }
}