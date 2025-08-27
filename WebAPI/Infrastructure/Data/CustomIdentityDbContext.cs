using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebAPI.Features.Account;

namespace WebAPI.Context;

public class CustomIdentityDbContext : IdentityDbContext<IdentityUser>
{
    public CustomIdentityDbContext(DbContextOptions<CustomIdentityDbContext> options) : base(options){}
}