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

		var querySessionMock = Substitute.For<IQuerySessionWrapper>();
		querySessionMock.QueryFirstOrDefaultAsync<InventoryStockUnitDetails>(
				Arg.Any<Expression<Func<InventoryStockUnitDetails, bool>>>(),
				Arg.Any<CancellationToken>())
			.Returns(existingEntry);

		var inventoryStockUnit = InventoryStockUnit.EnterStockUnit(ProductId.Of(productId), 10);
		_inventoryStockUnitRepository.FetchStreamAsync(existingEntry.Id)
			.Returns(inventoryStockUnit);

		var handler = new DecreaseStockQuantityHandler(querySessionMock, _inventoryStockUnitRepository);
		var command = DecreaseStockQuantity.Create(ProductId.Of(productId), 2);

		// When
		await handler.HandleAsync(command, CancellationToken.None);

		// Then
		Assert.Equal(8, inventoryStockUnit.Quantity); // 10 - 2
	}

	private readonly IEventStoreRepository<InventoryStockUnit> _inventoryStockUnitRepository =
		Substitute.For<IEventStoreRepository<InventoryStockUnit>>();
}
