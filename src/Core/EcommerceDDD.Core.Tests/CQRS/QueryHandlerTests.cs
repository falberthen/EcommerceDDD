namespace EcommerceDDD.Core.Tests.CQRS;

public class QueryHandlerTests
{
    [Fact]
    public async Task ValidQuery_ShouldBeHandled()
    {
        // Given
        var query = new DummyQuery(new DummyAggregateId(Guid.NewGuid()));
        var queryHandler = new DummyQueryHandler(_repository);

        // When
        var aggregate = await queryHandler.HandleAsync(query, CancellationToken.None);

        // Then
        Assert.True(aggregate.IsSuccess);
        Assert.NotNull(aggregate.Value);
		Assert.Equal(aggregate.Value!.Id.Value, query.Id.Value);
	}

	private readonly IEventStoreRepository<DummyAggregateRoot> _repository =
		Substitute.For<IEventStoreRepository<DummyAggregateRoot>>();
}