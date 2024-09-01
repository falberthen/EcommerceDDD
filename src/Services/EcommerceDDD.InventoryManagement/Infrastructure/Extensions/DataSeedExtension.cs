namespace EcommerceDDD.ProductCatalog.Infrastructure.Extensions;

public static class DataSeedExtension
{
	private static IIntegrationHttpService _integrationHttpService;
	private static ICommandBus _commandBus;

	/// <summary>
	/// Adds products to inventory
	/// </summary>
	/// <param name="app"></param>
	/// <returns></returns>
	/// <exception cref="NullReferenceException"></exception>
	/// <exception cref="RecordNotFoundException"></exception>
	public static async Task<IApplicationBuilder> SeedInventoryCatalogAsync(
		this IApplicationBuilder app, IConfiguration configuration)
	{
		using var serviceScope = app.ApplicationServices
			.GetService<IServiceScopeFactory>()!
			.CreateScope() ??
			throw new NullReferenceException("Can't create scope factory.");

		_integrationHttpService = serviceScope.ServiceProvider
			.GetRequiredService<IIntegrationHttpService>();
		_commandBus = serviceScope.ServiceProvider
			.GetRequiredService<ICommandBus>();

		var retryPolicy = Policy
			.Handle<HttpRequestException>()
			.WaitAndRetryForeverAsync(attempt => TimeSpan.FromSeconds(5),
				(exception, calculatedWaitDuration) =>
				{
					Console.WriteLine($"Retrying seeding inventory after {calculatedWaitDuration.TotalSeconds} " +
						$"seconds due to exception: {exception.Message}");
				});

		var timeoutPolicy = Policy.TimeoutAsync(TimeSpan.FromHours(1));
		var policyWrap = Policy.WrapAsync(timeoutPolicy, retryPolicy);

		await policyWrap.ExecuteAsync(async () =>
		{
			// Bringing all products from the catalog
			var apiRoute = configuration["ApiRoutes:ProductCatalog"];
			var response = await _integrationHttpService.FilterAsync<List<ProductViewModel>>(
				apiRoute!, new GetProductsRequest("USD", Array.Empty<Guid>())
			);

			if (response?.Success == false || response?.Data is null)
				throw new HttpRequestException("An error occurred while retrieving products.");

			List<Tuple<ProductId, int>> productQuantities = new();
			foreach (var product in response?.Data!)
			{
				productQuantities.Add(new Tuple<ProductId, int>(
					ProductId.Of(product.ProductId),
					new Random().Next(0, 50)));
			}

			var command = EnterProductInStock.Create(productQuantities);
			await _commandBus.SendAsync(command, CancellationToken.None);
		});

		return app;
	}
}

public record class GetProductsRequest(
	string CurrencyCode, 
	Guid[] ProductIds
);

public record class ProductViewModel(
	Guid ProductId, 
	string Name, 
	decimal Price, 
	string CurrencySymbol
);