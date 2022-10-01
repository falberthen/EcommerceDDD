using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Customers.Domain.Commands;

public record class UpdateCustomerInformation : ICommand
{
    public CustomerId CustomerId { get; private set; }
    public string Name { get; private set; }
    public string ShippingAddress { get; private set; }
    public decimal CreditLimit { get; private set; }

    public static UpdateCustomerInformation Create(
        CustomerId customerId,
        string name,
        string shippingAddress,
        decimal creditLimit)
    {
        if (customerId is null)
            throw new ArgumentNullException(nameof(customerId));
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));
        if (string.IsNullOrEmpty(shippingAddress))
            throw new ArgumentNullException(nameof(shippingAddress));
        if (creditLimit <= 0)
            throw new ArgumentOutOfRangeException(nameof(creditLimit));

        return new UpdateCustomerInformation(
            customerId, 
            name, 
            shippingAddress,
            creditLimit);
    }

    private UpdateCustomerInformation(
        CustomerId customerId,
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
