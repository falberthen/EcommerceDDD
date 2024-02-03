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
            ProductId = productId,
            DecreasedQuantity = 3
        };

        await _commandBus.SendAsync(Arg.Any<DecreaseStockQuantity>(), CancellationToken.None);

        // When
        var response = await _inventoryController
            .DecreaseQuantity(request, CancellationToken.None);

        // Then
        response.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task IncreaseQuantity_WithIncreasedQuantity_ShouldIncreaseQuantity()
    {
        // Given
        Guid productId = Guid.NewGuid();
        var request = new IncreaseQuantityInStockRequest()
        {
            ProductId = productId,
            IncreasedQuantity = 3
        };

        await _commandBus.SendAsync(Arg.Any<IncreaseStockQuantity>(), CancellationToken.None);

        // When
        var response = await _inventoryController
            .IncreaseQuantity(request, CancellationToken.None);

        // Then
        response.Should().BeOfType<OkObjectResult>();
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
        response.Should().BeOfType<OkObjectResult>()
            .Subject.Value.Should().BeOfType<ApiResponse<IList<InventoryStockUnitViewModel>>>()
            .Subject.Data.Should().BeEquivalentTo(expectedData);
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
        response.Should().BeOfType<OkObjectResult>()
            .Subject.Value.Should().BeOfType<ApiResponse<IList<InventoryStockUnitEventHistory>>>()
            .Subject.Data.Should().BeEquivalentTo(expectedData);
    }

    private ICommandBus _commandBus = Substitute.For<ICommandBus>();
    private IQueryBus _queryBus = Substitute.For<IQueryBus>();
    private InventoryController _inventoryController;
}