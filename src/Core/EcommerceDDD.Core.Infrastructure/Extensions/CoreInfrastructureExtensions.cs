namespace EcommerceDDD.Core.Infrastructure.Extensions;

public static class CoreInfrastructureExtensions
{
	public static IServiceCollection AddCoreInfrastructure(this IServiceCollection services,
		IConfiguration configuration)
	{
		if (configuration is null)
			throw new ArgumentNullException(nameof(configuration));

		services
			.AddMemoryCache()
			.AddHttpContextAccessor()
			// CQRS
			.AddScoped<ICommandBus, CommandBus>()
			.AddScoped<IQueryBus, QueryBus>()
			// Identity
			.AddJwtAuthentication(configuration)
			.AddScoped<IUserInfoRequester, UserInfoRequester>()
			// Token issuer
			.ConfigureTokenRequester(configuration)
			// Swagger extensions
			.AddSwagger(configuration)
			// Testing
			.AddScoped<IEventStoreRepository<DummyAggregateRoot>,
				DummyEventStoreRepository<DummyAggregateRoot>>()
			// EventBus
			.TryAddSingleton<IEventBus, EventPublisher>();

		return services;
	}

	public static IServiceCollection AddHandlersFromType(this IServiceCollection services, Type type)
	{
		Assembly assembly = type.Assembly;
		Type[] assemblyTypes = assembly.GetTypes() ??
			throw new ArgumentNullException(nameof(type));

		foreach (var assemblyType in assemblyTypes)
		{
			if (assemblyType.IsAbstract || assemblyType.IsInterface)
				continue;

			var interfaces = assemblyType.GetInterfaces();
			foreach (var @interface in interfaces)
			{
				if (!@interface.IsGenericType)
					continue;

				Type interfaceDefinition = @interface.GetGenericTypeDefinition();

				if (interfaceDefinition == typeof(IEventHandler<>))
				{
					var handledType = @interface.GenericTypeArguments[0];
					var handlerType = typeof(IEventHandler<>).MakeGenericType(handledType);
					services.AddScoped(handlerType, assemblyType);
				}
				else if (interfaceDefinition == typeof(IQueryHandler<,>))
				{
					var handledType = @interface.GenericTypeArguments[0];
					var responseType = @interface.GenericTypeArguments[1];
					var handlerType = typeof(IQueryHandler<,>).MakeGenericType(handledType, responseType);
					services.AddScoped(handlerType, assemblyType);
				}
				else if (interfaceDefinition == typeof(ICommandHandler<>))
				{
					var handledType = @interface.GenericTypeArguments[0];
					var handlerType = typeof(ICommandHandler<>).MakeGenericType(handledType);
					services.AddScoped(handlerType, assemblyType);
				}
			}
		}

		return services;
	}
}