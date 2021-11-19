using EcommerceDDD.Infrastructure.Identity.Users;
using EcommerceDDD.Infrastructure.Identity.Roles;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace EcommerceDDD.Infrastructure.Database.Context;

public class IdentityContext : IdentityDbContext<ApplicationUser, UserRole, Guid>
{
    public IdentityContext(DbContextOptions<IdentityContext> options)
        : base(options)
    {
        Database.Migrate();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUser>(b =>
        {
            b.Property(u => u.Id)
                .HasDefaultValueSql("newsequentialid()");
        });

        modelBuilder.Entity<UserRole>(b =>
        {
            b.Property(u => u.Id)
                .HasDefaultValueSql("newsequentialid()");
        });

        base.OnModelCreating(modelBuilder);
    }
}