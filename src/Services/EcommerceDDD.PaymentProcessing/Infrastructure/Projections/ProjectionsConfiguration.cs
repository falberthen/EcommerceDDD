namespace EcommerceDDD.PaymentProcessing.Infrastructure.Projections;

public static class ProjectionsConfiguration
{
    internal static void ConfigureProjections(this StoreOptions options)
    {
        options.Projections.Add<PaymentDetailsProjection>(ProjectionLifecycle.Inline);

        // Inline projection, so the index is enforced in the same transaction as the events.
        // Two concurrent requests for one order cannot both create a payment.
        options.Schema.For<PaymentDetails>().UniqueIndex(x => x.OrderId);
    }
}