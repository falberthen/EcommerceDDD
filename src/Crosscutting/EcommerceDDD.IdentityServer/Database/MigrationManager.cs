namespace EcommerceDDD.IdentityServer.Database;

public static class MigrationManager
{
    public static IHost MigrateDatabase(this IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

            using (var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>())
            {
                try
                {
                    context.Database.Migrate();

                    if (!context.Clients.Any())
                    {
                        foreach (var client in IdentityConfiguration.Clients)
                            context.Clients.Add(client.ToEntity());

                        context.SaveChanges();
                    }

                    if (!context.IdentityResources.Any())
                    {
                        foreach (var resource in IdentityConfiguration.IdentityResources)
                            context.IdentityResources.Add(resource.ToEntity());

                        context.SaveChanges();
                    }

                    if (!context.ApiResources.Any())
                    {
                        foreach (var resource in IdentityConfiguration.ApiResources)
                            context.ApiResources.Add(resource.ToEntity());

                        context.SaveChanges();
                    }

                    if (!context.ApiScopes.Any())
                    {
                        foreach (var apiScope in IdentityConfiguration.ApiScopes)
                            context.ApiScopes.Add(apiScope.ToEntity());

                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        return host;
    }
}

//https://code-maze.com/migrate-identityserver4-configuration-to-database/