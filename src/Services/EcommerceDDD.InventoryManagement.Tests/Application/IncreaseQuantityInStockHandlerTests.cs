using EcommerceDDD.InventoryManagement.Application.IncreaseQuantityInStock;

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

		var inventoryStockUnitDetails = new InventoryStockUnitDetails()
		{
			Id = inventoryStockUnit.Id.Value,
			ProductId = productId.Value,
			QuantityInStock = inventoryStockUnit.Quantity
		};

		var querySessionMock = Substitute.For<IQuerySessionWrapper>();
		querySessionMock.Query<InventoryStockUnitDetails>()
			.Returns(new List<InventoryStockUnitDetails> { inventoryStockUnitDetails }.AsQueryable());

		_inventoryStockUnitRepository.FetchStreamAsync(inventoryStockUnit.Id.Value)
			.Returns(inventoryStockUnit);

		var increaseQuantityInStockHandler = new IncreaseQuantityInStockHandler(
			querySessionMock, _inventoryStockUnitRepository);

		var increaseQuantityInStock = IncreaseStockQuantity.Create(productId, quantityIncreased);

		// When
		await increaseQuantityInStockHandler.HandleAsync(increaseQuantityInStock, CancellationToken.None);

		// Then
		Assert.Equal(initialQuantity + quantityIncreased, inventoryStockUnit.Quantity);
	}

	private readonly IEventStoreRepository<InventoryStockUnit> _inventoryStockUnitRepository =
		Substitute.For<IEventStoreRepository<InventoryStockUnit>>();
}
