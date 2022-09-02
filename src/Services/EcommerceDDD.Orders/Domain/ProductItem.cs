namespace EcommerceDDD.Orders.Domain;

public record ProductItem(ProductId ProductId, string ProductName, Money UnitPrice, int Quantity);