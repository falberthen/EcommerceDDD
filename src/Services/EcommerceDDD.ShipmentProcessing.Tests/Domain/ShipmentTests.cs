namespace EcommerceDDD.ShipmentProcessing.Tests.Domain;

public class ShipmentTests
{
    [Fact]
    public void Create_WithShipmentData_ShouldCreateShipment()
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

    [Fact]
    public void Complete_WithShipment_ShouldCompleteShipment()
    {
        // Given
        var orderId = OrderId.Of(Guid.NewGuid());
        var productItems = new List<ProductItem>() {
            new ProductItem(ProductId.Of(Guid.NewGuid()), 5),
            new ProductItem(ProductId.Of(Guid.NewGuid()), 1),
            new ProductItem(ProductId.Of(Guid.NewGuid()), 1)
        };
        var shipmentData = new ShipmentData(orderId, productItems);
        var shipment = Shipment.Create(shipmentData);

        // When
        shipment.Complete();

        // Then
        Assert.NotNull(shipment);
        shipment.OrderId.Value.Should().Be(orderId.Value);
        shipment.CreatedAt.Should().NotBe(null);
        shipment.ShippedAt.Should().NotBe(null);
        shipment.ProductItems.Count().Should().Be(productItems.Count());
        shipment.Status.Should().Be(ShipmentStatus.Shipped);
    }

    [Fact]
    public void Create_WithEmptyProductItems_ShouldThrowException()
    {
        // Given
        var orderId = OrderId.Of(Guid.NewGuid());
        var productItems = new List<ProductItem>();                    
        var shipmentData = new ShipmentData(orderId, productItems);

        // When
        Action action = () =>
            Shipment.Create(shipmentData);

        // Then
        action.Should().Throw<BusinessRuleException>();
    }
}