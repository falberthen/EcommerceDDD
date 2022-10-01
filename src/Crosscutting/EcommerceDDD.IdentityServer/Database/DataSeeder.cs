using EcommerceDDD.IdentityServer.Configurations;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDDD.IdentityServer.Database;

public static class DataSeeder
{
    public static void SeedConfiguration(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices
            .GetService<IServiceScopeFactory>().CreateScope();

        serviceScope.ServiceProvider
            .GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

        var iS4configurationContext = serviceScope.ServiceProvider
            .GetRequiredService<ConfigurationDbContext>();

        iS4configurationContext.Database.Migrate();

        foreach (var client in IdentityConfiguration.Clients)
        {
            var item = iS4configurationContext
                .Clients.SingleOrDefault(c => c.ClientId == client.ClientId);

            if (item is null)
                iS4configurationContext.Clients.Add(client.ToEntity());
        }

        foreach (var resource in IdentityConfiguration.IdentityResources)
        {
            var item = iS4configurationContext
                .IdentityResources.SingleOrDefault(c => c.Name == resource.Name);

            if (item is null)
                iS4configurationContext.IdentityResources.Add(resource.ToEntity());
        }

        foreach (var scope in IdentityConfiguration.ApiScopes)
        {
            var item = iS4configurationContext
                .ApiScopes.SingleOrDefault(c => c.Name == scope.Name);

            if (item is null)
                iS4configurationContext.ApiScopes.Add(scope.ToEntity());
        }

        iS4configurationContext.SaveChanges();
    }
}

//https://docs.identityserver.io/en/3.1.0/quickstarts/4_entityframework.html