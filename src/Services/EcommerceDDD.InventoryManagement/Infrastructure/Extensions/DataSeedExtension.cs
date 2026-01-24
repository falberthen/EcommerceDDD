namespace EcommerceDDD.InventoryManagement.Infrastructure.Extensions;

public static class DataSeedExtension
{
	/// <summary>
	/// Seeds inventory with deterministic product IDs matching ProductCatalog.
	/// No longer depends on ProductCatalog service at startup.
	/// </summary>
	/// <param name="app"></param>
	/// <returns></returns>
	public static async Task<IApplicationBuilder> SeedInventoryCatalogAsync(this IApplicationBuilder app)
	{
		using var serviceScope = app.ApplicationServices
			.GetService<IServiceScopeFactory>()!
			.CreateScope() ?? throw new NullReferenceException("Can't create scope factory.");

		var commandBus = serviceScope.ServiceProvider
			.GetRequiredService<ICommandBus>();

		try
		{
			// Get all products with their initial quantities
			var productsWithQuantities = InventorySeedIds.GetAllProductsWithQuantities();

			List<Tuple<ProductId, int>> productQuantities = productsWithQuantities
				.Select(p => new Tuple<ProductId, int>(ProductId.Of(p.ProductId), p.InitialQuantity))
				.ToList();

			var command = EnterProductInStock.Create(productQuantities);
			await commandBus.SendAsync(command, CancellationToken.None);

			Console.WriteLine($"Inventory seeded successfully with {productQuantities.Count} products.");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"An error occurred during inventory seeding: {ex.Message}");
		}

		return app;
	}
}
