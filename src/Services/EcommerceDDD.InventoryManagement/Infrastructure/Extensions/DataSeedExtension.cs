using EcommerceDDD.ServiceClients.ApiGateway.Models;

namespace EcommerceDDD.ProductCatalog.Infrastructure.Extensions;

public static class DataSeedExtension
{
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
			.CreateScope() ?? throw new NullReferenceException("Can't create scope factory.");

		var apiGatewayClient = serviceScope.ServiceProvider
			.GetRequiredService<ApiGatewayClient>();
		var commandBus = serviceScope.ServiceProvider
			.GetRequiredService<ICommandBus>();

		var retryPolicy = Policy
			.Handle<HttpRequestException>()
			.WaitAndRetryAsync(
				retryCount: 10, // Maximum 10 retries
				sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // Exponential backoff
				onRetry: (exception, timeSpan, retryCount, context) =>
				{
					Console.WriteLine($"Retry {retryCount} after {timeSpan.TotalSeconds} seconds due to: {exception.Message}");
				});

		var timeoutPolicy = Policy.TimeoutAsync(TimeSpan.FromHours(1));
		var policyWrap = Policy.WrapAsync(timeoutPolicy, retryPolicy);

		await policyWrap.ExecuteAsync(async () =>
		{
			// Bringing all products from the catalog
			var request = new GetProductsRequest()
			{
				CurrencyCode = "USD",
				ProductIds = new List<Guid?>()
			};

			try
			{
				var response = await apiGatewayClient.Api.V2.Products
					.PostAsync(request);

				if (response?.Success == false || response?.Data is null)
					throw new HttpRequestException("An error occurred while retrieving products.");

				List<Tuple<ProductId, int>> productQuantities = new();
				foreach (var product in response.Data)
				{
					if (product.ProductId.HasValue)
					{
						productQuantities.Add(new Tuple<ProductId, int>(
							ProductId.Of(product.ProductId.Value),
							new Random().Next(0, 50))
						);
					}
				}

				var command = EnterProductInStock.Create(productQuantities);
				await commandBus.SendAsync(command, CancellationToken.None);
			}
			catch (Microsoft.Kiota.Abstractions.ApiException ex)
			{
				throw new HttpRequestException("Products API not available.", ex);
			}
		});

		return app;
	}
}