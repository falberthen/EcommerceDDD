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

	public async Task<long> AppendEventsAsync(TA aggregate, CancellationToken cancellationToken = default)
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

        await _documentSession.SaveChangesAsync(cancellationToken);
        return version;
    }

    public async Task<TA?> FetchStreamAsync(Guid id, int? version = null, CancellationToken cancellationToken = default)
    {
        var stream = version.HasValue
            ? await _documentSession.Events.FetchForWriting<TA>(id, version.Value, cancellationToken)
            : await _documentSession.Events.FetchForWriting<TA>(id, cancellationToken);

        if (stream?.Aggregate is null) return null;
        _streams[id] = stream;
        return stream.Aggregate;
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
