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

        var shipment = Shipment.Create(new ShipmentData(orderId, "123 Main St", productItems));
        var shipmentWriteRepository = new DummyEventStoreRepository<Shipment>();
        await shipmentWriteRepository.AppendEventsAndCommitAsync(shipment);

        var shipPackage = ProcessShipment.Create(shipment.Id, orderId);
        var shipPackageHandler = new ProcessShipmentHandler(Substitute.For<IConfiguration>(), shipmentWriteRepository);

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

	[Fact]
	public async Task ProcessShipment_WhenAddressContainsUndeliverableMarker_ShouldCancelAndPublishShipmentNotDelivered()
	{
		// Given
		var orderId = OrderId.Of(Guid.NewGuid());
		var productItems = new List<ProductItem>() {
			new ProductItem(ProductId.Of(Guid.NewGuid()), 1)
		};

		// The shipping address carries the sentinel marker, which the carrier simulation treats as undeliverable.
		var shipment = Shipment.Create(new ShipmentData(orderId, "308 Permanent Redirect Ave", productItems));
		var shipmentWriteRepository = new DummyEventStoreRepository<Shipment>();
		await shipmentWriteRepository.AppendEventsAndCommitAsync(shipment);

		var configuration = Substitute.For<IConfiguration>();
		configuration["ShipmentFailureSimulation:UndeliverableAddressMarker"].Returns("308");
		var processShipmentHandler = new ProcessShipmentHandler(configuration, shipmentWriteRepository);

		// When
		await processShipmentHandler.HandleAsync(ProcessShipment.Create(shipment.Id, orderId), CancellationToken.None);

		// Then: shipment is canceled and the saga is notified via ShipmentNotDelivered (which cancels the order).
		Assert.Equal(ShipmentStatus.Canceled, shipment.Status);

		var published = Assert.IsType<ShipmentNotDelivered>(
			Assert.Single(shipmentWriteRepository.PublishedMessages));
		Assert.Equal(shipment.Id.Value, published.ShipmentId);
		Assert.Equal(orderId.Value, published.OrderId);
	}

	[Fact]
	public async Task ProcessShipment_WhenAlreadyShipped_ShouldSucceedWithoutChangingIt()
	{
		// Given: shipped by an earlier attempt
		var orderId = OrderId.Of(Guid.NewGuid());
		var shipment = Shipment.Create(new ShipmentData(orderId, "123 Main St",
			new List<ProductItem>() { new ProductItem(ProductId.Of(Guid.NewGuid()), 1) }));
		shipment.Complete();

		var shipmentWriteRepository = new DummyEventStoreRepository<Shipment>();
		await shipmentWriteRepository.AppendEventsAndCommitAsync(shipment);

		var processShipmentHandler = new ProcessShipmentHandler(Substitute.For<IConfiguration>(), shipmentWriteRepository);

		// When
		await processShipmentHandler.HandleAsync(
			ProcessShipment.Create(shipment.Id, orderId), CancellationToken.None);

		// Then
		Assert.Single(shipmentWriteRepository.AggregateStream);
		Assert.Empty(shipmentWriteRepository.PublishedMessages);
	}
}