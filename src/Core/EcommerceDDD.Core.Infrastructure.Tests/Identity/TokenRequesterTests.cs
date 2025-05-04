namespace EcommerceDDD.Core.Infrastructure.Tests.Http;

public class TokenRequesterTests
{
	[Fact]
	public async Task GetApplicationToken_ShouldReturnStatusCodeOK()
	{
		// Given
		var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
		{
			Content = new StringContent(
				"access_token=" + CreateTestToken() + "&expires_in=3600&token_type=Bearer",
				Encoding.UTF8,
				"application/x-www-form-urlencoded")
		};

		var messageHandler = new FakeHttpMessageHandler(responseMessage);
		var httpClient = new HttpClient(messageHandler)
		{
			BaseAddress = _url
		};
		_httpClientFactory.CreateClient(Arg.Any<string>())
			.Returns(httpClient);
		_options.Value.Returns(new TokenIssuerSettings() { Authority = _url.AbsoluteUri });

		var tokenRequester = new TokenRequester(
			_cache, _contextAccessor,
			_options,
			_httpClientFactory);

		_cache.CreateEntry(Arg.Any<object>())
			.Returns(Substitute.For<ICacheEntry>());

		// When
		var response = await tokenRequester
			.GetApplicationTokenAsync();

		// Then
		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.OK, response.HttpStatusCode);
	}

	[Fact]
	public async Task GetUserToken_ShouldReturnStatusCodeOK()
	{
		// Given
		var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
		{
			Content = new StringContent(
				"access_token=" + CreateTestToken() + "&expires_in=3600&token_type=Bearer",
				Encoding.UTF8,
				"application/x-www-form-urlencoded")
		};

		var messageHandler = new FakeHttpMessageHandler(responseMessage);
		var httpClient = new HttpClient(messageHandler)
		{
			BaseAddress = _url
		};
		_httpClientFactory.CreateClient(Arg.Any<string>())
			.Returns(httpClient);
		_options.Value.Returns(new TokenIssuerSettings() { Authority = _url.AbsoluteUri });

		var tokenRequester = new TokenRequester(
			_cache, _contextAccessor, _options, _httpClientFactory);

		// When
		var response = await tokenRequester.GetUserTokenFromCredentialsAsync(
			"username", "password");

		// Then
		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.OK, response.HttpStatusCode);
	}

	private Uri _url = new Uri("http://test.com");
	private IHttpClientFactory _httpClientFactory = Substitute.For<IHttpClientFactory>();
	private IMemoryCache _cache = Substitute.For<IMemoryCache>();
	private IHttpContextAccessor _contextAccessor = Substitute.For<IHttpContextAccessor>();
	private IOptions<TokenIssuerSettings> _options = Substitute.For<IOptions<TokenIssuerSettings>>();

	private static string CreateTestToken()
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var token = new JwtSecurityToken(
			issuer: "test",
			audience: "test",
			expires: DateTime.UtcNow.AddMinutes(30),
			signingCredentials: null
		);
		return tokenHandler.WriteToken(token);
	}
}
