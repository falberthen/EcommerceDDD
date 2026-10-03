namespace EcommerceDDD.ShipmentProcessing.Infrastructure.Projections;

public static class ProjectionsConfiguration
{
    internal static void ConfigureProjections(this StoreOptions options)
    {
        options.Projections.Add<ShipmentDetailsProjection>(ProjectionLifecycle.Inline);

		// Inline projection, so the index is enforced in the same transaction as the events.
		// Two concurrent requests for one order cannot both create a shipment.
		options.Schema.For<ShipmentDetails>().UniqueIndex(x => x.OrderId);
    }
}
