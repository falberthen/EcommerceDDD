using IdentityModel.Client;
using Microsoft.Extensions.Caching.Memory;

namespace EcommerceDDD.Core.Infrastructure.Identity;

public class TokenRequester : ITokenRequester
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;

    public TokenRequester(
        IMemoryCache cache, 
        IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient();
        _cache = cache;
    }

    public async Task<TokenResponse> GetApplicationToken(TokenIssuerSettings settings)
    {        
        if (settings == null)
            throw new ArgumentNullException(nameof(settings));

        const string applicationKey = "ApplicationToken";

        // Caching application token
        TokenResponse tokenResponse = default;

        if (!_cache.TryGetValue(applicationKey, out tokenResponse))
        {
            var identityServerAddress = $"{settings.Authority}/connect/token";
            tokenResponse = await _httpClient
                .RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = identityServerAddress,
                    ClientId = settings.ClientId,
                    ClientSecret = settings.ClientSecret,
                    Scope = settings.Scope
                });

            if(tokenResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                _cache.Set(applicationKey, tokenResponse);
        }

        return tokenResponse;       
    }

    public async Task<TokenResponse> GetUserToken(TokenIssuerSettings settings, string userName, string password)
    {
        var identityServerAddress = $"{settings.Authority}/connect/token";
        var response = await _httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
        {
            Address = identityServerAddress,
            ClientId = settings.ClientId,
            ClientSecret = settings.ClientSecret,
            Scope = settings.Scope,
            GrantType = "password",
            UserName = userName,
            Password = password
        });

        return response;
    }
}