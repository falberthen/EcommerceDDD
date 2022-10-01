using Marten;
using EcommerceDDD.Core.Domain;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Core.EventBus;

namespace EcommerceDDD.Core.Infrastructure.Marten;

public class MartenRepository<TA> : IEventStoreRepository<TA>
    where TA : class, IAggregateRoot<StronglyTypedId<Guid>>
{
    private readonly IDocumentStore _store;
    private readonly IDomainEventDispatcher _dispatcher;

    public MartenRepository(IDocumentStore store, IDomainEventDispatcher dispatcher)
    {
        _store = store;
        _dispatcher = dispatcher;
    }

    public async Task<long> AppendEventsAsync(TA aggregate, CancellationToken cancellationToken = default)
    {
        using var session = _store.OpenSession();
        var events = aggregate.GetUncommittedEvents().ToArray();
        var nextVersion = aggregate.Version + events.Length;

        aggregate.ClearUncommittedEvents();
        session.Events.Append(aggregate.Id.Value, nextVersion, events);
        await session.SaveChangesAsync();

        // Dispatching events after saving changes
        DispatchEvents(events);

        return nextVersion;
    }

    public async Task<TA> FetchStreamAsync(Guid id, int? version = null, CancellationToken cancellationToken = default)
    {
        using var session = _store.LightweightSession();
        var aggregate = await session.Events.AggregateStreamAsync<TA>(id, version ?? 0);
        return aggregate ?? throw new InvalidOperationException($"No aggregate found with id {id}.");
    }

    private void DispatchEvents(IList<IDomainEvent> domainEvents)
    {
        foreach (var @event in domainEvents)
            _dispatcher.DispatchAsync(@event);        
    }
}