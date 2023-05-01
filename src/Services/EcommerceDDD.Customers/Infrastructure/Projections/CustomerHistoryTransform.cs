namespace EcommerceDDD.Customers.Infrastructure.Projections;

public class CustomerHistoryTransform : EventProjection
{
    public CustomerEventHistory Transform(IEvent<CustomerRegistered> @event) =>
        CustomerEventHistory.Create(@event, @event.Data.CustomerId);

    public CustomerEventHistory Transform(IEvent<CustomerUpdated> @event) =>
        CustomerEventHistory.Create(@event, @event.Data.CustomerId);
}
