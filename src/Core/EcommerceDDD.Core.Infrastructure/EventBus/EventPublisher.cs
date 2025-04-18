namespace EcommerceDDD.Core.Infrastructure.EventBus;

public class EventPublisher(
	IServiceScopeFactory serviceScopeFactory,
	ILogger<EventPublisher> logger
) : IEventBus
{
	public async Task PublishEventAsync(INotification @event, CancellationToken cancellationToken)
	{
		logger.LogInformation("Publishing event: {Event}", @event);

		using var scope = serviceScopeFactory.CreateScope();
		var handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
		dynamic? handler = scope.ServiceProvider.GetService(handlerType)
			?? throw new InvalidOperationException($"Handler for event {@event.GetType().Name} not registered.");

		await handler.HandleAsync((dynamic)@event, cancellationToken);
	}
}