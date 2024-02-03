namespace EcommerceDDD.InventoryManagement.Tests.Domain;

public class InventoryStockUnitTests
{
    [Fact]
    public void EnterStockUnit_WithProductAndQuantity_ShouldEnterStockUnit()
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
    }

    [Fact]
    public void DecreaseStockQuantity_WithQuantityToDecrease_ShouldDecreasesInventoryStockUnitQuantity()
    {
        // Given        
        var productId = ProductId.Of(Guid.NewGuid());
        var initialQuantity = 10;
        var quantityToDecrease = 6;

        var inventoryStockUnit = InventoryStockUnit
            .EnterStockUnit(productId, initialQuantity);

        // When
        inventoryStockUnit.DecreaseStockQuantity(quantityToDecrease);

        // Then
        Assert.NotNull(inventoryStockUnit);
        inventoryStockUnit.ProductId.Value.Should().Be(productId.Value);
        inventoryStockUnit.Quantity.Should().Be(initialQuantity - quantityToDecrease);
    }

    [Fact]
    public void IncreaseStockQuantity_WithQuantityToDecrease_ShouldIncreaseInventoryStockUnitQuantity()
    {
        // Given        
        var productId = ProductId.Of(Guid.NewGuid());
        var initialQuantity = 10;
        var quantityToDecrease = 6;

        var inventoryStockUnit = InventoryStockUnit
            .EnterStockUnit(productId, initialQuantity);

        // When
        inventoryStockUnit.IncreaseStockQuantity(quantityToDecrease);

        // Then
        Assert.NotNull(inventoryStockUnit);
        inventoryStockUnit.ProductId.Value.Should().Be(productId.Value);
        inventoryStockUnit.Quantity.Should().Be(initialQuantity + quantityToDecrease);
    }

    [Fact]
    public void EnterStockUnit_WithInitialQuantityLessThanZero_ShouldThrowException()
    {
        // Given        
        var productId = ProductId.Of(Guid.NewGuid());
        var initialQuantity = -1;

        // When
        Func<InventoryStockUnit> action = () =>
            InventoryStockUnit.EnterStockUnit(productId, initialQuantity);

        // Then
        action.Should().Throw<BusinessRuleException>();
    }

    [Fact]
    public void IncreaseStockQuantity_WithIncreasedQuantityEqualsZero_ShouldThrowException()
    {
        // Given        
        var productId = ProductId.Of(Guid.NewGuid());
        var initialQuantity = 10;
        var inventoryStockUnit = InventoryStockUnit
            .EnterStockUnit(productId, initialQuantity);

        // When
        Action action = () =>
            inventoryStockUnit.IncreaseStockQuantity(0);

        // Then
        action.Should().Throw<BusinessRuleException>();
    }

    [Fact]
    public void DecreaseStockQuantity_WithIncreasedQuantityEqualsZero_ShouldThrowException()
    {
        // Given        
        var productId = ProductId.Of(Guid.NewGuid());
        var initialQuantity = 10;
        var inventoryStockUnit = InventoryStockUnit
            .EnterStockUnit(productId, initialQuantity);

        // When
        Action action = () =>
            inventoryStockUnit.DecreaseStockQuantity(0);

        // Then
        action.Should().Throw<BusinessRuleException>();
    }
}