namespace EcommerceDDD.InventoryManagement.Tests.Domain;

public class InventoryStockUnitEventsTests
{
    [Fact]
    public void EnterStockUnit_WithProductAndQuantity_ShouldApplyUnitEnteredInStockEvent()
    {
        // Given        
        var productId = ProductId.Of(Guid.NewGuid());
        var initialQuantity = 10;

        // When
        var inventoryStockUnit = InventoryStockUnit
            .EnterStockUnit(productId, initialQuantity);

        // Then
        Assert.NotNull(inventoryStockUnit);
		Assert.Equal(inventoryStockUnit.ProductId.Value, productId.Value);
		Assert.Equal(inventoryStockUnit.Quantity, initialQuantity);

        var @event = inventoryStockUnit.GetUncommittedEvents()
            .LastOrDefault() as UnitEnteredInStock;
        Assert.NotNull(@event);
		Assert.IsType<UnitEnteredInStock>(@event);
    }

    [Fact]
    public void DecreaseStockQuantity_WithDecreasedQuantity_ShouldApplyStockQuantityDecreasedEvent()
    {
        // Given        
        var productId = ProductId.Of(Guid.NewGuid());
        var initialQuantity = 10;

        var inventoryStockUnit = InventoryStockUnit
            .EnterStockUnit(productId, initialQuantity);

        // When
        inventoryStockUnit.DecreaseStockQuantity(5);

        // Then
        var @event = inventoryStockUnit.GetUncommittedEvents()
            .LastOrDefault() as StockQuantityDecreased;
        Assert.NotNull(@event);
		Assert.IsType<StockQuantityDecreased>(@event);
	}

    [Fact]
    public void IncreaseStockQuantity_WithDecreasedQuantity_ShouldApplyStockQuantityIncreasedEvent()
    {
        // Given        
        var productId = ProductId.Of(Guid.NewGuid());
        var initialQuantity = 10;

        var inventoryStockUnit = InventoryStockUnit
            .EnterStockUnit(productId, initialQuantity);

        // When
        inventoryStockUnit.IncreaseStockQuantity(5);

        // Then
        var @event = inventoryStockUnit.GetUncommittedEvents()
            .LastOrDefault() as StockQuantityIncreased;
        Assert.NotNull(@event);
		Assert.IsType<StockQuantityIncreased>(@event);
    }
}