namespace EcommerceDDD.Core.Infrastructure.Tests.Http;

public class HttpRequesterTests
{
    [Fact]
    public async Task GetAsync_ShouldReturnResponse()
    {
        // Given
        var aggregateId = new DummyAggregateId(Guid.NewGuid());
        var dummyResponse = new List<DummyAggregateRoot>()
        {
            new DummyAggregateRoot(aggregateId),
            new DummyAggregateRoot(aggregateId),
            new DummyAggregateRoot(aggregateId),
        };

        SetupMessageHandlerResponse(
            new StringContent(JsonConvert.SerializeObject(dummyResponse))
        );

        var client = new HttpClient(_mockHttpMessageHandler.Object);
        _httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>()))
            .Returns(client);

        var requester = new HttpRequester(_httpClientFactory.Object);

        // When
        var response = await requester.GetAsync<List<DummyAggregateRoot>>(_url);

        // Then
        Assert.NotNull(response);
        response!.Count.Should().Be(dummyResponse.Count);
    }

    [Fact]
    public async Task PostAsync_ShouldReturnResponse()
    {
        // Given        
        SetupMessageHandlerResponse(
            new StringContent(JsonConvert.SerializeObject(new IntegrationHttpResponse() { Success = true }))
        );

        var client = new HttpClient(_mockHttpMessageHandler.Object);
        _httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>()))
            .Returns(client);

        var requester = new HttpRequester(_httpClientFactory.Object);

        // When
        var response = await requester.PostAsync<IntegrationHttpResponse>(_url, new object());

        // Then
        Assert.NotNull(response);
        response!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task PutAsync_ShouldReturnResponse()
    {
        // Given
        SetupMessageHandlerResponse(
            new StringContent(JsonConvert.SerializeObject(new IntegrationHttpResponse() { Success = true }))
        );

        var client = new HttpClient(_mockHttpMessageHandler.Object);
        _httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>()))
            .Returns(client);

        var requester = new HttpRequester(_httpClientFactory.Object);

        // When
        var response = await requester.PutAsync<IntegrationHttpResponse>(_url, new object());

        // Then
        Assert.NotNull(response);
        response!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnResponse()
    {
        // Given
        SetupMessageHandlerResponse(
            new StringContent(JsonConvert.SerializeObject(new IntegrationHttpResponse() { Success = true }))
        );

        var client = new HttpClient(_mockHttpMessageHandler.Object);
        _httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>()))
            .Returns(client);

        var requester = new HttpRequester(_httpClientFactory.Object);

        // When
        var response = await requester.DeleteAsync<IntegrationHttpResponse>(_url);

        // Then
        Assert.NotNull(response);
        response!.Success.Should().BeTrue();
    }

    private void SetupMessageHandlerResponse(dynamic content)
    {
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = content
            });
    }

    private const string _url = "http://url";
    private Mock<IHttpClientFactory> _httpClientFactory = new();
    private Mock<HttpMessageHandler> _mockHttpMessageHandler = new();
}


