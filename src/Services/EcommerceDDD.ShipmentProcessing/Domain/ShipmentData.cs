namespace EcommerceDDD.ShipmentProcessing.Domain;

public record class ShipmentData(
    OrderId OrderId, 
    IReadOnlyList<ProductItem> ProductItems);