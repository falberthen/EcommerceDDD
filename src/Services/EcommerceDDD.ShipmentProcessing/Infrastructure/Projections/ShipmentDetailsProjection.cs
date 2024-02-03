using PackageShipped = EcommerceDDD.ShipmentProcessing.Domain.Events.PackageShipped;

namespace EcommerceDDD.ShipmentProcessing.Infrastructure.Projections;

public class ShipmentDetailsProjection : SingleStreamProjection<ShipmentDetails>
{
    public ShipmentDetailsProjection()
    {
        ProjectEvent<ShipmentCreated>((item, @event) => item.Apply(@event));
        ProjectEvent<PackageShipped>((item, @event) => item.Apply(@event));
    }
}

//https://martendb.io/events/projections/aggregate-projections.html#aggregate-by-stream