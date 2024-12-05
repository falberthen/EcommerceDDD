namespace EcommerceDDD.Core.Testing;

public class DummyQueryHandler(IEventStoreRepository<DummyAggregateRoot> repository) : IQueryHandler<DummyQuery, DummyAggregateRoot>
{
    private readonly IEventStoreRepository<DummyAggregateRoot> _eventStoreRepository = repository
		?? throw new ArgumentNullException(nameof(repository));

    public async Task<DummyAggregateRoot> Handle(DummyQuery query, CancellationToken cancellationToken)
    {
        var aggregate = new DummyAggregateRoot(query.Id);
		aggregate.DoSomething();
		await _eventStoreRepository.AppendEventsAsync(aggregate);

        return aggregate;
    }
}