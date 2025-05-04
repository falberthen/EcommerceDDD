namespace EcommerceDDD.Core.Infrastructure.Identity;

public static class TokenRequesterExtension
{
	public static IServiceCollection ConfigureTokenRequester(this IServiceCollection services,
		IConfiguration configuration)
	{
		if (configuration is null)
			throw new ArgumentNullException(nameof(configuration));

		// ---- Settings
		var tokenIssuerSettings = configuration.GetSection("TokenIssuerSettings")
			?? throw new ArgumentNullException("TokenIssuerSettings section was not found.");
		var integrationHttpSettings = configuration.GetSection("IntegrationHttpSettings")
			?? throw new ArgumentNullException("IntegrationHttpSettings section was not found.");

		services.Configure<TokenIssuerSettings>(tokenIssuerSettings);

		services.AddMemoryCache();
		services.AddHttpClient();
		services.AddTransient<ITokenRequester, TokenRequester>();

		return services;
	}
}