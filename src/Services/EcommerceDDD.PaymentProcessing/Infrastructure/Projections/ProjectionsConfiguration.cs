namespace EcommerceDDD.PaymentProcessing.Infrastructure.Projections;

public static class ProjectionsConfiguration
{
    internal static void ConfigureProjections(this StoreOptions options)
    {
        options.Projections.Add<PaymentDetailsProjection>(ProjectionLifecycle.Inline);
    }
}