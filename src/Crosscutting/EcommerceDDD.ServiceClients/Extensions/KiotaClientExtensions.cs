namespace EcommerceDDD.ServiceClients.Extensions;

public static class KiotaClientExtensions
{
	private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);
	private const int RetryCount = 3;
	private static readonly TimeSpan CircuitBreakerDuration = TimeSpan.FromSeconds(30);
	private const int CircuitBreakerThreshold = 5;

	/// <summary>
	/// Adds a Kiota generated client to the service collection with resilience policies.
	/// </summary>
	/// <typeparam name="TClient"></typeparam>
	/// <param name="services"></param>
	/// <param name="baseUrl"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="InvalidOperationException"></exception>
	public static IServiceCollection AddKiotaClient<TClient>(this IServiceCollection services, string? baseUrl)
		where TClient : class
	{
		if (string.IsNullOrEmpty(baseUrl))
			throw new ArgumentNullException(nameof(baseUrl), $"The base url for the client {typeof(TClient).Name} cannot be empty.");

		services.AddHttpClient<TClient>((serviceProvider, client) =>
		{
			client.BaseAddress = new Uri(baseUrl);
			client.Timeout = DefaultTimeout;
		})
		.AddPolicyHandler(GetRetryPolicy())
		.AddPolicyHandler(GetCircuitBreakerPolicy())
		.AddTypedClient<TClient>((httpClient, serviceProvider) =>
		{
			var tokenRequester = serviceProvider.GetRequiredService<ITokenRequester>();

			var tokenProvider = new BearerTokenProvider(tokenRequester);
			var authProvider = new BaseBearerTokenAuthenticationProvider(tokenProvider);

			var adapter = new HttpClientRequestAdapter(authProvider, httpClient: httpClient);

			var clientInstance = Activator.CreateInstance(typeof(TClient), adapter);

			if (clientInstance is null)
				throw new InvalidOperationException($"It was not possible to create an instance of {typeof(TClient).Name}.");

			return (TClient)clientInstance;
		});

		return services;
	}

	/// <summary>
	/// Gets the retry policy with exponential backoff.
	/// Retries on transient HTTP errors (5xx, 408, network failures).
	/// </summary>
	private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
	{
		return HttpPolicyExtensions
			.HandleTransientHttpError()
			.WaitAndRetryAsync(
				RetryCount,
				retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
	}

	/// <summary>
	/// Gets the circuit breaker policy.
	/// Opens circuit after consecutive failures to prevent cascading failures.
	/// </summary>
	private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
	{
		return HttpPolicyExtensions
			.HandleTransientHttpError()
			.CircuitBreakerAsync(
				CircuitBreakerThreshold,
				CircuitBreakerDuration);
	}
}
