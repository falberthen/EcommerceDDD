using MediatR;
using EcommerceDDD.Core.Testing;
using EcommerceDDD.Shipments.Domain;
using EcommerceDDD.Shipments.Domain.Events;
using EcommerceDDD.Shipments.Application.ShippingPackage;
using EcommerceDDD.Shipments.Application.DeliveringPackage;

namespace EcommerceDDD.Shipments.Tests.Application;

public class PackageShippedHandlerTests
{
    [Fact]
    public async Task PackageShipped_WithDomainEvent_ShouldDeliverPackage()
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
        await shipPackageHandler.Handle(shipPackage, CancellationToken.None);

        var shipment = shipmentWriteRepository.AggregateStream.First().Aggregate;
        var @event = shipment.GetUncommittedEvents().LastOrDefault() as PackageShipped;
        Assert.NotNull(@event);

        var packageShippedHandler = new PackageShippedHandler(_mediator.Object, shipmentWriteRepository);

        // When
        await packageShippedHandler.Handle(@event, CancellationToken.None);

        // Then
        Assert.NotNull(shipment);
        shipment.OrderId.Should().Be(orderId);
        shipment.ProductItems.Count().Should().Be(productItems.Count());
        shipment.ShippedAt.Should().NotBe(null);
        shipment.DeliveredAt.Should().NotBe(null);
        shipment.Status.Should().Be(ShipmentStatus.Delivered);
    }

    private Mock<IMediator> _mediator = new();
}