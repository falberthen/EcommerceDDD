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
        inventoryStockUnit.ProductId.Value.Should().Be(productId.Value);
        inventoryStockUnit.Quantity.Should().Be(initialQuantity);

        var @event = inventoryStockUnit.GetUncommittedEvents()
            .LastOrDefault() as UnitEnteredInStock;
        Assert.NotNull(@event);
        @event.Should().BeOfType<UnitEnteredInStock>();
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
        @event.Should().BeOfType<StockQuantityDecreased>();
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
        @event.Should().BeOfType<StockQuantityIncreased>();
    }
}