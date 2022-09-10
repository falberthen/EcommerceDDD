using EcommerceDDD.Customers.Domain.Events;
using Marten.Events.Aggregation;

namespace EcommerceDDD.Customers.Application.GettingCustomerDetails;

public class CustomerDetails
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public decimal AvailableCreditLimit { get; set; }

    internal void Apply(CustomerRegistered registered)
    {
        Id = registered.CustomerId.Value;
        Email = registered.Email;
        Name = registered.Name;
        Address = registered.Address;
        AvailableCreditLimit = registered.AvailableCreditLimit;
    }

    internal void Apply(CustomerUpdated updated)
    {
        Name = updated.Name;
        Address = updated.Address;
        AvailableCreditLimit = updated.AvailableCreditLimit;
    }
}


public class CustomerDetailsProjection : SingleStreamAggregation<CustomerDetails>
{
    public CustomerDetailsProjection()
    {
        ProjectEvent<CustomerRegistered>((item, @event) => item.Apply(@event));
        ProjectEvent<CustomerUpdated>((item, @event) => item.Apply(@event));
    }
}

//https://martendb.io/events/projections/aggregate-projections.html#aggregate-by-stream