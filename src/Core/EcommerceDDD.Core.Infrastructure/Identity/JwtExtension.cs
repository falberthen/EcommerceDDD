namespace EcommerceDDD.Core.Infrastructure.Identity;

public static class JwtExtension
{
	public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
		IConfiguration configuration)
	{
		if (configuration is null)
			throw new ArgumentNullException(nameof(configuration));

		var authority = configuration
			.GetValue<string>("TokenIssuerSettings:Authority")
			?? throw new ArgumentNullException("TokenIssuerSettings:Authority section was not found");

		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(options =>
		{
			options.RequireHttpsMetadata = false;
			options.Authority = authority;
			options.MapInboundClaims = false;
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = false,
				RoleClaimType = JwtClaimTypes.Role,
				NameClaimType = JwtClaimTypes.Name
			};
		});

		return services;
	}

	/// <summary>
	/// Accepts the access token from the "access_token" query string for the given hub path.
	/// A browser cannot set an Authorization header on the WebSocket handshake, so the SignalR
	/// client sends it as a query-string parameter instead.
	/// </summary>
	public static IServiceCollection AddHubQueryStringAuthentication(this IServiceCollection services,
		string hubPath)
	{
		if (string.IsNullOrWhiteSpace(hubPath))
			throw new ArgumentNullException(nameof(hubPath));

		services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
			options.Events = new JwtBearerEvents
			{
				OnMessageReceived = context =>
				{
					var accessToken = context.Request.Query["access_token"];

					if (!string.IsNullOrEmpty(accessToken)
						&& context.HttpContext.Request.Path.StartsWithSegments(hubPath))
						context.Token = accessToken;

					return Task.CompletedTask;
				}
			});

		return services;
	}
}
