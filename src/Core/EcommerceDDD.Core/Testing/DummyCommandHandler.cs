namespace EcommerceDDD.Core.Testing;

public class DummyCommandHandler(IEventStoreRepository<DummyAggregateRoot> repository)
{
	private readonly IEventStoreRepository<DummyAggregateRoot> _eventStoreRepository = repository ??
		throw new ArgumentNullException(nameof(repository));

	public async Task<Result> HandleAsync(DummyCommand command, CancellationToken cancellationToken)
	{
		var aggregate = new DummyAggregateRoot(command.Id);
		await _eventStoreRepository.AppendEventsAndCommitAsync(aggregate);
		return Result.Ok();
	}
}