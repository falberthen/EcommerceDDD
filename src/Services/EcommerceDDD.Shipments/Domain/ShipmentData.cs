namespace EcommerceDDD.Shipments.Domain;

public record class ShipmentData(
    OrderId OrderId, 
    IReadOnlyList<ProductItem> ProductItems);