using EcommerceDDD.Shipments.Domain;
using EcommerceDDD.Shipments.Domain.Events;

namespace EcommerceDDD.Shipments.Tests.Domain;

public class ShipmentEventsTests
{
    [Fact]
    public void SendShipment_ReturnsPackageSentEvent()
    {
        // Given
        var orderId = OrderId.Of(Guid.NewGuid());

        // When
        var shipment = Shipment.CreateNew(orderId, _productItems);

        // Then
        var @event = shipment.GetUncommittedEvents().LastOrDefault() as PackageShipped;
        Assert.NotNull(@event);
        @event.Should().BeOfType<PackageShipped>();
    }

    [Fact]
    public void DeliverPayment_ReturnsPackageDeliveredEvent()
    {
        // Given
        var orderId = OrderId.Of(Guid.NewGuid());        
        var shipment = Shipment.CreateNew(orderId, _productItems);

        // When
        shipment.RecordDelivery();

        // Then
        var @event = shipment.GetUncommittedEvents().LastOrDefault() as PackageDelivered;
        Assert.NotNull(@event);
        @event.Should().BeOfType<PackageDelivered>();
    }

    List<ProductItem> _productItems = new List<ProductItem>() {
        new ProductItem(ProductId.Of(Guid.NewGuid()), 5),
        new ProductItem(ProductId.Of(Guid.NewGuid()), 1),
        new ProductItem(ProductId.Of(Guid.NewGuid()), 1)
    };
}