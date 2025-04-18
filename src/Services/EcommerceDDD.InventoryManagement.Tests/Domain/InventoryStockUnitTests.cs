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
		Assert.Equal(inventoryStockUnit.ProductId.Value, productId.Value);
		Assert.Equal(inventoryStockUnit.Quantity, initialQuantity);
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
		Assert.Equal(inventoryStockUnit.ProductId.Value, productId.Value);
		Assert.Equal(inventoryStockUnit.Quantity, initialQuantity - quantityToDecrease);
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
		Assert.Equal(inventoryStockUnit.ProductId.Value, productId.Value);
		Assert.Equal(inventoryStockUnit.Quantity, initialQuantity + quantityToDecrease);
	}

	[Fact]
	public void EnterStockUnit_WithInitialQuantityLessThanZero_ShouldThrowException()
	{
		// Given        
		var productId = ProductId.Of(Guid.NewGuid());
		var initialQuantity = -1;

		// When & Then
		Assert.Throws<BusinessRuleException>(() =>
			InventoryStockUnit.EnterStockUnit(productId, initialQuantity));
	}

	[Fact]
	public void IncreaseStockQuantity_WithIncreasedQuantityEqualsZero_ShouldThrowException()
	{
		// Given        
		var productId = ProductId.Of(Guid.NewGuid());
		var initialQuantity = 10;
		var inventoryStockUnit = InventoryStockUnit.EnterStockUnit(productId, initialQuantity);

		// When & Then
		Assert.Throws<BusinessRuleException>(() =>
			inventoryStockUnit.IncreaseStockQuantity(0));
	}

	[Fact]
	public void DecreaseStockQuantity_WithIncreasedQuantityEqualsZero_ShouldThrowException()
	{
		// Given        
		var productId = ProductId.Of(Guid.NewGuid());
		var initialQuantity = 10;
		var inventoryStockUnit = InventoryStockUnit.EnterStockUnit(productId, initialQuantity);

		// When & Then
		Assert.Throws<BusinessRuleException>(() =>
			inventoryStockUnit.DecreaseStockQuantity(0));
	}
}