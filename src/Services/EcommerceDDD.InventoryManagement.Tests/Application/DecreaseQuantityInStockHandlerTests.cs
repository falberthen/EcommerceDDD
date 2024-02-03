namespace EcommerceDDD.InventoryManagement.Tests.Application;

public class DecreaseQuantityInStockHandlerTests
{
    [Fact]
    public async Task DecreaseStockQuantity_WithCommand_ShouldDecreaseProductInStock()
    {
        // Given
        var productId = ProductId.Of(Guid.NewGuid());
        var initialQuantity = 3;
        var quantityIncreased = 1;

        var inventoryStockUnit = InventoryStockUnit.EnterStockUnit(productId, initialQuantity);
        await _inventoryStockUnitWriteRepository.AppendEventsAsync(inventoryStockUnit);

        var decreaseQuantityInStock = DecreaseStockQuantity.Create(productId, quantityIncreased);
        var decreaseQuantityInStockHandler = new DecreaseStockQuantityHandler(
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
        await decreaseQuantityInStockHandler.Handle(decreaseQuantityInStock, CancellationToken.None);

        // Then
        var enteredInventoryStockUnit = _inventoryStockUnitWriteRepository
            .AggregateStream.First().Aggregate;
        enteredInventoryStockUnit.Quantity.Should().Be(initialQuantity - quantityIncreased);        
    }

    private readonly IQuerySession _querySession = Substitute.For<IQuerySession>();
    private readonly DummyEventStoreRepository<InventoryStockUnit> _inventoryStockUnitWriteRepository = new();
}