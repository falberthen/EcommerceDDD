using EcommerceDDD.Core.Exceptions.Types;

namespace EcommerceDDD.Core.Infrastructure.Identity;

public class TokenRequester(
	IMemoryCache cache,
	IHttpContextAccessor httpContextAccessor,
	IOptions<TokenIssuerSettings> tokenIssuerSettings,
	IHttpClientFactory factory) : ITokenRequester
{
	private readonly TokenIssuerSettings _tokenIssuerSettings = tokenIssuerSettings.Value
		?? throw new ArgumentNullException(nameof(tokenIssuerSettings));

	private readonly HttpClient _httpClient = factory.CreateClient();
	private readonly IMemoryCache _cache = cache;
	private readonly IHttpContextAccessor _contextAccessor = httpContextAccessor;

	private const string _applicationKey = "ApplicationToken";
	private const string _userAccessTokenKey = "access_token";

	// Caching application token
	public async Task<TokenResponse?> GetApplicationTokenAsync()
	{
		TokenResponse? tokenResponse = default!;
		var isStoredToken = _cache.TryGetValue(_applicationKey, out tokenResponse);

		if (!isStoredToken)
			tokenResponse = await RequestApplicationTokenAsync(_tokenIssuerSettings)
				?? throw new InvalidOperationException("No active HttpContext found.");

		if (isStoredToken && IsTokenExpired(tokenResponse!))
			tokenResponse = await RequestApplicationTokenAsync(_tokenIssuerSettings);

		return tokenResponse;
	}

	public async Task<TokenResponse?> GetUserTokenFromCredentialsAsync(string userName, string password)
	{
		var identityServerAddress = $"{_tokenIssuerSettings.Authority}/connect/token";
		TokenResponse response = await _httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
		{
			Address = identityServerAddress,
			ClientId = _tokenIssuerSettings.ClientId,
			ClientSecret = _tokenIssuerSettings.ClientSecret,
			Scope = _tokenIssuerSettings.Scope,
			GrantType = "password",
			UserName = userName,
			Password = password
		})
		?? throw new RecordNotFoundException($"Cannot retrieve token with given credentials.");

		return response;
	}

	public async Task<string?> GetUserTokenFromHttpContextAsync()
	{
		var context = _contextAccessor?.HttpContext
			?? throw new InvalidOperationException("No active HttpContext found.");

		return await context.GetTokenAsync(_userAccessTokenKey);
	}

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