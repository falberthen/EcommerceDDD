namespace EcommerceDDD.CustomerManagement.Infrastructure.Projections;

public partial class CustomerDetailsProjection : SingleStreamProjection<CustomerDetails, Guid>
{
    public static void Apply(CustomerDetails item, CustomerRegistered @event) => item.Apply(@event);
    public static void Apply(CustomerDetails item, CustomerUpdated @event) => item.Apply(@event);
}

//https://martendb.io/events/projections/aggregate-projections.html#aggregate-by-stream