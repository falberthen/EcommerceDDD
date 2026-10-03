namespace EcommerceDDD.Core.Infrastructure.Extensions;

public static class CoreInfrastructureExtensions
{
	private static readonly TimeSpan[] RetryDelays =
		[TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(1)];

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

			options.DefaultSerializer = new NewtonsoftMessageSerializer();

			// Node assignment health checks are background polling, not traces worth keeping.
			options.Durability.NodeAssignmentHealthCheckTracingEnabled = false;

			// The service's own options come first, so its failure rules take precedence over the ones below.
			configureWolverine?.Invoke(options);

			// Note 1: Wolverine 6 defaults ServiceLocationPolicy to NotAllowed. Services that consume the
			// Kiota clients opt into service location via EcommerceDDD.ServiceClients.Extensions.UseServiceClientServiceLocation().

			// Note 2: A command's [Audit]-marked OrderId is written onto Wolverine's handler
			// span natively as the "order.id" tag the SPA deep-links on.
			
			// Wolverine is the only retry layer (the HTTP clients don't retry). Applies to every service.
			// A broken domain rule or a failed Result is not retried.
			options.Policies
				.OnException<DomainException>()
				.Or<FailedResultException>()
				.MoveToErrorQueue();

			// Transient failures are retried in place, so HTTP requests are retried too.
			options.Policies
				.OnException<HttpRequestException>()
				.Or<ConcurrencyException>()
				.Or<DocumentAlreadyExistsException>()
				.Or<ExistingStreamIdCollisionException>()
				.RetryWithCooldown(
					TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(250))
				.Then.ScheduleRetry(RetryDelays);

			// Other technical failures are retried later, on messages only.
			// Exhausted, the message goes to the dead letter queue.
			options.Policies
				.OnAnyException()
				.ScheduleRetry(RetryDelays);

			// A returned Result goes to the InvokeAsync caller and is never cascaded as a message.
			options.Policies.Add<ResultReturnPolicy>();
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
