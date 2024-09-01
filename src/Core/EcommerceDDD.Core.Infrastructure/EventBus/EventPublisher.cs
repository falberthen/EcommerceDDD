namespace EcommerceDDD.Core.Infrastructure.EventBus;

public class EventPublisher(IServiceScopeFactory serviceScopeFactory, ILogger<EventPublisher> logger) : IEventPublisher
{
	private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory
		?? throw new ArgumentNullException(nameof(serviceScopeFactory));
	private readonly ILogger<EventPublisher> _logger = logger
		?? throw new ArgumentNullException(nameof(logger));

	public async Task PublishEventAsync(INotification @event, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Publishing event {@event}", @event);

		try
		{
			var scope = _serviceScopeFactory.CreateScope();
			var scopedServices = scope.ServiceProvider;
			var mediator = scopedServices.GetRequiredService<IMediator>();

			await mediator.Publish(@event, cancellationToken);
		}
		catch (Exception e)
		{
			_logger.LogError("An error occurred when publishing event: {Message} {StackTrace}", e.Message, e.StackTrace);
		}
	}

	public async Task PublishEventsAsync(IEnumerable<INotification> @events, CancellationToken cancellationToken)
	{
		foreach (var @event in @events)
			await PublishEventAsync(@event, cancellationToken);
	}
}
