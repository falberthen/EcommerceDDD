namespace EcommerceDDD.ShipmentProcessing.Domain;

public record class ShipmentData(
    OrderId OrderId,
    string ShippingAddress,
    IReadOnlyList<ProductItem> ProductItems);