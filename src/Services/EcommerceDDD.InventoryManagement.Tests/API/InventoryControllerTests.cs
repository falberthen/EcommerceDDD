namespace EcommerceDDD.InventoryManagement.Tests.API;

public class InventoryControllerTests
{
    public InventoryControllerTests()
    {
        _inventoryController = new InventoryController(_bus);
		_inventoryInternalController = new InventoryInternalController(_bus);
	}

    [Fact]
    public async Task ListHistory_WithProductId_ShouldReturnListOfInventoryStockUnitEventHistory()
    {
        // Given
        var productId = Guid.NewGuid();
        var inventoryStockUnitId = Guid.NewGuid();
        var expectedData = new List<InventoryStockUnitEventHistory>
        {
            new InventoryStockUnitEventHistory(
                Guid.NewGuid(),
                inventoryStockUnitId,
                typeof(UnitEnteredInStock).Name,
                "event data",
                DateTime.UtcNow
            ),
            new InventoryStockUnitEventHistory(
                Guid.NewGuid(),
                inventoryStockUnitId,
                typeof(StockQuantityDecreased).Name,
                "event data",
                DateTime.UtcNow
            )
        };

        _bus.InvokeAsync<Result<IReadOnlyList<InventoryStockUnitEventHistory>>>(Arg.Any<GetInventoryStockUnitEventHistory>(), Arg.Any<CancellationToken>(), Arg.Any<TimeSpan?>())
            .Returns(Result.Ok<IReadOnlyList<InventoryStockUnitEventHistory>>(expectedData));

        // When
        var response = await _inventoryController
            .ListHistory(productId, CancellationToken.None);

		// Then
		var okResult = Assert.IsType<OkObjectResult>(response);
		Assert.IsAssignableFrom<IReadOnlyList<InventoryStockUnitEventHistory>>(okResult.Value);
	}

	#region INTERNAL
	[Fact]
	public async Task DecreaseQuantity_WithDecreasedQuantity_ShouldDecreaseQuantity()
	{
		// Given
		Guid productId = Guid.NewGuid();
		var request = new DecreaseQuantityInStockRequest()
		{
			DecreasedQuantity = 3
		};

		_bus.InvokeAsync<Result>(Arg.Any<DecreaseStockQuantity>(), Arg.Any<CancellationToken>(), Arg.Any<TimeSpan?>())
			.Returns(Result.Ok());

		// When
		var response = await _inventoryInternalController
			.DecreaseQuantity(productId, request, CancellationToken.None);

		// Then
		Assert.IsType<OkResult>(response);
	}

	[Fact]
	public async Task CheckStockQuantity_WithProductIds_ShouldReturnListOfInventoryStockUnitViewModel()
	{
		// Given
		var inventoryStockUnitId = Guid.NewGuid();
		var quantityInStock = 10;

		var request = new CheckProductsInStockRequest()
		{
			ProductIds = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()]
		};

		var expectedData = request.ProductIds.Select(pid => new
			InventoryStockUnitViewModel(inventoryStockUnitId, pid, quantityInStock)
		).ToList();

		_bus.InvokeAsync<Result<IReadOnlyList<InventoryStockUnitViewModel>>>(Arg.Any<CheckProductsInStock>(), Arg.Any<CancellationToken>(), Arg.Any<TimeSpan?>())
			.Returns(Result.Ok<IReadOnlyList<InventoryStockUnitViewModel>>(expectedData));

		// When
		var response = await _inventoryInternalController
			.CheckStockQuantity(request, CancellationToken.None);

		// Then
		var okResult = Assert.IsType<OkObjectResult>(response);
		Assert.IsAssignableFrom<IList<InventoryStockUnitViewModel>>(okResult.Value);
	}
	#endregion

	private IMessageBus _bus = Substitute.For<IMessageBus>();
    private InventoryController _inventoryController;
	private InventoryInternalController _inventoryInternalController;
}
