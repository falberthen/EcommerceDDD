namespace EcommerceDDD.Core.Infrastructure.Tests.Http;

public class MockHttpMessageHandler(string response, HttpStatusCode statusCode) : HttpMessageHandler
{
	private readonly string _response = response
		?? throw new ArgumentException(response);
	private readonly HttpStatusCode _statusCode = statusCode;

	public string Input { get; private set; } = string.Empty;
	public int NumberOfCalls { get; private set; }

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
		CancellationToken cancellationToken)
	{
		NumberOfCalls++;
		if (request.Content != null) // Could be a GET-request without a body
		{
			Input = await request.Content.ReadAsStringAsync();
		}
		return new HttpResponseMessage
		{
			StatusCode = _statusCode,
			Content = new StringContent(_response)
		};
	}
}

//https://dev.to/n_develop/mocking-the-httpclient-in-net-core-with-nsubstitute-k4j