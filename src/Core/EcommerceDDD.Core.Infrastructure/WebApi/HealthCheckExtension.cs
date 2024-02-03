namespace EcommerceDDD.Core.Infrastructure.WebApi;

public static class HealthCheckExtension
{
    public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/health");
        });

        return app;
    }
}
