using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using EcommerceDDD.IdentityServer.Models;

namespace EcommerceDDD.IdentityServer.Database;

public class IdentityApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public IdentityApplicationDbContext(DbContextOptions<IdentityApplicationDbContext> options)
        : base(options)
    {
        Database.Migrate();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}

//https://blog.devgenius.io/identityserver4-authentication-with-asp-net-identity-for-user-management-6449bb985d21
//https://deblokt.com/2020/01/24/03-identityserver4-ef-with-postgresql-net-core-3-1/