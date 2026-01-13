namespace EcommerceDDD.CustomerManagement.Infrastructure.Projections;

public class CustomerDetailsProjection : SingleStreamProjection<CustomerDetails, Guid>
{
    public CustomerDetailsProjection()
    {
        ProjectEvent<CustomerRegistered>((item, @event) => item.Apply(@event));
        ProjectEvent<CustomerUpdated>((item, @event) => item.Apply(@event));
    }
}

//https://martendb.io/events/projections/aggregate-projections.html#aggregate-by-stream