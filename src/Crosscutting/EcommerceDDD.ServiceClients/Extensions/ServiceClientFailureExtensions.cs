namespace EcommerceDDD.ServiceClients.Extensions;

public static class ServiceClientFailureExtensions
{
	/// <summary>
	/// Dead-letters a message at once when another service rejects its request with a 4xx.
	/// </summary>
	public static WolverineOptions DeadLetterServiceClientRejections(this WolverineOptions options)
	{
		options.Policies
			.OnException<ApiException>(e => IsRejection((HttpStatusCode)e.ResponseStatusCode))
			.MoveToErrorQueue();

		return options;
	}

	// Request timeout and too many requests are temporary, so they keep the default retries.
	private static bool IsRejection(HttpStatusCode statusCode) =>
		statusCode is >= HttpStatusCode.BadRequest and < HttpStatusCode.InternalServerError
			and not HttpStatusCode.RequestTimeout
			and not HttpStatusCode.TooManyRequests;
}
