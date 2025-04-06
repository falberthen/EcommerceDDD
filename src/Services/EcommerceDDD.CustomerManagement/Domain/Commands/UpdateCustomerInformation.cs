namespace EcommerceDDD.CustomerManagement.Domain.Commands;

public record class UpdateCustomerInformation : ICommand
{
    public string Name { get; private set; }
    public string ShippingAddress { get; private set; }
    public decimal CreditLimit { get; private set; }

    public static UpdateCustomerInformation Create(        
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

        return new UpdateCustomerInformation(            
            name, 
            shippingAddress,
            creditLimit);
    }

    private UpdateCustomerInformation(
        string name,
        string shippingAddress,
        decimal creditLimit)
    {
        Name = name;
        ShippingAddress = shippingAddress;
        CreditLimit = creditLimit;
    }
}
