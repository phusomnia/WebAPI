using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Infrastructure.Data;

public class CustomIdentityDbContext : IdentityDbContext<IdentityUser>
{
    public CustomIdentityDbContext(DbContextOptions<CustomIdentityDbContext> options) : base(options){}
}