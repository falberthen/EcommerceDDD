namespace EcommerceDDD.InventoryManagement.Tests.Application;

public class EnterProductInStockHandlerTests
{
	[Fact]
	public async Task EnterProductInStock_WithCommand_ShouldEnterProductInStock()
	{
		// Given
		List<Tuple<ProductId, int>> productIdsQuantities = new()
		{
			new Tuple<ProductId, int>(ProductId.Of(Guid.NewGuid()), 1),
			new Tuple<ProductId, int>(ProductId.Of(Guid.NewGuid()), 1),
			new Tuple<ProductId, int>(ProductId.Of(Guid.NewGuid()), 1),
		};

		var querySessionMock = Substitute.For<IQuerySessionWrapper>();
		querySessionMock.QueryListAsync<InventoryStockUnitDetails>(
				Arg.Any<Expression<Func<InventoryStockUnitDetails, bool>>>(),
				Arg.Any<CancellationToken>())
			.Returns((IReadOnlyList<InventoryStockUnitDetails>)new List<InventoryStockUnitDetails>());

		var handler = new EnterProductInStockHandler(
			querySessionMock, _inventoryStockUnitRepository);

		var enterProductInStock = EnterProductInStock.Create(productIdsQuantities);

		// When
		await handler.HandleAsync(enterProductInStock, CancellationToken.None);

		// Then
		await _inventoryStockUnitRepository.Received(productIdsQuantities.Count)
			.AppendEventsAsync(Arg.Is<InventoryStockUnit>(unit =>
				productIdsQuantities.Any(tuple => tuple.Item1 == unit.ProductId)),
				Arg.Any<CancellationToken>());
	}

	private readonly IEventStoreRepository<InventoryStockUnit> _inventoryStockUnitRepository =
			Substitute.For<IEventStoreRepository<InventoryStockUnit>>();
}
