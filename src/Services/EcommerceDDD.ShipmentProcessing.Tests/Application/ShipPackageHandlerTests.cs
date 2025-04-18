namespace EcommerceDDD.ShipmentProcessing.Tests.Application;

public class ShipPackageHandlerTests
{
    [Fact]
    public async Task ShipPackage_WithCommand_ShouldShipPackage()
    {
        // Given
        var orderId = OrderId.Of(Guid.NewGuid());
        var productItems = new List<ProductItem>() {
            new ProductItem(ProductId.Of(Guid.NewGuid()), 5),
            new ProductItem(ProductId.Of(Guid.NewGuid()), 1),
            new ProductItem(ProductId.Of(Guid.NewGuid()), 1)
        };

        var shipmentWriteRepository = new DummyEventStoreRepository<Shipment>();

        _availabilityChecker.CheckProductsInStockAsync(Arg.Any<IReadOnlyList<ProductItem>>())
            .Returns(Task.FromResult(true));

        var requestShipment = RequestShipment.Create(orderId, productItems);
        var requestShipmentHandler = new RequestShipmentHandler(_commandBus, shipmentWriteRepository);
        await requestShipmentHandler.HandleAsync(requestShipment, CancellationToken.None);
        var shipment = shipmentWriteRepository.AggregateStream.First().Aggregate;
        Assert.NotNull(shipment);

        var shipPackage = ProcessShipment.Create(shipment.Id);
        var shipPackageHandler = new ProcessShipmentHandler(_availabilityChecker, 
            shipmentWriteRepository, _eventPublisher);

        // When
        await shipPackageHandler.HandleAsync(shipPackage, CancellationToken.None);

        // Then
        var shippedPackage = shipmentWriteRepository.AggregateStream.First().Aggregate;
		Assert.NotNull(shipment);
		Assert.Equal(shipment.OrderId, orderId);
		Assert.NotEqual(default(DateTime), shipment.CreatedAt);
		Assert.NotNull(shipment.ShippedAt);
		Assert.Equal(shipment.ProductItems.Count(), productItems.Count());
		Assert.Equal(ShipmentStatus.Shipped, shipment.Status);
	}

	private ICommandBus _commandBus = Substitute.For<ICommandBus>();    
    private IProductInventoryHandler _availabilityChecker = Substitute.For<IProductInventoryHandler>();
    private IEventBus _eventPublisher = Substitute.For<IEventBus>();
}