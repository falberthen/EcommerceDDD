using MediatR;
using EcommerceDDD.Core.Testing;
using EcommerceDDD.Shipments.Application.ShippingPackage;
using EcommerceDDD.Shipments.Domain;

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

        var shipPackage = new ShipPackage(orderId, productItems);
        var shipPackageHandler = new ShipPackageHandler(_mediator.Object, shipmentWriteRepository);

        // When
        await shipPackageHandler.Handle(shipPackage, CancellationToken.None);

        // Then
        var shipment = shipmentWriteRepository.AggregateStream.First().Aggregate;
        Assert.NotNull(shipment);
        shipment.OrderId.Should().Be(orderId);
        shipment.ProductItems.Count().Should().Be(productItems.Count());
        shipment.DeliveredAt.Should().Be(null);
        shipment.ShippedAt.Should().NotBe(null);        
        shipment.Status.Should().Be(ShipmentStatus.Shipped);
    }

    private Mock<IMediator> _mediator = new();
}