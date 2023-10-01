namespace EcommerceDDD.Shipments.Tests.Application;

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

        _availabilityChecker.EnsureProductsInStock(Arg.Any<IReadOnlyList<ProductItem>>())
            .Returns(Task.FromResult(true));

        var requestShipment = RequestShipment.Create(orderId, productItems);
        var requestShipmentHandler = new RequestShipmentHandler(_commandBus, _availabilityChecker,
            shipmentWriteRepository, _outboxMessageService);
        await requestShipmentHandler.Handle(requestShipment, CancellationToken.None);
        var shipment = shipmentWriteRepository.AggregateStream.First().Aggregate;
        Assert.NotNull(shipment);

        var shipPackage = ShipPackage.Create(shipment.Id);
        var shipPackageHandler = new ShipPackageHandler(shipmentWriteRepository, _outboxMessageService);

        // When
        await shipPackageHandler.Handle(shipPackage, CancellationToken.None);

        // Then
        var shippedPackage = shipmentWriteRepository.AggregateStream.First().Aggregate;
        Assert.NotNull(shippedPackage);
        shipment.OrderId.Should().Be(orderId);
        shipment.ProductItems.Count().Should().Be(productItems.Count());
        shipment.CreatedAt.Should().NotBe(null);
        shipment.ShippedAt.Should().NotBe(null);
        shipment.Status.Should().Be(ShipmentStatus.Shipped);
    }

    private ICommandBus _commandBus = Substitute.For<ICommandBus>();
    private IOutboxMessageService _outboxMessageService = Substitute.For<IOutboxMessageService>();
    private IProductAvailabilityChecker _availabilityChecker = Substitute.For<IProductAvailabilityChecker>();
}