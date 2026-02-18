namespace EcommerceDDD.CustomerManagement.Domain.Events;

public record class CustomerUpdated(
    Guid CustomerId,
    string Name,
    string ShippingAddress,
    decimal CreditLimit) : DomainEvent;
