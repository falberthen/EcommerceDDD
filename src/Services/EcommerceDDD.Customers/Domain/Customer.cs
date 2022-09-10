using EcommerceDDD.Core.Domain;
using EcommerceDDD.Core.Exceptions;
using EcommerceDDD.Customers.Domain.Events;

namespace EcommerceDDD.Customers.Domain;

public sealed class Customer : AggregateRoot<CustomerId>
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Address { get; private set; }
    public DummyWallet Wallet { get; private set; }

    public static Customer CreateNew(string email, string name, string address,
        decimal availableCreditLimit, ICustomerUniquenessChecker checker)
    {
        if (!checker.IsUserUnique(email))
            throw new DomainException("This e-mail is already in use.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Customer name cannot be null or whitespace.");

        if (string.IsNullOrWhiteSpace(address))
            throw new DomainException("Address cannot be null or whitespace.");

        if (availableCreditLimit <= 0)
            throw new DomainException("Available credit limit must be greater than 0.");

        return new Customer(email, name, address, availableCreditLimit);
    }

    public void UpdateCustomerInfo(CustomerId customerId, string name, string address,
        decimal availableCreditLimit)
    {
        if (customerId == null)
            throw new DomainException("CustomerId is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Customer name cannot be null or whitespace.");

        if (string.IsNullOrWhiteSpace(address))
            throw new DomainException("Address cannot be null or whitespace.");

        if (availableCreditLimit <= 0)
            throw new DomainException("Available credit limit must be greater than 0.");

        var @event = CustomerUpdated.Create(customerId, name, address, 
            availableCreditLimit);

        AppendEvent(@event);
        Apply(@event);
    }

    private void Apply(CustomerRegistered registered)
    {
        Id = registered.CustomerId;
        Email = registered.Email;
        Name = registered.Name;
        Address = registered.Address;
        Wallet = DummyWallet.CreateNew(registered.AvailableCreditLimit);
    }

    private void Apply(CustomerUpdated updated)
    {
        Name = updated.Name;
        Address = updated.Address;
        Wallet = DummyWallet.CreateNew(updated.AvailableCreditLimit);
    }

    private Customer(string email, string name, string address, 
        decimal availableCreditLimit)
    {
        var @event = new CustomerRegistered(CustomerId.Create(), name, email, address,
            availableCreditLimit);

        AppendEvent(@event);
        Apply(@event);
    }

    private Customer() { }
}