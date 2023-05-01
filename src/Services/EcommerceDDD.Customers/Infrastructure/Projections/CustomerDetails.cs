namespace EcommerceDDD.Customers.Infrastructure.Projections;

public class CustomerDetails
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string ShippingAddress { get; set; }
    public decimal CreditLimit { get; set; }

    internal void Apply(CustomerRegistered registered)
    {
        Id = registered.CustomerId;
        Email = registered.Email;
        Name = registered.Name;
        ShippingAddress = registered.ShippingAddress;
        CreditLimit = registered.CreditLimit;
    }

    internal void Apply(CustomerUpdated updated)
    {
        Name = updated.Name;
        ShippingAddress = updated.ShippingAddress;
        CreditLimit = updated.CreditLimit;
    }
}