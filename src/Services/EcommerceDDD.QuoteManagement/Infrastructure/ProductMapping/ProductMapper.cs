using EcommerceDDD.ServiceClients.ApiGateway.Models;

namespace EcommerceDDD.QuoteManagement.Infrastructure.ProductMapping;

public class ProductMapper(ApiGatewayClient apiGatewayClient) : IProductMapper
{
	/// <summary>
	/// Maps product from catalog
	/// </summary>
	/// <param name="productIds"></param>
	/// <param name="currency"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	/// <exception cref="RecordNotFoundException"></exception>
	public async Task<IEnumerable<ProductViewModel>> MapProductFromCatalogAsync(IEnumerable<ProductId> productIds,
		Currency currency, CancellationToken cancellationToken)
	{
		var productIdValues = productIds
			.Select(p => (Guid?)p.Value)
			.ToList();

		// Bringing all products from the catalog
		var request = new GetProductsRequest()
		{
			CurrencyCode = currency.Code,
			ProductIds = productIdValues
		};

		try
		{
			var response = await apiGatewayClient.Api.V2.Products
				.PostAsync(request, cancellationToken: cancellationToken);

			if (response?.Success == false || response?.Data is null)
				throw new HttpRequestException("An error occurred while retrieving products.");

			var productViewModel = response?.Data!;
			return productViewModel;
		}
		catch (Microsoft.Kiota.Abstractions.ApiException ex)
		{
			throw new ApplicationLogicException(
				$"An error occurred requesting products from catalog.", ex);
		}
	}
}