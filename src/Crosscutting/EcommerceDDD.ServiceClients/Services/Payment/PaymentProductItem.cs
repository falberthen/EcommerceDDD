namespace EcommerceDDD.ServiceClients.Services.Payment;

public record PaymentProductItem(Guid ProductId, string ProductName, int Quantity, double UnitPrice);
