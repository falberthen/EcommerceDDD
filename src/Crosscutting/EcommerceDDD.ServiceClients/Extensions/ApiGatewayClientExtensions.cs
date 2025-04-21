namespace EcommerceDDD.ServiceClients.Extensions;

public static class ApiGatewayClientExtensions
{
	public static IServiceCollection AddApiGatewayClient(this IServiceCollection services, IConfiguration configuration)
	{
		// Configure HttpClient with Bearer Token Authentication
		services.AddHttpClient<IRequestAdapter, HttpClientRequestAdapter>(client =>
		{
			var baseUrl = configuration["IntegrationHttpSettings:ApiGatewayBaseUrl"];
			if (string.IsNullOrEmpty(baseUrl))			
				throw new InvalidOperationException("The ApiGatewayBaseUrl configuration setting is missing or empty.");
			
			client.BaseAddress = new Uri(baseUrl);
			// Optionally configure the client further (timeouts, headers, etc.)
		})
		.AddTypedClient<IRequestAdapter>((httpClient, serviceProvider) =>
		{
			var tokenRequester = serviceProvider
				.GetRequiredService<ITokenRequester>();

			// Create token provider with the required dependency
			var tokenProvider = new BearerTokenProvider(tokenRequester);
			var authProvider = new BaseBearerTokenAuthenticationProvider(tokenProvider);
			return new HttpClientRequestAdapter(authProvider, httpClient: httpClient);
		});

		// Configure ApiGatewayClient
		services.AddSingleton<ApiGatewayClient>(sp =>
		{
			var adapter = sp.GetRequiredService<IRequestAdapter>();
			return new ApiGatewayClient(adapter);
		});

		return services;
	}
}