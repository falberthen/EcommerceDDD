using PackageShipped = EcommerceDDD.ShipmentProcessing.Domain.Events.PackageShipped;

namespace EcommerceDDD.ShipmentProcessing.Infrastructure.Projections;

public partial class ShipmentDetailsProjection : SingleStreamProjection<ShipmentDetails, Guid>
{
    public static void Apply(ShipmentDetails item, ShipmentCreated @event) => item.Apply(@event);
    public static void Apply(ShipmentDetails item, PackageShipped @event) => item.Apply(@event);
}

//https://martendb.io/events/projections/aggregate-projections.html#aggregate-by-stream