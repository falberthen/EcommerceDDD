namespace EcommerceDDD.CustomerManagement.Infrastructure.Projections;

public static class ProjectionsConfiguration
{
    internal static void ConfigureProjections(this StoreOptions options)
    {
        options.Projections.Add<CustomerDetailsProjection>(ProjectionLifecycle.Inline);
        options.Projections.Add<CustomerHistoryTransform>(ProjectionLifecycle.Inline);

        // Inline projection, so the index is enforced in the same transaction as the events:
        // two concurrent registrations for one e-mail cannot both create a customer.
        options.Schema.For<CustomerDetails>().UniqueIndex(x => x.Email);
    }
}