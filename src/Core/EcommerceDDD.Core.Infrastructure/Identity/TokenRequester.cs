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

	// Static so all transient instances share one lock (TokenRequester is registered as Transient).
	private static readonly SemaphoreSlim _refreshLock = new(1, 1);

	private const string _applicationKey = "application_token";
	private const string _userAccessTokenKey = "access_token";

	public async Task<TokenResponse?> GetApplicationTokenAsync()
	{
		// Fast path: valid token already cached.
		if (_cache.TryGetValue(_applicationKey, out TokenResponse? tokenResponse))
			return tokenResponse;

		// Slow path: serialize concurrent refreshes so only one thread hits IdentityServer.
		await _refreshLock.WaitAsync();
		try
		{
			// Double-check after acquiring the lock — another thread may have populated the cache.
			if (_cache.TryGetValue(_applicationKey, out tokenResponse))
				return tokenResponse;

			return await RequestApplicationTokenAsync(_tokenIssuerSettings)
				?? throw new InvalidOperationException("Failed to acquire application token.");
		}
		finally
		{
			_refreshLock.Release();
		}
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
		?? throw new InvalidOperationException($"Cannot retrieve token with given credentials.");

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
		{
			var options = new MemoryCacheEntryOptions();
			if (!string.IsNullOrEmpty(tokenResponse.AccessToken))
			{
				var jwt = new JwtSecurityTokenHandler().ReadJwtToken(tokenResponse.AccessToken);
				// Evict 60 s before the token actually expires to avoid clock-skew rejections.
				options.SetAbsoluteExpiration(jwt.ValidTo.AddSeconds(-60));
			}
			_cache.Set(_applicationKey, tokenResponse, options);
		}

		return tokenResponse;
	}
}