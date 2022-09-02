using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Customers.Domain.Events;

public record class CustomerUpdated(
    CustomerId CustomerId,
    string Name, 
    string Address) : IDomainEvent
{
    public static CustomerUpdated Create(
        CustomerId customerId,
        string name, 
        string address)
    {
        return new CustomerUpdated(customerId, name, address);
    }
}
