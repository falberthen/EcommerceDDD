namespace EcommerceDDD.Core.Infrastructure.Tests.Http;

public class TokenRequesterTests
{
    [Fact]
    public async Task GetApplicationToken_ShouldReturnStatusCodeOK()
    {
        // Given
        await SetupMessageHandlerResponseAsync();
        var httpClient = new HttpClient(_mockHttpMessageHandler.Object);

        _httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);
            
        var tokenRequester = new TokenRequester(_cache.Object, _httpClientFactory.Object);

            // IMemoryCache.Set is an extension method using CreateEntry
        _cache.Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns(Mock.Of<ICacheEntry>);

        // When
        var response = await tokenRequester
            .GetApplicationTokenAsync(new TokenIssuerSettings() { Authority = _url });

        // Then
        Assert.NotNull(response);
        response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetUserToken_ShouldReturnStatusCodeOK()
    {
        // Given
        await SetupMessageHandlerResponseAsync();
        var httpClient = new HttpClient(_mockHttpMessageHandler.Object);

        _httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        var tokenRequester = new TokenRequester(_cache.Object, _httpClientFactory.Object);

        // When
        var response = await tokenRequester.GetUserTokenAsync(new TokenIssuerSettings() { Authority = _url }, 
            "username", "password");

        // Then
        Assert.NotNull(response);
        response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
    }


    private Task SetupMessageHandlerResponseAsync()
    {
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });
        return Task.CompletedTask;
    }

    private const string _url = "http://url";
    private Mock<IHttpClientFactory> _httpClientFactory = new();
    private Mock<IMemoryCache> _cache = new();
    private Mock<HttpMessageHandler> _mockHttpMessageHandler = new();

    public record DummyResponse(string AccessToken);
}
