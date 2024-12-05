namespace EcommerceDDD.InventoryManagement.Tests.Application;

public class DecreaseQuantityInStockHandlerTests
{
	[Fact]
	public async Task DecreaseStockQuantity_WithValidCommand_ShouldDecreaseStock()
	{
		// Given
		var productId = Guid.NewGuid();
		var existingEntry = new InventoryStockUnitDetails
		{
			Id = Guid.NewGuid(),
			ProductId = productId,
			QuantityInStock = 10
		};

		// Mocking query session  
		var querySessionMock = Substitute.For<IQuerySessionWrapper>();
		var queryableData = new List<InventoryStockUnitDetails> { existingEntry }.AsQueryable();
		querySessionMock.Query<InventoryStockUnitDetails>()
			.Returns(queryableData);
		
		var inventoryStockUnit = InventoryStockUnit.EnterStockUnit(ProductId.Of(productId), 10);
		_inventoryStockUnitRepository.FetchStreamAsync(existingEntry.Id)
			.Returns(inventoryStockUnit);

		var handler = new DecreaseStockQuantityHandler(querySessionMock, _inventoryStockUnitRepository);
		var command = DecreaseStockQuantity.Create(ProductId.Of(productId), 2);

		// When
		await handler.Handle(command, CancellationToken.None);

		// Then
		Assert.Equal(8, inventoryStockUnit.Quantity); // 10 - 2
	}

	private readonly IEventStoreRepository<InventoryStockUnit> _inventoryStockUnitRepository = 
		Substitute.For<IEventStoreRepository<InventoryStockUnit>>();
}