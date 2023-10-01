namespace EcommerceDDD.Shipments.Tests.Application;

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

        _availabilityChecker.EnsureProductsInStock(Arg.Any<IReadOnlyList<ProductItem>>())
            .Returns(Task.FromResult(true));

        var requestShipment = RequestShipment.Create(orderId, productItems);
        var requestShipmentHandler = new RequestShipmentHandler(_commandBus, 
            _availabilityChecker, shipmentWriteRepository, _outboxMessageService);

        // When
        await requestShipmentHandler.Handle(requestShipment, CancellationToken.None);

        // Then
        var shipment = shipmentWriteRepository.AggregateStream.First().Aggregate;
        Assert.NotNull(shipment);
        shipment.OrderId.Should().Be(orderId);
        shipment.ProductItems.Count().Should().Be(productItems.Count());
        shipment.CreatedAt.Should().NotBe(null);
        shipment.ShippedAt.Should().Be(null);
        shipment.Status.Should().Be(ShipmentStatus.Pending);
    }

    private ICommandBus _commandBus = Substitute.For<ICommandBus>();
    private IOutboxMessageService _outboxMessageService = Substitute.For<IOutboxMessageService>();
    private IProductAvailabilityChecker _availabilityChecker = Substitute.For<IProductAvailabilityChecker>();
}