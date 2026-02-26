namespace EcommerceDDD.ServiceClients.Extensions;

public static class KiotaClientExtensions
{
	private static readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(30);
	private const int _retryCount = 3;
	private static readonly TimeSpan _circuitBreakerDuration = TimeSpan.FromSeconds(30);
	private const int _circuitBreakerThreshold = 5;

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
			client.Timeout = _defaultTimeout;
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

	public static IServiceCollection AddIdentityServiceClient(this IServiceCollection services, string? baseUrl)
	{
		services.AddKiotaClient<IdentityServerClient>(baseUrl);
		services.AddScoped<IIdentityService, IdentityService>();
		return services;
	}

	public static IServiceCollection AddCustomerManagementServiceClient(this IServiceCollection services, string? baseUrl)
	{
		services.AddKiotaClient<CustomerManagementClient>(baseUrl);
		services.AddScoped<ICustomerManagementService, CustomerManagementService>();
		return services;
	}

	public static IServiceCollection AddInventoryServiceClient(this IServiceCollection services, string? baseUrl)
	{
		services.AddKiotaClient<InventoryManagementClient>(baseUrl);
		services.AddScoped<IInventoryService, InventoryService>();
		return services;
	}

	public static IServiceCollection AddProductCatalogServiceClient(this IServiceCollection services, string? baseUrl)
	{
		services.AddKiotaClient<ProductCatalogClient>(baseUrl);
		services.AddScoped<IProductCatalogService, ProductCatalogService>();
		return services;
	}

	public static IServiceCollection AddQuoteServiceClient(this IServiceCollection services, string? baseUrl)
	{
		services.AddKiotaClient<QuoteManagementClient>(baseUrl);
		services.AddScoped<IQuoteService, QuoteService>();
		return services;
	}

	public static IServiceCollection AddPaymentServiceClient(this IServiceCollection services, string? baseUrl)
	{
		services.AddKiotaClient<PaymentProcessingClient>(baseUrl);
		services.AddScoped<IPaymentService, PaymentService>();
		return services;
	}

	public static IServiceCollection AddShipmentServiceClient(this IServiceCollection services, string? baseUrl)
	{
		services.AddKiotaClient<ShipmentProcessingClient>(baseUrl);
		services.AddScoped<IShipmentService, ShipmentService>();
		return services;
	}

	public static IServiceCollection AddOrderNotificationServiceClient(this IServiceCollection services, string? baseUrl)
	{
		services.AddKiotaClient<SignalRClient>(baseUrl);
		services.AddScoped<IOrderNotificationService, OrderNotificationService>();
		return services;
	}

	public static IServiceCollection AddIdentityServiceClient(this IServiceCollection services, IConfiguration configuration)
		=> services.AddIdentityServiceClient(GetOptions(configuration).IdentityServer);

	public static IServiceCollection AddCustomerManagementServiceClient(this IServiceCollection services, IConfiguration configuration)
		=> services.AddCustomerManagementServiceClient(GetOptions(configuration).CustomerManagement);

	public static IServiceCollection AddInventoryServiceClient(this IServiceCollection services, IConfiguration configuration)
		=> services.AddInventoryServiceClient(GetOptions(configuration).InventoryManagement);

	public static IServiceCollection AddProductCatalogServiceClient(this IServiceCollection services, IConfiguration configuration)
		=> services.AddProductCatalogServiceClient(GetOptions(configuration).ProductCatalog);

	public static IServiceCollection AddQuoteServiceClient(this IServiceCollection services, IConfiguration configuration)
		=> services.AddQuoteServiceClient(GetOptions(configuration).QuoteManagement);

	public static IServiceCollection AddPaymentServiceClient(this IServiceCollection services, IConfiguration configuration)
		=> services.AddPaymentServiceClient(GetOptions(configuration).PaymentProcessing);

	public static IServiceCollection AddShipmentServiceClient(this IServiceCollection services, IConfiguration configuration)
		=> services.AddShipmentServiceClient(GetOptions(configuration).ShipmentProcessing);

	public static IServiceCollection AddOrderNotificationServiceClient(this IServiceCollection services, IConfiguration configuration)
		=> services.AddOrderNotificationServiceClient(GetOptions(configuration).SignalRClient);

	private static ServiceClientsOptions GetOptions(IConfiguration configuration)
		=> configuration.GetSection(ServiceClientsOptions.SectionName).Get<ServiceClientsOptions>()
			?? new ServiceClientsOptions();

	/// <summary>
	/// Gets the retry policy with exponential backoff.
	/// Retries on transient HTTP errors (5xx, 408, network failures).
	/// </summary>
	private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
	{
		return HttpPolicyExtensions
			.HandleTransientHttpError()
			.WaitAndRetryAsync(
				_retryCount,
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
			.CircuitBreakerAsync(_circuitBreakerThreshold, _circuitBreakerDuration);
	}
}
