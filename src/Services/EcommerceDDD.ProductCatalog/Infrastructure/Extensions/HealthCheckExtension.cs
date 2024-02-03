namespace EcommerceDDD.ProductCatalog.Infrastructure.Extensions;

public static class HealthCheckExtension
{
    public static IApplicationBuilder MapHealthChecks(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/health");
        });

        return app;
    }
}
