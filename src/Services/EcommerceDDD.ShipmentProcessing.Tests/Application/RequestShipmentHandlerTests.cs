namespace EcommerceDDD.ShipmentProcessing.Tests.Application;

public class RequestShipmentHandlerTests
{
    [Fact]
    public async Task RequestShipment_WithCommand_ShouldCreatePendingShipment()
    {
        // Given
        var orderId = OrderId.Of(Guid.NewGuid());
        var productItems = new List<ProductItem>() {
            new ProductItem(ProductId.Of(Guid.NewGuid()), 5),
            new ProductItem(ProductId.Of(Guid.NewGuid()), 1),
            new ProductItem(ProductId.Of(Guid.NewGuid()), 1)
        };

        var shipmentWriteRepository = new DummyEventStoreRepository<Shipment>();
        var requestShipment = RequestShipment.Create(orderId, productItems);
        var requestShipmentHandler = new RequestShipmentHandler(_commandBus, shipmentWriteRepository);

        // When
        await requestShipmentHandler.HandleAsync(requestShipment, CancellationToken.None);

        // Then
        var shipment = shipmentWriteRepository.AggregateStream.First().Aggregate;
        Assert.NotNull(shipment);        
		Assert.Equal(shipment.OrderId, orderId);
		Assert.Equal(shipment.ProductItems.Count(), productItems.Count());
		Assert.NotEqual(default(DateTime), shipment.CreatedAt);
		Assert.Null(shipment.ShippedAt);
		Assert.Equal(ShipmentStatus.Pending, shipment.Status);
	}

    private ICommandBus _commandBus = Substitute.For<ICommandBus>();    
}