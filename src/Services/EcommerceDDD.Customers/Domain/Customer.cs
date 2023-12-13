namespace EcommerceDDD.Customers.Domain;

public sealed class Customer : AggregateRoot<CustomerId>
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public Address ShippingAddress { get; private set; }
    public CreditLimit CreditLimit { get; private set; }

    public static Customer Create(CustomerData customerData)
    {
        var (Email, Name, ShippingAddress, CreditLimit) = customerData
            ?? throw new ArgumentNullException(nameof(customerData));

        if (string.IsNullOrWhiteSpace(Email))
            throw new BusinessRuleException("Customer email cannot be null or whitespace.");

        if (string.IsNullOrWhiteSpace(Name))
            throw new BusinessRuleException("Customer name cannot be null or whitespace.");

        if (string.IsNullOrWhiteSpace(ShippingAddress))
            throw new BusinessRuleException("Customer shipping address cannot be null or whitespace.");

        if (CreditLimit <= 0)
            throw new BusinessRuleException("Customer available credit limit must be greater than 0.");

        return new Customer(customerData);
    }

    public void UpdateCustomerInformation(CustomerData customerData)
    {
        var (Email, Name, ShippingAddress, CreditLimit) = customerData
            ?? throw new ArgumentNullException(nameof(customerData));

        if (string.IsNullOrWhiteSpace(customerData.Name))
            throw new BusinessRuleException("Customer name cannot be null or whitespace.");

        if (string.IsNullOrWhiteSpace(ShippingAddress))
            throw new BusinessRuleException("Customer shipping address cannot be null or whitespace.");

        if (CreditLimit <= 0)
            throw new BusinessRuleException("Available credit limit must be greater than 0.");

        var @event = CustomerUpdated.Create(
            Id.Value,
            Name,
            ShippingAddress,
            CreditLimit);

        AppendEvent(@event);
        Apply(@event);
    }

    private void Apply(CustomerRegistered registered)
    {
        Id = CustomerId.Of(registered.CustomerId);
        Email = registered.Email;
        Name = registered.Name;
        ShippingAddress = Address.FromStreetAddress(registered.ShippingAddress);
        CreditLimit = CreditLimit.Create(registered.CreditLimit);
    }

    private void Apply(CustomerUpdated updated)
    {
        Name = updated.Name;
        ShippingAddress = Address.FromStreetAddress(updated.ShippingAddress);
        CreditLimit = CreditLimit.Create(updated.CreditLimit);
    }

    private Customer(CustomerData customerData)
    {
        var @event = CustomerRegistered.Create(
            Guid.NewGuid(),
            customerData.Name,
            customerData.Email,
            customerData.ShippingAddress,
            customerData.CreditLimit);

        AppendEvent(@event);
        Apply(@event);
    }

    private Customer() {}
}