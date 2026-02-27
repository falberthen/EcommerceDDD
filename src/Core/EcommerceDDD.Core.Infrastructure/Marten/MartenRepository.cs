namespace EcommerceDDD.Core.Infrastructure.Marten;

public class MartenRepository<TA>(
	IDocumentSession documentSession,
    ILogger<MartenRepository<TA>> logger
) : IEventStoreRepository<TA> where TA : class, IAggregateRoot<StronglyTypedId<Guid>>
{
	private static readonly ActivitySource _activitySource = new(ActivitySources.OutboxWrite);
	private readonly IDocumentSession _documentSession = documentSession
		?? throw new ArgumentNullException(nameof(documentSession));
	private readonly ILogger<MartenRepository<TA>> _logger = logger
		?? throw new ArgumentNullException(nameof(logger));
	private Activity? _pendingOutboxActivity;

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

        try
        {
            await _documentSession.SaveChangesAsync(cancellationToken);
            return nextVersion;
        }
        finally
        {
            // Close the outbox producer span only after the DB commit.
            // Npgsql spans from SaveChangesAsync are children of this span.
            _pendingOutboxActivity?.Dispose();
            _pendingOutboxActivity = null;
        }
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
        var aggregate = await _documentSession.Events.AggregateStreamAsync<TA>(
			id, version ?? 0, 
			token: cancellationToken
		);
        return aggregate ?? 
			throw new InvalidOperationException($"No aggregate found with id {id}.");
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

        var eventName = @event.GetType().Name;

        // Open the producer span here so Activity.Current is this span when
        // IntegrationEvent.FromNotification captures TraceContext = Activity.Current?.Id.
        // The span remains open until SaveChangesAsync commits the outbox entry to the DB inside AppendEventsAsync.
        _pendingOutboxActivity = _activitySource.StartActivity(
            $"outbox.publish {eventName}",
            ActivityKind.Producer);

        _pendingOutboxActivity?.SetTag("messaging.system", "kafka");
        _pendingOutboxActivity?.SetTag("messaging.destination.name", "outbox");
        _pendingOutboxActivity?.SetTag("messaging.operation", "publish");
        _pendingOutboxActivity?.SetTag("event.type", eventName);

        var integrationEvent = IntegrationEvent.FromNotification(@event!);

        _logger.LogInformation("Adding integration event {EventName} to outbox...", eventName);
        _documentSession.Store(integrationEvent!);
    }
}