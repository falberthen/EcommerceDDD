namespace EcommerceDDD.CustomerManagement.Domain.Events;

public record class CustomerRegistered(
    Guid CustomerId,
    string Name,
    string Email,
    string ShippingAddress,
    decimal CreditLimit) : DomainEvent;
