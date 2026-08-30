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

	private readonly Dictionary<Guid, IEventStream<TA>> _streams = new();

	public async Task<long> AppendEventsAndCommitAsync(TA aggregate, CancellationToken cancellationToken = default,
		params INotification[] integrationEvents)
    {
        var events = aggregate.GetUncommittedEvents().ToArray();
        aggregate.ClearUncommittedEvents();

        long version;
        if (_streams.TryGetValue(aggregate.Id.Value, out var stream))
        {
            // FetchForWriting handed us this stream: append through it so Marten
            // keeps the optimistic concurrency check it staged on the session.
            stream.AppendMany(events);

            // Calculating version.
            version = stream.CurrentVersion.GetValueOrDefault();
        }
        else
        {
            _documentSession.Events.StartStream<TA>(aggregate.Id.Value, events);
            version = events.Length;
        }

        await StageIntegrationEventsAsync(integrationEvents);

        await _documentSession.SaveChangesAsync(cancellationToken);
        return version;
    }

    public async Task<TA?> FetchForWritingAsync(Guid id, int? version = null, CancellationToken cancellationToken = default)
    {
        var stream = version.HasValue
            ? await _documentSession.Events.FetchForWriting<TA>(id, version.Value, cancellationToken)
            : await _documentSession.Events.FetchForWriting<TA>(id, cancellationToken);

        if (stream?.Aggregate is null) return null;
        _streams[id] = stream;
        return stream.Aggregate;
    }

    /// <summary>
    /// Writes the outgoing messages into the same session that holds the aggregate's events,
    /// so the caller's SaveChangesAsync commits both or neither. Wolverine's durability agent
    /// relays them to Kafka after the commit and carries the trace context across the hop on its own. 
    /// </summary>
    private async Task StageIntegrationEventsAsync(INotification[] integrationEvents)
    {
        if (integrationEvents.Length == 0)
            return;

        if (Array.Exists(integrationEvents, e => e is null))
            throw new ArgumentException("Integration events cannot be null.", nameof(integrationEvents));

        _outbox.Enroll(_documentSession);

        foreach (var @event in integrationEvents)
        {
            _logger.LogInformation("Adding integration event {EventName} to outbox...", @event.GetType().Name);
            await _outbox.PublishAsync(@event);
        }
    }
}
