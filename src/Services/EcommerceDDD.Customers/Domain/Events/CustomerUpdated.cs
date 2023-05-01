namespace EcommerceDDD.Customers.Domain.Events;

public record class CustomerUpdated : IDomainEvent
{
    public Guid CustomerId { get; private set; }
    public string Name { get; private set; }
    public string ShippingAddress { get; private set; }
    public decimal CreditLimit;

    public static CustomerUpdated Create(
        Guid customerId,
        string name, 
        string shippingAddress,
        decimal creditLimit)
    {        
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));        
        if (string.IsNullOrEmpty(shippingAddress))
            throw new ArgumentNullException(nameof(shippingAddress));
        if (creditLimit <= 0)
            throw new ArgumentOutOfRangeException(nameof(creditLimit));

        return new CustomerUpdated(
            customerId, 
            name, 
            shippingAddress,
            creditLimit);
    }

    private CustomerUpdated(
        Guid customerId,
        string name,
        string shippingAddress,
        decimal creditLimit)
    {
        CustomerId = customerId;
        Name = name;
        ShippingAddress = shippingAddress;
        CreditLimit = creditLimit;
    }
}