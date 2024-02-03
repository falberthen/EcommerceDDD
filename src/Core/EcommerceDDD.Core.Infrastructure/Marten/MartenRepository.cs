namespace EcommerceDDD.Core.Infrastructure.Marten;

public class MartenRepository<TA> : IEventStoreRepository<TA>
    where TA : class, IAggregateRoot<StronglyTypedId<Guid>>
{
    private readonly IDocumentSession _documentSession;
    private readonly ILogger<MartenRepository<TA>> _logger;

    public MartenRepository(
        IDocumentSession documentSession, 
        ILogger<MartenRepository<TA>> logger)
    {
        _documentSession = documentSession;
        _logger = logger;
    }

    /// <summary>
    /// Stores uncommited events from an aggregate 
    /// </summary>
    /// <param name="aggregate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<long> AppendEventsAsync(TA aggregate, CancellationToken cancellationToken = default)
    {
        var events = aggregate.GetUncommittedEvents().ToArray();
        var nextVersion = aggregate.Version + events.Length;

        aggregate.ClearUncommittedEvents();
        _documentSession.Events.Append(aggregate.Id.Value, nextVersion, events);

        await _documentSession.SaveChangesAsync();

        return nextVersion;
    }

    /// <summary>
    /// Fetch domain events from the stream
    /// </summary>
    /// <param name="id"></param>
    /// <param name="version"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<TA> FetchStreamAsync(Guid id, int? version = null, CancellationToken cancellationToken = default)
    {
        var aggregate = await _documentSession.Events.AggregateStreamAsync<TA>(id, version ?? 0);
        return aggregate ?? throw new InvalidOperationException($"No aggregate found with id {id}.");
    }

    /// <summary>
    /// Store integration events into the store
    /// Outbox pattern
    /// </summary>
    /// <param name="event"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void AppendToOutbox(INotification @event)
    {
        if (@event is null)
            throw new ArgumentNullException(nameof(@event));

        var integrationEvent = IntegrationEvent
            .FromNotification(@event!);

        _logger.LogInformation($"Adding integration event {@event} to outbox...", @event);
        _documentSession.Store(integrationEvent!);        
    }
}