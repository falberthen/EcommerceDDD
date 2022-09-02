using Marten;
using EcommerceDDD.Customers.Application.GettingCustomerDetails;
using EcommerceDDD.Customers.Application.GettingCustomerEventHistory;
using Marten.Events.Projections;

namespace EcommerceDDD.Customers.API.Configurations;

public static class ProjectionsConfiguration
{
    internal static void ConfigureProjections(this StoreOptions options)
    {
        options.Projections.Add<CustomerDetailsProjection>(ProjectionLifecycle.Inline);
        options.Projections.Add<CustomerHistoryTransformation>();
    }
}