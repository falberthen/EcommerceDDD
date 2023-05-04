namespace EcommerceDDD.Core.Infrastructure.WebApi;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services, 
        IConfiguration configuration)
    {
        if (configuration is null)
            throw new ArgumentNullException(nameof(configuration));

        var swaggerSettings = configuration.GetSection("SwaggerSettings")
            .Get<SwaggerGenSettings>();
        if (swaggerSettings is null)
            return services;

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

        return services;
    }

    public static IApplicationBuilder UseSwagger(this IApplicationBuilder app,
        IConfiguration configuration)
    {
        if (configuration is null)
            throw new ArgumentNullException(nameof(configuration));

        var swaggerSettings = configuration.GetSection("SwaggerSettings")
            .Get<SwaggerGenSettings>();
        if (swaggerSettings is null)
            return app;

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint($"/swagger/{swaggerSettings.Version}/swagger.json", swaggerSettings.Title);
        });

        return app;
    }
}
