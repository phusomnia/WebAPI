using Microsoft.EntityFrameworkCore;

namespace WebAPI.Context;

public partial class AppDbContext : DbContext
{
    private readonly string _connectionString;

    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : base(options)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
    }
}