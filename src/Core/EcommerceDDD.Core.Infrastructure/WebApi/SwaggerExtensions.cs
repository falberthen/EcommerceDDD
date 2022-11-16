using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceDDD.Core.Infrastructure.WebApi;

public static class SwaggerExtensions
{
    public static void AddSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        var swaggerSettings = configuration.GetSection("SwaggerSettings").Get<SwaggerGenSettings>();
        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc(swaggerSettings.Version, new OpenApiInfo
            {
                Version = swaggerSettings.Version,
                Title = swaggerSettings.Title,
                Description = swaggerSettings.Description,
                Contact = new OpenApiContact { Name = "Felipe Henrique", Email = "fealberto@gmail.com" },
                License = new OpenApiLicense() { Name = "MIT", Url = new Uri("https://github.com/falberthen/EcommerceDDD/blob/master/LICENSE") }
            });

            s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            s.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,

                    },
                    new List<string>()
                }
            });
        });
    }

    public static void UseSwagger(this IApplicationBuilder app,
        ConfigurationManager configuration)
    {
        if (app is null)
            throw new ArgumentNullException(nameof(app));

        var swaggerSettings = configuration
            .GetSection("SwaggerSettings").Get<SwaggerGenSettings>();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", swaggerSettings.Description);
        });
    }
}
