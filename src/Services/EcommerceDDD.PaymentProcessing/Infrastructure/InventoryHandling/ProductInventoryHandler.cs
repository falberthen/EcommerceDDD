using EcommerceDDD.ServiceClients.ApiGateway.Models;

namespace EcommerceDDD.PaymentProcessing.Infrastructure.InventoryHandling;

public class ProductInventoryHandler(ApiGatewayClient apiGatewayClient) : IProductInventoryHandler
{
	private readonly ApiGatewayClient _apiGatewayClient = apiGatewayClient;

	public async Task<bool> CheckProductsInStockAsync(IReadOnlyList<ProductItem> productItems, CancellationToken cancellationToken)
	{
		var productIdFilter = productItems
			.Select(p => new Guid?(p.ProductId.Value))
			.ToList();

		var request = new CheckProductsInStockRequest()
		{
			ProductIds = productIdFilter
		};
		bool hasOutOfStockItem = false;

		try
		{
			var inventoryRequestBuilder = _apiGatewayClient.Api.V2.Inventory;
			var response = await inventoryRequestBuilder
				.CheckStockQuantity
				.PostAsync(request, cancellationToken: cancellationToken);

			if (response?.Success != true)
				throw new ApplicationLogicException("An error occurred checking products stock availability.");

			var inventoryData = response.Data
				?? throw new RecordNotFoundException("No data was provided for the filtered products.");

			// Check if any product's requested quantity exceeds what's in stock
			hasOutOfStockItem = productItems.Any(item =>
			{
				InventoryStockUnitViewModel stock = inventoryData
					.SingleOrDefault(p => p.ProductId == item.ProductId.Value)
					?? throw new RecordNotFoundException($"Product {item.ProductId.Value} not found in stock data.");

				return item.Quantity > stock.QuantityInStock;
			});
		}
		catch (Microsoft.Kiota.Abstractions.ApiException ex)
		{
			throw new ApplicationLogicException(
				$"An error occurred checking stock quantity for products.", ex);
		}

		return !hasOutOfStockItem;
	}

	public async Task DecreaseQuantityInStockAsync(IReadOnlyList<ProductItem> productItems, CancellationToken cancellationToken)
	{
		var tasks = productItems.Select(async productItem =>
		{
			var request = new DecreaseQuantityInStockRequest
			{
				DecreasedQuantity = productItem.Quantity
			};

			try
			{
				var inventoryRequestBuilder = _apiGatewayClient.Api.V2.Inventory[productItem.ProductId.Value];
				await inventoryRequestBuilder
					.DecreaseStockQuantity
					.PutAsync(request, cancellationToken: cancellationToken);
			}
			catch (Microsoft.Kiota.Abstractions.ApiException ex)
			{
				throw new ApplicationLogicException(
					$"An error occurred decreasing stock quantity for product {productItem.ProductId.Value}.", ex);
			}
		});

		await Task.WhenAll(tasks);
	}
}