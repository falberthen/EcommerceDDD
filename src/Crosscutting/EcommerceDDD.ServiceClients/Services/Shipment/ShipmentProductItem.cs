namespace EcommerceDDD.ServiceClients.Services.Shipment;

public record ShipmentProductItem(Guid ProductId, string ProductName, int Quantity, double UnitPrice);
