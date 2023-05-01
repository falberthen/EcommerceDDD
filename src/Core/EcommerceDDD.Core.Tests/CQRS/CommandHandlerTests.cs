namespace EcommerceDDD.Core.Tests.CQRS;

public class CommandHandlerTests
{
    [Fact]
    public async Task ValidCommand_ShouldBeHandled()
    {
        // Given
        var repository = new DummyEventStoreRepository<DummyAggregateRoot>();
        var command = new DummyCommand(new DummyAggregateId(Guid.NewGuid()));
        var commandHandler = new DummyCommandHandler(repository);

        // When
        await commandHandler.Handle(command, CancellationToken.None);

        // Then
        repository.AggregateStream.Count().Should().Be(1);
    }
}