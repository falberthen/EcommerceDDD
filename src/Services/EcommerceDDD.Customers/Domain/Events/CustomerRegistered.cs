using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Customers.Domain.Events;

public record class CustomerRegistered(
    CustomerId CustomerId,
    string Name, 
    string Email, 
    string Address) : IDomainEvent
{
    public static CustomerRegistered Create(Customer customer)
    {
        return new CustomerRegistered(
            customer.Id, 
            customer.Name, 
            customer.Email, 
            customer.Address);            
    }
}