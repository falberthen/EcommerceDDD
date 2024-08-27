namespace EcommerceDDD.Core.Testing;

public class DummyQueryHandler(IEventStoreRepository<DummyAggregateRoot> repository) : IQueryHandler<DummyQuery, DummyAggregateRoot>
{
    private readonly IEventStoreRepository<DummyAggregateRoot> _eventStoreRepository = repository;

    public async Task<DummyAggregateRoot> Handle(DummyQuery query, CancellationToken cancellationToken)
    {
        var aggregate = new DummyAggregateRoot(query.Id);
        await _eventStoreRepository.AppendEventsAsync(aggregate);

        var storedAggregate = await _eventStoreRepository
            .FetchStreamAsync(query.Id.Value);

        return storedAggregate;
    }
}