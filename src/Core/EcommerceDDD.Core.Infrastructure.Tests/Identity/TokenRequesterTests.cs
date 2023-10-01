using EcommerceDDD.Core.Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

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

        var tokenRequester = new TokenRequester(
            _cache, _contextAccessor, _httpClientFactory);

        _cache.CreateEntry(Arg.Any<object>())
            .Returns(Substitute.For<ICacheEntry>());

        // When
        var response = await tokenRequester
            .GetApplicationTokenAsync(new TokenIssuerSettings() { Authority = _url.AbsoluteUri });

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

        var tokenRequester = new TokenRequester(
            _cache, _contextAccessor, _httpClientFactory);

        // When
        var response = await tokenRequester.GetUserTokenAsync(
            new TokenIssuerSettings() { Authority = _url.AbsoluteUri },
            "username", "password");

        // Then
        Assert.NotNull(response);
        response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
    }

    private Uri _url = new Uri("http://test.com");
    private IHttpClientFactory _httpClientFactory = Substitute.For<IHttpClientFactory>();
    private IMemoryCache _cache = Substitute.For<IMemoryCache>();
    private IHttpContextAccessor _contextAccessor = Substitute.For<IHttpContextAccessor>();
}
