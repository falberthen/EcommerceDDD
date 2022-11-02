using Microsoft.EntityFrameworkCore;

namespace EcommerceDDD.Core.Infrastructure.Outbox.Persistence;

public sealed class OutboxDbContext : DbContext
{
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    public OutboxDbContext(DbContextOptions<OutboxDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OutboxDbContext).Assembly);
    }
}