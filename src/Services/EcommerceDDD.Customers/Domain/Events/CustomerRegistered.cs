using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Customers.Domain.Events;

public record class CustomerRegistered(
    CustomerId CustomerId,
    string Name, 
    string Email, 
    string Address,
    decimal AvailableCreditLimit) : IDomainEvent
{
    public static CustomerRegistered Create(Customer customer, decimal availableCreditLimit)
    {
        return new CustomerRegistered(
            customer.Id, 
            customer.Name, 
            customer.Email, 
            customer.Address,
            availableCreditLimit);            
    }
}