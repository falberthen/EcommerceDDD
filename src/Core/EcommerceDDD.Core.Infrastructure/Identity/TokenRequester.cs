﻿using Microsoft.AspNetCore.Authentication;

namespace EcommerceDDD.Core.Infrastructure.Identity;

public class TokenRequester(
    IMemoryCache cache,
    IHttpContextAccessor httpContextAccessor,
    IHttpClientFactory factory) : ITokenRequester
{
    private readonly HttpClient _httpClient = factory.CreateClient();
    private readonly IMemoryCache _cache = cache;
    private readonly IHttpContextAccessor _contextAccessor = httpContextAccessor;

    private const string _applicationKey = "ApplicationToken";

    // Caching application token
    public async Task<TokenResponse> GetApplicationTokenAsync(TokenIssuerSettings settings)
    {
        TokenResponse tokenResponse = default!;
        var isStoredToken = _cache.TryGetValue(_applicationKey, out tokenResponse);

        if (!isStoredToken)        
            tokenResponse = await RequestApplicationTokenAsync(settings);      
        
        if (isStoredToken && IsTokenExpired(tokenResponse))
            tokenResponse = await RequestApplicationTokenAsync(settings);
        
        return tokenResponse;       
    }

    public async Task<TokenResponse> GetUserTokenAsync(TokenIssuerSettings settings, string userName, string password)
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

    public async Task<string> GetUserTokenFromHttpContextAsync() =>
        await _contextAccessor.HttpContext?.GetTokenAsync("access_token");

    private async Task<TokenResponse> RequestApplicationTokenAsync(TokenIssuerSettings settings)
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