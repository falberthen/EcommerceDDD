namespace EcommerceDDD.InventoryManagement.Tests.API;

public class InventoryControllerTests
{
    public InventoryControllerTests()
    {
        _inventoryController = new InventoryController(
            _commandBus, _queryBus);
    }

    [Fact]
    public async Task DecreaseQuantity_WithDecreasedQuantity_ShouldDecreaseQuantity()
    {
        // Given
        Guid productId = Guid.NewGuid();
        var request = new DecreaseQuantityInStockRequest()
        {
            DecreasedQuantity = 3
        };

        await _commandBus.SendAsync(Arg.Any<DecreaseStockQuantity>(), CancellationToken.None);

        // When
        var response = await _inventoryController
            .DecreaseQuantity(productId, request, CancellationToken.None);

		// Then
		Assert.IsType<OkObjectResult>(response);
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
        
        _queryBus.SendAsync(Arg.Any<CheckProductsInStock>(), Arg.Any<CancellationToken>())
            .Returns(expectedData);

        // When
        var response = await _inventoryController.CheckStockQuantity(request, CancellationToken.None);

		// Then
		var okResult = Assert.IsType<OkObjectResult>(response);
		var apiResponse = Assert.IsType<ApiResponse<IList<InventoryStockUnitViewModel>>>(okResult.Value);
		Assert.IsAssignableFrom<IList<InventoryStockUnitViewModel>>(apiResponse.Data);
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

        _queryBus.SendAsync(Arg.Any<GetInventoryStockUnitEventHistory>(), CancellationToken.None)
            .Returns(expectedData);

        // When
        var response = await _inventoryController
            .ListHistory(productId, CancellationToken.None);

		// Then
		var okResult = Assert.IsType<OkObjectResult>(response);
		var apiResponse = Assert.IsType<ApiResponse<IList<InventoryStockUnitEventHistory>>>(okResult.Value);
		Assert.IsAssignableFrom<IList<InventoryStockUnitEventHistory>>(apiResponse.Data);
	}

    private ICommandBus _commandBus = Substitute.For<ICommandBus>();
    private IQueryBus _queryBus = Substitute.For<IQueryBus>();
    private InventoryController _inventoryController;
}