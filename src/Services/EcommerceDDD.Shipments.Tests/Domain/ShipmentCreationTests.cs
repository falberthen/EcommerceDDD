using EcommerceDDD.Shipments.Domain;

namespace EcommerceDDD.Shipments.Tests.Domain;

public class ShipmentCreationTests
{
    [Fact]
    public void Create_FromOrder_ReturnsShipmentPending()
    {
        // Given
        var orderId = OrderId.Of(Guid.NewGuid());
        var productItems = new List<ProductItem>() {
            new ProductItem(ProductId.Of(Guid.NewGuid()), 5),
            new ProductItem(ProductId.Of(Guid.NewGuid()), 1),
            new ProductItem(ProductId.Of(Guid.NewGuid()), 1)
        };
        var shipmentData = new ShipmentData(orderId, productItems);

        // When
        var shipment = Shipment.Create(shipmentData);

        // Then
        Assert.NotNull(shipment);
        shipment.OrderId.Value.Should().Be(orderId.Value);
        shipment.CreatedAt.Should().NotBe(null);
        shipment.ShippedAt.Should().Be(null);
        shipment.ProductItems.Count().Should().Be(productItems.Count());
        shipment.Status.Should().Be(ShipmentStatus.Pending);
    }
}