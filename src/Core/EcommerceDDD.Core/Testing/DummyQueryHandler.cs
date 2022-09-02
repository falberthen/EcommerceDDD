using System.Threading;
using EcommerceDDD.Core.CQRS.QueryHandling;
using EcommerceDDD.Core.Persistence;

namespace EcommerceDDD.Core.Testing;

public class DummyQueryHandler : QueryHandler<DummyQuery, DummyAggregateRoot>
{
    private readonly IEventStoreRepository<DummyAggregateRoot> _eventStoreRepository;

    public DummyQueryHandler(IEventStoreRepository<DummyAggregateRoot> repository)
    {
        _eventStoreRepository = repository;
    }

    public override async Task<DummyAggregateRoot> Handle(DummyQuery query, CancellationToken cancellationToken)
    {
        var aggregate = new DummyAggregateRoot(query.Id);
        await _eventStoreRepository.AppendEventsAsync(aggregate);

        var storedAggregate = await _eventStoreRepository
            .FetchStream(query.Id.Value);

        return storedAggregate;
    }
}