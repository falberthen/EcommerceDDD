namespace EcommerceDDD.Core.Tests.CQRS;

public class QueryHandlerTests
{
    [Fact]
    public async Task ValidQuery_ShouldBeHandled()
    {
        // Given
        var repository = new DummyEventStoreRepository<DummyAggregateRoot>();
        var query = new DummyQuery(new DummyAggregateId(Guid.NewGuid()));
        var queryHandler = new DummyQueryHandler(repository);

        // When
        var aggregate = await queryHandler.Handle(query, CancellationToken.None);

        // Then
        Assert.NotNull(aggregate);
        aggregate.Id.Value.Should().Be(query.Id.Value);
    }
}