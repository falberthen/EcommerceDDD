using System.Threading;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Core.Persistence;

namespace EcommerceDDD.Core.Testing;

public class DummyCommandHandler : CommandHandler<DummyCommand>
{
    private readonly IEventStoreRepository<DummyAggregateRoot> _eventStoreRepository;

    public DummyCommandHandler(IEventStoreRepository<DummyAggregateRoot> repository)
    {
        _eventStoreRepository = repository;
    }

    public override async Task Handle(DummyCommand command, CancellationToken cancellationToken)
    {
        var aggregate = new DummyAggregateRoot(command.Id);
        await _eventStoreRepository.AppendEventsAsync(aggregate);

        await Task.CompletedTask;
    }
}