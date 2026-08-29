namespace EcommerceDDD.Core.Infrastructure.Marten;

public class MartenRepository<TA>(
	IDocumentSession documentSession,
	IMartenOutbox outbox,
    ILogger<MartenRepository<TA>> logger
) : IEventStoreRepository<TA> where TA : class, IAggregateRoot<StronglyTypedId<Guid>>
{
	private readonly IDocumentSession _documentSession = documentSession
		?? throw new ArgumentNullException(nameof(documentSession));
	private readonly IMartenOutbox _outbox = outbox
		?? throw new ArgumentNullException(nameof(outbox));
	private readonly ILogger<MartenRepository<TA>> _logger = logger
		?? throw new ArgumentNullException(nameof(logger));

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

        await _documentSession.SaveChangesAsync(cancellationToken);
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
    public async Task<TA?> FetchStreamAsync(Guid id, int? version = null, CancellationToken cancellationToken = default)
    {
        var aggregate = await _documentSession.Events.AggregateStreamAsync<TA>(
			id, version ?? 0, 
			token: cancellationToken
		);
		return aggregate;
    }

    /// <summary>
    /// Stages an integration event in Wolverine's durable outbox.
    /// Wolverine's durability agent relays it to Kafka afterwards
    /// and carries the trace context across the hop on its own.
    /// </summary>
    /// <param name="event"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task AppendToOutboxAsync(INotification @event)
    {
        if (@event is null)
            throw new ArgumentNullException(nameof(@event));

        _outbox.Enroll(_documentSession);

        _logger.LogInformation("Adding integration event {EventName} to outbox...", @event.GetType().Name);
        await _outbox.PublishAsync(@event);
    }
}
