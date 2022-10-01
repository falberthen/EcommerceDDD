using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Customers.Domain.Events;

public record class CustomerRegistered : IDomainEvent
{
    public Guid CustomerId { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string ShippingAddress { get; private set; }
    public decimal CreditLimit { get; private set; }

    public static CustomerRegistered Create(
        Guid customerId,
        string name,
        string email,
        string shippingAddress,
        decimal creditLimit)
    {      
        if (customerId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(customerId));
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));
        if (string.IsNullOrEmpty(email))
            throw new ArgumentNullException(nameof(email));
        if (string.IsNullOrEmpty(shippingAddress))
            throw new ArgumentNullException(nameof(shippingAddress));
        if (creditLimit <= 0)
            throw new ArgumentOutOfRangeException(nameof(creditLimit));

        return new CustomerRegistered(
            customerId, 
            name, 
            email, 
            shippingAddress,
            creditLimit);            
    }

    private CustomerRegistered(
        Guid customerId,
        string name,
        string email,
        string shippingAddress,
        decimal creditLimit)
    {
        CustomerId = customerId;
        Name = name;
        Email = email;
        ShippingAddress = shippingAddress;
        CreditLimit = creditLimit;
    }
}