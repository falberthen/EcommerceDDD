using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace EcommerceDDD.Core.Infrastructure.WebApi;

public static class HealthCheckExtension
{
	public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder app)
	{
		app.UseEndpoints(endpoints =>
		{
			endpoints.MapHealthChecks("/health", new HealthCheckOptions
			{
				ResponseWriter = async (context, report) =>
				{
					context.Response.ContentType = "application/json";

					var result = System.Text.Json.JsonSerializer.Serialize(new
					{
						status = report.Status.ToString(),
						results = report.Entries.Select(e => new
						{
							key = e.Key,
							status = e.Value.Status.ToString(),
							description = e.Value.Description
						})
					});

					await context.Response.WriteAsync(result);
				}
			});
		});

		return app;
	}
}
