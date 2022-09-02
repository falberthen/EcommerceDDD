namespace EcommerceDDD.IntegrationServices.Shipments.Requests;

public record class ShipOrderRequest(    
    Guid OrderId,
    IReadOnlyList<ProductItemRequest> ProductItems);

public record ProductItemRequest(
    Guid ProductId, 
    string ProductName, 
    decimal UnitPrice, 
    int Quantity);