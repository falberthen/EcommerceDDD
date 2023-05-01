namespace EcommerceDDD.IdentityServer.Configurations;

public class IdentityConfiguration
{
    public const string CustomerRole = "Customer";

    private const string _apiScope = "ecommerceddd-api.scope";    
    private const string _readScope = "read";
    private const string _writeScope = "write";
    private const string _deleteScope = "delete";

    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email()
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new List<ApiResource>
        {
            new ApiResource("ecommerceddd-api")
            {
                Scopes = new List<string> { 
                    _apiScope, 
                    _readScope, 
                    _writeScope,
                    _deleteScope
                }
            }
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope> {
            new ApiScope(_apiScope, "EcommerceDDD"),
            new ApiScope(name: _readScope,   displayName: "Read your data."),
            new ApiScope(name: _writeScope,  displayName: "Write your data."),
            new ApiScope(name: _deleteScope, displayName: "Delete your data."),
        };

    public static IEnumerable<Client> Clients =>
        new List<Client>
        {            
            // User's client
            new Client
            {
                ClientId = "ecommerceddd.user_client",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                RequireClientSecret = true,
                ClientSecrets = new List<Secret>
                {
                    new Secret("secret234554^&%&^%&^f2%%%".Sha256())
                },
                AllowedScopes = {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    _readScope,
                    _writeScope,
                    _deleteScope
                },
                AccessTokenLifetime = 86400
            },
            // machine to machine client
            new Client
            {
                ClientId = "ecommerceddd.application_client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                RequireClientSecret = true,
                ClientSecrets = new List<Secret> { new Secret("secret33587^&%&^%&^f3%%%".Sha256()) },
                AllowedScopes = new List<string>
                {
                    _apiScope,
                    _readScope,
                    _writeScope,
                    _deleteScope
                },
                AccessTokenLifetime = 86400
            },
        };  
}