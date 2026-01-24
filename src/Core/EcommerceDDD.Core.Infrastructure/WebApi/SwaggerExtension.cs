using Swashbuckle.AspNetCore.SwaggerGen;

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
				Type = SecuritySchemeType.Http,
				Scheme = "Bearer"
			});

			s.OperationFilter<AuthorizeCheckOperationFilter>();
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

public class AuthorizeCheckOperationFilter : IOperationFilter
{
	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		var hasAuthorize = context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()
			|| context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

		var allowAnonymous = context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any()
			|| context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any();

		if (hasAuthorize && !allowAnonymous)
		{
			operation.Security = new List<OpenApiSecurityRequirement>
			{
				new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							}
						},
						new string[] {}
					}
				}
			};

			operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
			operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });
		}
	}
}