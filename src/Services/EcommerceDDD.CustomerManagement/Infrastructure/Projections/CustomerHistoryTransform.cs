namespace EcommerceDDD.CustomerManagement.Infrastructure.Projections;

public class CustomerHistoryTransform : EventProjection
{
    public CustomerEventHistory Transform(JasperFx.Events.IEvent<CustomerRegistered> @event) =>
        CustomerEventHistory.Create(@event, @event.Data.CustomerId);

    public CustomerEventHistory Transform(JasperFx.Events.IEvent<CustomerUpdated> @event) =>
        CustomerEventHistory.Create(@event, @event.Data.CustomerId);
}
