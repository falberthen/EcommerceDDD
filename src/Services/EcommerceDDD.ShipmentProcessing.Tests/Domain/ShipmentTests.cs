using EcommerceDDD.Core.Exceptions.Types;

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
		Assert.Equal(shipment.OrderId.Value, orderId.Value);
		Assert.NotEqual(default(DateTime), shipment.CreatedAt); 
		Assert.Null(shipment.ShippedAt);
		Assert.Equal(shipment.ProductItems.Count(), productItems.Count());
		Assert.Equal(ShipmentStatus.Pending, shipment.Status);
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
		Assert.Equal(shipment.OrderId.Value, orderId.Value);
		Assert.NotEqual(default(DateTime), shipment.CreatedAt);
		Assert.NotNull(shipment.ShippedAt);
		Assert.Equal(shipment.ProductItems.Count(), productItems.Count());
		Assert.Equal(ShipmentStatus.Shipped, shipment.Status);
	}

    [Fact]
    public void Create_WithEmptyProductItems_ShouldThrowException()
    {
        // Given
        var orderId = OrderId.Of(Guid.NewGuid());
        var productItems = new List<ProductItem>();                    
        var shipmentData = new ShipmentData(orderId, productItems);

		// When & Then
		BusinessRuleException exception = Assert.Throws<BusinessRuleException>(() =>
			Shipment.Create(shipmentData));
	}
}