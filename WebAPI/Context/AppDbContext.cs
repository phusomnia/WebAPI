using Microsoft.EntityFrameworkCore;
using WebAPI.Entities;

namespace WebAPI.Context;

public partial class AppDbContext : DbContext
{
    private readonly string _connectionString;

    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : base(options)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString))
            .UseSnakeCaseNamingConvention();
    
    public virtual DbSet<Account> Account { get; set; }

    public virtual DbSet<Manga> Manga { get; set; }

    public virtual DbSet<RefreshToken> RefreshToken { get; set; }
}