namespace EcommerceDDD.IdentityServer.Database;

public class IdentityApplicationDbContext(
	DbContextOptions<IdentityApplicationDbContext> options)
	: IdentityDbContext<ApplicationUser>(options), IDataProtectionKeyContext
{
	public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
	}
}
