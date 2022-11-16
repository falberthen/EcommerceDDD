using EcommerceDDD.Core.Testing;
using EcommerceDDD.Shipments.Domain;
using EcommerceDDD.Shipments.Domain.Commands;
using EcommerceDDD.Core.Infrastructure.Outbox.Services;
using EcommerceDDD.Shipments.Application.RequestingShipment;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Shipments.Application.ShippingPackage;

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

        _availabilityChecker.Setup(p => p.EnsureProductsInStock(It.IsAny<IReadOnlyList<ProductItem>>()))
            .Returns(Task.FromResult(true));

        var requestShipment = RequestShipment.Create(orderId, productItems);
        var requestShipmentHandler = new RequestShipmentHandler(_commandBus.Object, _availabilityChecker.Object, shipmentWriteRepository, _outboxMessageService.Object);
        await requestShipmentHandler.Handle(requestShipment, CancellationToken.None);
        var shipment = shipmentWriteRepository.AggregateStream.First().Aggregate;
        Assert.NotNull(shipment);

        var shipPackage = ShipPackage.Create(shipment.Id);
        var shipPackageHandler = new ShipPackageHandler(shipmentWriteRepository, _outboxMessageService.Object);

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

    private Mock<ICommandBus> _commandBus = new();
    private Mock<IOutboxMessageService> _outboxMessageService = new();
    private Mock<IProductAvailabilityChecker> _availabilityChecker = new();
}