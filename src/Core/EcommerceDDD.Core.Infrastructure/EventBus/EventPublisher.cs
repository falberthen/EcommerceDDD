namespace EcommerceDDD.Core.Infrastructure.EventBus;

public class EventPublisher : IEventPublisher
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<EventPublisher> _logger;

    public EventPublisher(IServiceScopeFactory serviceScopeFactory, ILogger<EventPublisher> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

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
