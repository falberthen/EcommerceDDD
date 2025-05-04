namespace EcommerceDDD.Core.Infrastructure.Tests.Http;

public class FakeHttpMessageHandler : HttpMessageHandler
{
	private readonly HttpResponseMessage _fakeResponse;

	public FakeHttpMessageHandler(HttpResponseMessage fakeResponse)
	{
		_fakeResponse = fakeResponse;
	}

	protected override Task<HttpResponseMessage> SendAsync(
		HttpRequestMessage request,
		CancellationToken cancellationToken)
	{
		return Task.FromResult(_fakeResponse);
	}
}
