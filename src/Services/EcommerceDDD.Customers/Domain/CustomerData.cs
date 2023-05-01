namespace EcommerceDDD.Customers.Domain;

public record class CustomerData(
    string Email,
    string Name,
    string ShippingAddress,
    decimal CreditLimit);