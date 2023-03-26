using MediatR;
using System.Threading;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Core.Testing;

public class DummyCommandHandler : ICommandHandler<DummyCommand>
{
    private readonly IEventStoreRepository<DummyAggregateRoot> _eventStoreRepository;

    public DummyCommandHandler(IEventStoreRepository<DummyAggregateRoot> repository)
    {
        _eventStoreRepository = repository;
    }

    public async Task Handle(DummyCommand command, CancellationToken cancellationToken)
    {
        var aggregate = new DummyAggregateRoot(command.Id);
        await _eventStoreRepository.AppendEventsAsync(aggregate);
    }
}