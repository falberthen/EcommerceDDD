using PackageShipped = EcommerceDDD.ShipmentProcessing.Domain.Events.PackageShipped;

namespace EcommerceDDD.ShipmentProcessing.Tests.Domain;

public class ShipmentEventsTests
{
    [Fact]
    public void CreateShipment_WithShipmentData_ReturnsShipmentCreatedEvent()
    {
        // Given
        var orderId = OrderId.Of(Guid.NewGuid());
        var shipmentData = new ShipmentData(orderId, _productItems);

        // When
        var shipment = Shipment.Create(shipmentData);

        // Then
        var @event = shipment.GetUncommittedEvents().LastOrDefault() as ShipmentCreated;
        Assert.NotNull(@event);
		Assert.IsType<ShipmentCreated>(@event);
    }

    [Fact]
    public void CompleteShipment_WithShipmentData_ReturnsPackageShippedEvent()
    {
        // Given
        var orderId = OrderId.Of(Guid.NewGuid());
        var shipmentData = new ShipmentData(orderId, _productItems);
        var shipment = Shipment.Create(shipmentData);

        // When
        shipment.Complete();

        // Then
        var @event = shipment.GetUncommittedEvents().LastOrDefault() as PackageShipped;
        Assert.NotNull(@event);
		Assert.IsType<PackageShipped>(@event);
    }

    List<ProductItem> _productItems = new List<ProductItem>() {
        new ProductItem(ProductId.Of(Guid.NewGuid()), 5),
        new ProductItem(ProductId.Of(Guid.NewGuid()), 1),
        new ProductItem(ProductId.Of(Guid.NewGuid()), 1)
    };
}