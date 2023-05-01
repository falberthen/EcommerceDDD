namespace EcommerceDDD.Core.Infrastructure.Marten;

public class MartenRepository<TA> : IEventStoreRepository<TA>
    where TA : class, IAggregateRoot<StronglyTypedId<Guid>>
{
    private readonly IDocumentSession _documentSession;
    private readonly IEventDispatcher _dispatcher;

    public MartenRepository(IDocumentSession documentSession, IEventDispatcher dispatcher)
    {
        _documentSession = documentSession;
        _dispatcher = dispatcher;
    }

    public async Task<long> AppendEventsAsync(TA aggregate, CancellationToken cancellationToken = default)
    {
        var events = aggregate.GetUncommittedEvents().ToArray();
        var nextVersion = aggregate.Version + events.Length;

        aggregate.ClearUncommittedEvents();
        _documentSession.Events.Append(aggregate.Id.Value, nextVersion, events);

        await _documentSession.SaveChangesAsync();

        // Dispatching events after saving changes
        DispatchEvents(events);

        return nextVersion;
    }

    public async Task<TA> FetchStreamAsync(Guid id, int? version = null, CancellationToken cancellationToken = default)
    {
        var aggregate = await _documentSession.Events.AggregateStreamAsync<TA>(id, version ?? 0);
        return aggregate ?? throw new InvalidOperationException($"No aggregate found with id {id}.");
    }

    private void DispatchEvents(IList<IDomainEvent> domainEvents)
    {
        foreach (var @event in domainEvents)
            _dispatcher.DispatchAsync(@event);        
    }
}