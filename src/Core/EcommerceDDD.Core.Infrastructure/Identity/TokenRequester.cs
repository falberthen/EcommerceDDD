using IdentityModel.Client;
using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;

namespace EcommerceDDD.Core.Infrastructure.Identity;

public class TokenRequester : ITokenRequester
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private const string _applicationKey = "ApplicationToken";

    public TokenRequester(
        IMemoryCache cache, 
        IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient();
        _cache = cache;
    }

    // Caching application token
    public async Task<TokenResponse> GetApplicationToken(TokenIssuerSettings settings)
    {
        TokenResponse tokenResponse = default!;
        var isStoredToken = _cache.TryGetValue(_applicationKey, out tokenResponse);

        if (!isStoredToken)        
            tokenResponse = await RequestApplicationToken(settings);      
        
        if (isStoredToken && IsTokenExpired(tokenResponse))
            tokenResponse = await RequestApplicationToken(settings);
        
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

    private async Task<TokenResponse> RequestApplicationToken(TokenIssuerSettings settings)
    {
        if (settings is null)
            throw new ArgumentNullException(nameof(settings));

        var identityServerAddress = $"{settings.Authority}/connect/token";
        var tokenResponse = await _httpClient
            .RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = identityServerAddress,
                ClientId = settings.ClientId,
                ClientSecret = settings.ClientSecret,
                Scope = settings.Scope
            });

        if (tokenResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
            _cache.Set(_applicationKey, tokenResponse);

        return tokenResponse;
    }

    private bool IsTokenExpired(TokenResponse tokenResponse)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = tokenHandler.ReadJwtToken(tokenResponse.AccessToken);

        if (jwtSecurityToken.ValidTo < DateTime.UtcNow.AddSeconds(10))
            return true;

        return false;
    }
}