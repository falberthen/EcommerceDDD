namespace EcommerceDDD.InventoryManagement.Tests.Application;

public class CheckProductsInStockHandlerTests
{
	[Fact]
	public async Task CheckProductsInStock_WithProductIds_ShouldReturnAllProductsWithQuantityInStock()
	{
		// Given
		Random random = new Random();
		var productIds = Enumerable.Range(1, 10)
			.Select(_ => ProductId.Of(Guid.NewGuid()))
			.ToList();

		var stockUnitDetails = productIds
			.Select(vm => new InventoryStockUnitDetails
			{
				ProductId = vm.Value,
				QuantityInStock = random.Next(1, 101)
			}).ToList();

		var querySessionMock = Substitute.For<IQuerySessionWrapper>();
		querySessionMock.QueryListAsync<InventoryStockUnitDetails>(
				Arg.Any<Expression<Func<InventoryStockUnitDetails, bool>>>(),
				Arg.Any<CancellationToken>())
			.Returns((IReadOnlyList<InventoryStockUnitDetails>)stockUnitDetails);

		var checkProductsInStock = CheckProductsInStock.Create(productIds);
		var checkProductsInStockHandler = new CheckProductsInStockHandler(querySessionMock);

		// When
		var result = await checkProductsInStockHandler
			.HandleAsync(checkProductsInStock, CancellationToken.None);

		// Then
		Assert.True(result.IsSuccess);
		Assert.All(result.Value!, viewModel =>
		{
			Assert.Contains(productIds, pid => pid.Value == viewModel.ProductId);
			Assert.True(viewModel.QuantityInStock > 0);
		});
	}
}
