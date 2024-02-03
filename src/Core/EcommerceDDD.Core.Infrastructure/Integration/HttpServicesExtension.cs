namespace EcommerceDDD.Core.Infrastructure.Integration;

public static class HttpServicesExtension
{
    public static IServiceCollection ConfigureIntegrationHttpService(this IServiceCollection services, 
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
        services.Configure<IntegrationHttpSettings>(integrationHttpSettings);

        services.AddMemoryCache();
        services.AddHttpClient();
        services.AddTransient<IIntegrationHttpService, IntegrationHttpService>();
        services.AddTransient<IHttpRequester, HttpRequester>();
        services.AddTransient<ITokenRequester, TokenRequester>();

        return services;
    }
}