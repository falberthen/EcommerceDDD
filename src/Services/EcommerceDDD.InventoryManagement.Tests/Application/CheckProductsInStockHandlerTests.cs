namespace EcommerceDDD.InventoryManagement.Tests.Application;

public class CheckProductsInStockHandlerTests
{
	[Fact]
	public async Task CheckProductsInStock_WithProductIds_ShouldReturnAllProductsWithQuantityInStock()
	{
		// Given
		Random random = new Random();
		var productIds = Enumerable.Range(1, 10)
			.Select(_ => ProductId.Of(Guid.NewGuid())).ToList();
		var stockUnitDetails = productIds
			.Select(vm => new InventoryStockUnitDetails()
			{
				ProductId = vm.Value,
				QuantityInStock = random.Next(1, 101)
			});
		
		// Mocking query session  
		var querySessionMock = Substitute.For<IQuerySessionWrapper>();
		var queryableData = stockUnitDetails.AsQueryable();
		querySessionMock.Query<InventoryStockUnitDetails>()
			.Returns(queryableData);

		var checkProductsInStock = CheckProductsInStock.Create(productIds);
		var checkProductsInStockHandler = new CheckProductsInStockHandler(querySessionMock);

		// When
		var result = await checkProductsInStockHandler
			.Handle(checkProductsInStock, CancellationToken.None);

		// Then
		result.Select(viewModel => viewModel.ProductId).Should()
			.OnlyContain(productId => productIds.Any(pid => pid.Value == productId));

		result.Select(viewModel => viewModel.QuantityInStock).Should()
			.OnlyContain(quantityInStock => quantityInStock > 0);
	}
}