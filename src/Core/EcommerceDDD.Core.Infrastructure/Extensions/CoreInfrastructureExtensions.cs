namespace EcommerceDDD.Core.Infrastructure.Extensions;

public static class CoreInfrastructureExtensions
{
	public static IServiceCollection AddCoreInfrastructure(this IServiceCollection services,
		IConfiguration configuration,
		Action<WolverineOptions>? configureWolverine = null)
	{
		if (configuration is null)
			throw new ArgumentNullException(nameof(configuration));

		// Wolverine owns command/query/event dispatch. Handler discovery is conventional,
		// so it scans the entry assembly of whichever service is bootstrapping.
		services.AddWolverine(options =>
		{
			var applicationAssembly = Assembly.GetEntryAssembly();
			if (applicationAssembly is not null)
				options.ApplicationAssembly = applicationAssembly;

			// Note 1: Wolverine 6 defaults ServiceLocationPolicy to NotAllowed. Services that consume the
			// Kiota clients opt into service location via EcommerceDDD.ServiceClients.Extensions.UseServiceClientServiceLocation().

			// Note 2: A command's [Audit]-marked OrderId is written onto Wolverine's handler
			// span natively as the "order.id" tag the SPA deep-links on.
			// Transient failures retry with backoff; exhausting the attempts dead-letters the
			// message instead of dropping it silently. Applies to every service.
			options.Policies.OnAnyException()
				.RetryWithCooldown(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5));

			options.DefaultSerializer = new NewtonsoftMessageSerializer();

			configureWolverine?.Invoke(options);
		});

		services
			.AddMemoryCache()
			.AddHttpContextAccessor()
			// Exception handling
			.AddExceptionHandler<GlobalExceptionHandler>()
			.AddProblemDetails()
			// Identity
			.AddJwtAuthentication(configuration)
			.AddScoped<IUserInfoRequester, UserInfoRequester>()
			// Token issuer
			.ConfigureTokenRequester(configuration)
			// Swagger extensions
			.AddSwagger(configuration)
			// Testing
			.AddScoped<IEventStoreRepository<DummyAggregateRoot>,
				DummyEventStoreRepository<DummyAggregateRoot>>();

		// OpenTelemetry
		var serviceName = Assembly.GetEntryAssembly()?.GetName().Name ?? "Unknown";
		services.AddOpenTelemetryObservability(serviceName);

		return services;
	}
}
