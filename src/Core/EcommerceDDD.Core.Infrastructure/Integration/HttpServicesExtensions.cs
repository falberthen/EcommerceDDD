using Microsoft.Extensions.Configuration;
using EcommerceDDD.Core.Infrastructure.Http;
using EcommerceDDD.Core.Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceDDD.Core.Infrastructure.Integration;

public static class HttpServicesExtensions
{
    public static void ConfigureIntegrationHttpService(this IServiceCollection services, IConfiguration configuration)
    {
        // ---- Settings
        var tokenIssuerSettings = configuration.GetSection("TokenIssuerSettings");
        var integrationHttpSettings = configuration.GetSection("IntegrationHttpSettings");
        services.Configure<TokenIssuerSettings>(tokenIssuerSettings);
        services.Configure<IntegrationHttpSettings>(integrationHttpSettings);

        services.AddMemoryCache();
        services.AddHttpClient();
        services.AddTransient<IIntegrationHttpService, IntegrationHttpService>();
        services.AddTransient<IHttpRequester, HttpRequester>();
        services.AddTransient<ITokenRequester, TokenRequester>();
    }
}
