using Marten.Events.Aggregation;
using EcommerceDDD.Customers.Domain.Events;

namespace EcommerceDDD.Customers.Infrastructure.Projections;

public class CustomerDetailsProjection : SingleStreamAggregation<CustomerDetails>
{
    public CustomerDetailsProjection()
    {
        ProjectEvent<CustomerRegistered>((item, @event) => item.Apply(@event));
        ProjectEvent<CustomerUpdated>((item, @event) => item.Apply(@event));
    }
}

//https://martendb.io/events/projections/aggregate-projections.html#aggregate-by-stream