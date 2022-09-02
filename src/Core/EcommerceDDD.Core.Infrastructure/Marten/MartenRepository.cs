using Marten;
using EcommerceDDD.Core.Domain;
using EcommerceDDD.Core.Persistence;

namespace EcommerceDDD.Core.Infrastructure.Marten;

public class MartenRepository<TA> : IEventStoreRepository<TA>
    where TA : class, IAggregateRoot<StronglyTypedId<Guid>>
{
    private readonly IDocumentStore _store;

    public MartenRepository(IDocumentStore store)
    {
        _store = store;
    }

    public async Task<long> AppendEventsAsync(TA aggregate, CancellationToken cancellationToken = default)
    {
        using var session = _store.OpenSession();
        var events = aggregate.GetUncommittedEvents().ToArray();
        var nextVersion = aggregate.Version + events.Length;

        session.Events.Append(aggregate.Id.Value, nextVersion, events);
        await session.SaveChangesAsync();

        aggregate.ClearUncommittedEvents();
        return nextVersion;
    }

    public async Task<TA> FetchStream(Guid id, int? version = null, CancellationToken cancellationToken = default)
    {
        using var session = _store.LightweightSession();
        var aggregate = await session.Events.AggregateStreamAsync<TA>(id, version ?? 0);
        return aggregate ?? throw new InvalidOperationException($"No aggregate found with id {id}.");
    }
}