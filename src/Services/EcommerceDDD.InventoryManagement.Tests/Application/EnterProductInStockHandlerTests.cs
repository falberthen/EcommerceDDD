namespace EcommerceDDD.InventoryManagement.Tests.Application;

public class EnterProductInStockHandlerTests
{
    [Fact]
    public async Task EnterProductInStock_WithCommand_ShouldEnterProductInStock()
    {
        // Given
        List<Tuple<ProductId, int>> productIdsQuantities = new()
        {
            new Tuple<ProductId, int>(ProductId.Of(Guid.NewGuid()), 1),
            new Tuple<ProductId, int>(ProductId.Of(Guid.NewGuid()), 1),
            new Tuple<ProductId, int>(ProductId.Of(Guid.NewGuid()), 1),
        };

        foreach (var productId in productIdsQuantities)
        {
            var inventoryStockUnit = InventoryStockUnit.EnterStockUnit(productId.Item1, 1);
            await _inventoryStockUnitWriteRepository.AppendEventsAsync(inventoryStockUnit);
        }

        var enterProductInStock = EnterProductInStock.Create(productIdsQuantities);
        var enterProductInStockHandler = new EnterProductInStockHandler(_querySession,
            _inventoryStockUnitWriteRepository);

        // Mocking query session  
        _querySession.Query<InventoryStockUnitDetails>()
            .Returns(new MartenQueryableStub<InventoryStockUnitDetails>());

        // When
        await enterProductInStockHandler.Handle(enterProductInStock, CancellationToken.None);

        // Then
        _inventoryStockUnitWriteRepository.AggregateStream
            .Select(unitEnteredInSock => unitEnteredInSock.Aggregate.ProductId).Should()
            .OnlyContain(productId => productIdsQuantities.Any(tuple => tuple.Item1 == productId));
    }

    private readonly IQuerySession _querySession = Substitute.For<IQuerySession>();
    private readonly DummyEventStoreRepository<InventoryStockUnit> _inventoryStockUnitWriteRepository = new();
}