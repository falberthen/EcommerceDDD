namespace EcommerceDDD.Core.Tests.CQRS;

public class CommandHandlerTests
{
    [Fact]
    public async Task ValidCommand_ShouldBeHandled()
    {
        // Given        
        var command = new DummyCommand(new DummyAggregateId(Guid.NewGuid()));
        var commandHandler = new DummyCommandHandler(_repository);

		// When
		await commandHandler.HandleAsync(command, CancellationToken.None);

		// Then
		await _repository.Received(1).AppendEventsAsync(
			Arg.Is<DummyAggregateRoot>(aggregate => aggregate.Id.Value == command.Id.Value),
			Arg.Any<CancellationToken>());
	}

	private readonly IEventStoreRepository<DummyAggregateRoot> _repository =
		Substitute.For<IEventStoreRepository<DummyAggregateRoot>>();
}