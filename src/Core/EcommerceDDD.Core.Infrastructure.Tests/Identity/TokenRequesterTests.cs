using Microsoft.Extensions.Options;

namespace EcommerceDDD.Core.Infrastructure.Tests.Http;

public class TokenRequesterTests
{
	[Fact]
	public async Task GetApplicationToken_ShouldReturnStatusCodeOK()
	{
		// Given
		var dummyResponse = JsonConvert
			.SerializeObject(new IntegrationHttpResponse() { Success = true });

		var messageHandler = new MockHttpMessageHandler(dummyResponse, HttpStatusCode.OK);
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
		response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
	}

	[Fact]
	public async Task GetUserToken_ShouldReturnStatusCodeOK()
	{
		// Given
		var dummyResponse = JsonConvert
			.SerializeObject(new IntegrationHttpResponse() { Success = true });

		var messageHandler = new MockHttpMessageHandler(dummyResponse, HttpStatusCode.OK);
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
		response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
	}

	private Uri _url = new Uri("http://test.com");
	private IHttpClientFactory _httpClientFactory = Substitute.For<IHttpClientFactory>();
	private IMemoryCache _cache = Substitute.For<IMemoryCache>();
	private IHttpContextAccessor _contextAccessor = Substitute.For<IHttpContextAccessor>();
	private IOptions<TokenIssuerSettings> _options = Substitute.For<IOptions<TokenIssuerSettings>>();
	
}
