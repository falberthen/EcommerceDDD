namespace EcommerceDDD.InventoryManagement.Tests.Application;

public class IncreaseQuantityInStockHandlerTests
{
    [Fact]
    public async Task IncreaseStockQuantity_WithCommand_ShouldIncreaseProductInStock()
    {
        // Given
        var productId = ProductId.Of(Guid.NewGuid());
        var initialQuantity = 1;
        var quantityIncreased = 2;

        var inventoryStockUnit = InventoryStockUnit.EnterStockUnit(productId, initialQuantity);
        await _inventoryStockUnitWriteRepository.AppendEventsAsync(inventoryStockUnit);

        var increaseQuantityInStock = IncreaseStockQuantity.Create(productId, quantityIncreased);
        var increaseQuantityInStockHandler = new IncreaseQuantityInStockHandler(
            _querySession, _inventoryStockUnitWriteRepository);

        var viewModel = new InventoryStockUnitDetails() 
        { 
            Id = inventoryStockUnit.Id.Value, 
            ProductId = productId.Value, 
            QuantityInStock = inventoryStockUnit.Quantity
        };
        var queryableStub = new MartenQueryableStub<InventoryStockUnitDetails>
        {
            viewModel
        };

        // Mocking query session  
        _querySession.Query<InventoryStockUnitDetails>()
            .Returns(queryableStub);

        // When
        await increaseQuantityInStockHandler.Handle(increaseQuantityInStock, CancellationToken.None);

        // Then
        var enteredInventoryStockUnit = _inventoryStockUnitWriteRepository
            .AggregateStream.First().Aggregate;
        enteredInventoryStockUnit.Quantity.Should().Be(initialQuantity + quantityIncreased);        
    }

    private readonly IQuerySession _querySession = Substitute.For<IQuerySession>();
    private readonly DummyEventStoreRepository<InventoryStockUnit> _inventoryStockUnitWriteRepository = new();
}