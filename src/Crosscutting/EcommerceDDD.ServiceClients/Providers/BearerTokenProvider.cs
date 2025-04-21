using Duende.IdentityModel.Client;
using EcommerceDDD.Core.Infrastructure.Identity;
using Microsoft.Kiota.Abstractions.Authentication;

public class BearerTokenProvider : IAccessTokenProvider
{
	private readonly ITokenRequester _tokenRequester;

	public BearerTokenProvider(ITokenRequester tokenRequester)
	{
		_tokenRequester = tokenRequester;
	}

	public AllowedHostsValidator AllowedHostsValidator { get; }

	public async Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object> additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
	{
		TokenResponse? tokenResponse = await _tokenRequester
		   .GetApplicationTokenAsync();
		return tokenResponse?.AccessToken ?? string.Empty;
	}	
}