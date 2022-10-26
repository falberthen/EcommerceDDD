//https://identityserver4.readthedocs.io/en/latest/topics/deployment.html?highlight=operational%20store#configuration-data
using IdentityServer4;
using IdentityServer4.Models;

namespace EcommerceDDD.IdentityServer.Configurations;

public class IdentityConfiguration
{
    private const string api_scope = "ecommerceddd-api";

    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email()
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new[]
        {
            new ApiScope(api_scope, "EcommerceDDD API")
        };
    
    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // User's client
            new Client
            {
                ClientId = "ecommerceddd.userclient",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                RequireClientSecret = true,
                RequireConsent = false,
                ClientSecrets = new List<Secret>
                {
                    new Secret("secret234554^&%&^%&^f2%%%".Sha256())
                },
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    api_scope
                },
                AccessTokenLifetime = 86400
            },
            // machine to machine client
            new Client
            {
                ClientId = "ecommerceddd.applicationclient",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("secret33587^&%&^%&^f3%%%".Sha256())
                },
                AllowedScopes = { api_scope }
            }
        };    
}