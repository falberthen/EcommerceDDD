using Marten.Events.Aggregation;
using EcommerceDDD.Shipments.Domain.Events;

namespace EcommerceDDD.Shipments.Infrastructure.Projections;

public class ShipmentDetailsProjection : SingleStreamAggregation<ShipmentDetails>
{
    public ShipmentDetailsProjection()
    {
        ProjectEvent<PackageShipped>((item, @event) => item.Apply(@event));
        ProjectEvent<PackageDelivered>((item, @event) => item.Apply(@event));
    }
}

//https://martendb.io/events/projections/aggregate-projections.html#aggregate-by-stream