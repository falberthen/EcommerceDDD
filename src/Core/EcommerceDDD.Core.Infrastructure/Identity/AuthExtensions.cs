using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace EcommerceDDD.Core.Infrastructure.Identity;

public static class AuthExtensions
{
    public static void AddAuthentication(this IServiceCollection services, ConfigurationManager configuration)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.Authority = configuration.GetValue<string>("TokenIssuerSettings:Authority");
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false
            };
        });
    }
}
