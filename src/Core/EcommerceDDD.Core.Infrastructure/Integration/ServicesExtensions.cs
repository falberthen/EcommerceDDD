using Microsoft.AspNetCore.Builder;
using EcommerceDDD.Core.Infrastructure.Http;
using EcommerceDDD.Core.Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceDDD.Core.Infrastructure.Integration;

public static class ServicesExtensions
{
    public static void ConfigureIntegrationHttpService(this IServiceCollection services, WebApplicationBuilder builder)
    {
        // ---- Settings
        var tokenIssuerSettings = builder.Configuration.GetSection("TokenIssuerSettings");
        var integrationHttpSettings = builder.Configuration.GetSection("IntegrationHttpSettings");
        builder.Services.Configure<TokenIssuerSettings>(tokenIssuerSettings);
        builder.Services.Configure<IntegrationHttpSettings>(integrationHttpSettings);

        services.AddMemoryCache();
        services.AddHttpClient();
        services.AddTransient<IIntegrationHttpService, IntegrationHttpService>();
        services.AddTransient<IHttpRequester, HttpRequester>();
        services.AddTransient<ITokenRequester, TokenRequester>();
    }
}
