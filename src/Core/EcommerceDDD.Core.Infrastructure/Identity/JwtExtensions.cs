namespace EcommerceDDD.Core.Infrastructure.Identity;

public static class JwtExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, 
        IConfiguration configuration)
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

        return services;
    }
}
