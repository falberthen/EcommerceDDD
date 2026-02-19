namespace EcommerceDDD.Core.Infrastructure.WebApi;

public static class ApiVersioningExtension
{
    public static IServiceCollection AddApiVersioning(
		this IServiceCollection services, string apiVersion = ApiVersions.V2)
    {
		services.AddApiVersioning(options =>
		{
			options.AssumeDefaultVersionWhenUnspecified = true;
			options.DefaultApiVersion = ApiVersionParser.Default.Parse(apiVersion);
			options.ReportApiVersions = true;
			options.ApiVersionReader = new UrlSegmentApiVersionReader();
		})
		.AddApiExplorer(options =>
		{
			options.GroupNameFormat = "'v'VVV";
			options.SubstituteApiVersionInUrl = true;
		});

		return services;
	}
}
