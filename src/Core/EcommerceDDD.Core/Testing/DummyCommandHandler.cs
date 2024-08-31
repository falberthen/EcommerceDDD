namespace EcommerceDDD.Core.Testing;

public class DummyCommandHandler(IEventStoreRepository<DummyAggregateRoot> repository) : ICommandHandler<DummyCommand>
{
    private readonly IEventStoreRepository<DummyAggregateRoot> _eventStoreRepository = repository;

    public async Task Handle(DummyCommand command, CancellationToken cancellationToken)
    {
        var aggregate = new DummyAggregateRoot(command.Id);
        await _eventStoreRepository.AppendEventsAsync(aggregate);
    }
}