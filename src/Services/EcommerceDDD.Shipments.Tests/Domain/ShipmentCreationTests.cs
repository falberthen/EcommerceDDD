using EcommerceDDD.Shipments.Domain;

namespace EcommerceDDD.Shipments.Tests.Domain;

public class ShipmentCreationTests
{
    [Fact]
    public void Create_FromOrder_ReturnsPaymentShipped()
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
        shipment.ShippedAt.Should().NotBe(null);
        shipment.DeliveredAt.Should().Be(null);
        shipment.ProductItems.Count().Should().Be(productItems.Count());
        shipment.Status.Should().Be(ShipmentStatus.Shipped);
    }
}