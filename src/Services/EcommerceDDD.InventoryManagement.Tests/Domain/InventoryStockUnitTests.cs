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
		inventoryStockUnit.DecreaseStockQuantity(quantityToDecrease, Guid.NewGuid());

		// Then
		Assert.NotNull(inventoryStockUnit);
		Assert.Equal(inventoryStockUnit.ProductId.Value, productId.Value);
		Assert.Equal(inventoryStockUnit.Quantity, initialQuantity - quantityToDecrease);
	}

	[Fact]
	public void IncreaseStockQuantity_ForOrderThatDecreased_ShouldReturnItsQuantity()
	{
		// Given        
		var productId = ProductId.Of(Guid.NewGuid());
		var initialQuantity = 10;
		var quantity = 6;
		var orderId = Guid.NewGuid();

		var inventoryStockUnit = InventoryStockUnit
			.EnterStockUnit(productId, initialQuantity);
		inventoryStockUnit.DecreaseStockQuantity(quantity, orderId);

		// When
		inventoryStockUnit.IncreaseStockQuantity(quantity, orderId);

		// Then
		Assert.NotNull(inventoryStockUnit);
		Assert.Equal(inventoryStockUnit.ProductId.Value, productId.Value);
		Assert.Equal(initialQuantity, inventoryStockUnit.Quantity);
	}

	[Fact]
	public void DecreaseStockQuantity_RepeatedForSameOrder_ShouldDecreaseOnce()
	{
		// Given
		var inventoryStockUnit = InventoryStockUnit.EnterStockUnit(ProductId.Of(Guid.NewGuid()), 10);
		var orderId = Guid.NewGuid();

		// When
		inventoryStockUnit.DecreaseStockQuantity(3, orderId);
		inventoryStockUnit.DecreaseStockQuantity(3, orderId);

		// Then
		Assert.Equal(7, inventoryStockUnit.Quantity);
		Assert.Single(inventoryStockUnit.GetUncommittedEvents().OfType<StockQuantityDecreased>());
	}

	[Fact]
	public void IncreaseStockQuantity_RepeatedForSameOrder_ShouldIncreaseOnce()
	{
		// Given
		var inventoryStockUnit = InventoryStockUnit.EnterStockUnit(ProductId.Of(Guid.NewGuid()), 10);
		var orderId = Guid.NewGuid();
		inventoryStockUnit.DecreaseStockQuantity(3, orderId);

		// When
		inventoryStockUnit.IncreaseStockQuantity(3, orderId);
		inventoryStockUnit.IncreaseStockQuantity(3, orderId);

		// Then
		Assert.Equal(10, inventoryStockUnit.Quantity);
		Assert.Single(inventoryStockUnit.GetUncommittedEvents().OfType<StockQuantityIncreased>());
	}

	[Fact]
	public void IncreaseStockQuantity_ForOrderThatTookNothing_ShouldBeIgnored()
	{
		// Given
		var inventoryStockUnit = InventoryStockUnit.EnterStockUnit(ProductId.Of(Guid.NewGuid()), 10);

		// When
		inventoryStockUnit.IncreaseStockQuantity(3, Guid.NewGuid());

		// Then
		Assert.Equal(10, inventoryStockUnit.Quantity);
		Assert.Empty(inventoryStockUnit.GetUncommittedEvents().OfType<StockQuantityIncreased>());
	}

	[Fact]
	public void EnterStockUnit_WithInitialQuantityLessThanZero_ShouldThrowException()
	{
		// Given        
		var productId = ProductId.Of(Guid.NewGuid());
		var initialQuantity = -1;

		// When & Then
		Assert.Throws<DomainException>(() =>
			InventoryStockUnit.EnterStockUnit(productId, initialQuantity));
	}

	[Fact]
	public void IncreaseStockQuantity_WithIncreasedQuantityEqualsZero_ShouldThrowException()
	{
		// Given        
		var productId = ProductId.Of(Guid.NewGuid());
		var initialQuantity = 10;
		var inventoryStockUnit = InventoryStockUnit.EnterStockUnit(productId, initialQuantity);
		var orderId = Guid.NewGuid();
		inventoryStockUnit.DecreaseStockQuantity(1, orderId);

		// When & Then
		Assert.Throws<DomainException>(() =>
			inventoryStockUnit.IncreaseStockQuantity(0, orderId));
	}

	[Fact]
	public void DecreaseStockQuantity_WithIncreasedQuantityEqualsZero_ShouldThrowException()
	{
		// Given        
		var productId = ProductId.Of(Guid.NewGuid());
		var initialQuantity = 10;
		var inventoryStockUnit = InventoryStockUnit.EnterStockUnit(productId, initialQuantity);

		// When & Then
		Assert.Throws<DomainException>(() =>
			inventoryStockUnit.DecreaseStockQuantity(0, Guid.NewGuid()));
	}
}