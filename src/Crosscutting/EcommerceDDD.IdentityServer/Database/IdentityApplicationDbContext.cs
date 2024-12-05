namespace EcommerceDDD.IdentityServer.Database;

public class IdentityApplicationDbContext : IdentityDbContext<ApplicationUser>
{
	public IdentityApplicationDbContext(DbContextOptions<IdentityApplicationDbContext> options)
		: base(options) {}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
	}
}
