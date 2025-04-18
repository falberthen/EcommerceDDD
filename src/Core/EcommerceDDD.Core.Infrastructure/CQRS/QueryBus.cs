namespace EcommerceDDD.Core.Infrastructure.CQRS;

public class QueryBus(
	IServiceProvider serviceProvider, 
	ILogger<QueryBus> logger
) : IQueryBus
{
	public async Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
	{
		logger.LogInformation("Executing query: {query}", query);

		var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
		dynamic? handler = serviceProvider.GetService(handlerType);

		if (handler is null)
			throw new InvalidOperationException($"Handler for query {query.GetType().Name} not registered.");

		return await handler.HandleAsync((dynamic)query, cancellationToken);
	}
}
