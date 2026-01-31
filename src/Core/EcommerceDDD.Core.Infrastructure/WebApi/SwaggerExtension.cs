namespace EcommerceDDD.Core.Infrastructure.WebApi;

public static class SwaggerExtension
{
	public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
	{
		if (configuration is null)
			throw new ArgumentNullException(nameof(configuration));

		var swaggerSettings = configuration.GetSection("SwaggerSettings")
			.Get<SwaggerGenSettings>();
		if (swaggerSettings is null)
			return services;

		services.AddSwaggerGen(options =>
		{
			options.SwaggerDoc(swaggerSettings.Version, new OpenApiInfo
			{
				Version = swaggerSettings.Version,
				Title = swaggerSettings.Title,
				Description = swaggerSettings.Description,
				Contact = new OpenApiContact { Name = "Felipe Henrique", Email = "fealberto@gmail.com" },
				License = new OpenApiLicense() { Name = "MIT", Url = new Uri("https://github.com/falberthen/EcommerceDDD/blob/master/LICENSE") }
			});

			options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Type = SecuritySchemeType.Http,
				Scheme = "bearer",
				BearerFormat = "JWT",
				In = ParameterLocation.Header,
				Name = "Authorization",
				Description = "JWT Authorization header using the Bearer scheme."
			});
			
			options.OperationFilter<SecurityRequirementsOperationFilter>();
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
			c.SwaggerEndpoint($"/swagger/{swaggerSettings.Version ?? ApiVersions.V2}/swagger.json", swaggerSettings.Title);
		});

		return app;
	}
}