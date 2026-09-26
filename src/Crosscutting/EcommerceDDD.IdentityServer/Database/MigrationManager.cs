namespace EcommerceDDD.IdentityServer.Database;

public static class MigrationManager
{
    public static async Task MigrateDatabaseAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();

		await scope.ServiceProvider
			.GetRequiredService<IdentityApplicationDbContext>().Database.MigrateAsync();
		await scope.ServiceProvider
			.GetRequiredService<PersistedGrantDbContext>().Database.MigrateAsync();

		// Seeded once here so registering a user never has to create it.
		var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
		if (!await roleManager.RoleExistsAsync(Roles.Customer))
			await roleManager.CreateAsync(new IdentityRole(Roles.Customer));

		var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
		await context.Database.MigrateAsync();

		if (!await context.Clients.AnyAsync())
		{
			foreach (var client in IdentityConfiguration.Clients)
				context.Clients.Add(client.ToEntity());

			await context.SaveChangesAsync();
		}

		if (!await context.IdentityResources.AnyAsync())
		{
			foreach (var resource in IdentityConfiguration.IdentityResources)
				context.IdentityResources.Add(resource.ToEntity());

			await context.SaveChangesAsync();
		}

		if (!await context.ApiResources.AnyAsync())
		{
			foreach (var resource in IdentityConfiguration.ApiResources)
				context.ApiResources.Add(resource.ToEntity());

			await context.SaveChangesAsync();
		}

		if (!await context.ApiScopes.AnyAsync())
		{
			foreach (var apiScope in IdentityConfiguration.ApiScopes)
				context.ApiScopes.Add(apiScope.ToEntity());

			await context.SaveChangesAsync();
		}
    }
}

//https://code-maze.com/migrate-identityserver4-configuration-to-database/
