using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Customers.Domain.Events;

public record class CustomerUpdated(
    CustomerId CustomerId,
    string Name, 
    string Address,
    decimal AvailableCreditLimit) : IDomainEvent
{
    public static CustomerUpdated Create(
        CustomerId customerId,
        string name, 
        string address,
        decimal availableCreditLimit)
    {
        return new CustomerUpdated(
            customerId, 
            name, 
            address, 
            availableCreditLimit);
    }
}
