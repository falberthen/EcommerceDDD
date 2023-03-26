using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace EcommerceDDD.IdentityServer.Configurations;

public class IdentityConfiguration
{
    private const string _apiDescription = "EcommerceDDD API";
    private const string _apiScope = "ecommerceddd-api.scope";

    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email()
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        {
            new ApiResource("ecommerceddd-api", _apiDescription)
            {
                Scopes = new List<string> { _apiScope },
                Description = "Allow the application to access Weather Api on your behalf",
                ApiSecrets = new List<Secret> {new Secret("ProCodeGuide".Sha256())},
            }
        };


    public static IEnumerable<ApiScope> ApiScopes =>
        new[]
        {
            new ApiScope(_apiScope, "EcommerceDDD API Scope"),
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {            
            // machine to machine client
            new Client
            {
                ClientId = "ecommerceddd.applicationclient",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = new List<Secret> { new Secret("secret33587^&%&^%&^f3%%%".Sha256()) },
                AllowedScopes = new List<string> { _apiScope }
            },
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
                    _apiScope
                },
                AccessTokenLifetime = 86400,
                RequirePkce = true,
                AllowPlainTextPkce = false
            },
        };    
}