namespace EcommerceDDD.QuoteManagement.Infrastructure.ProductMapping;

public class ProductMapper(ProductCatalogClient productCatalogClient) : IProductMapper
{
	/// <summary>
	/// Maps product from catalog
	/// </summary>
	/// <param name="productIds"></param>
	/// <param name="currency"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	public async Task<Result<IEnumerable<ProductViewModel>>> MapProductFromCatalogAsync(IEnumerable<ProductId> productIds,
		Currency currency, CancellationToken cancellationToken)
	{
		var productIdValues = productIds
			.Select(p => (Guid?)p.Value)
			.ToList();

		var request = new GetProductsRequest()
		{
			CurrencyCode = currency.Code,
			ProductIds = productIdValues
		};

		try
		{
			var response = await productCatalogClient.Api.V2.Products
				.PostAsync(request, cancellationToken: cancellationToken);

			if (response is null)
				return Result.Fail<IEnumerable<ProductViewModel>>("An error occurred while retrieving products from catalog.");

			return Result.Ok<IEnumerable<ProductViewModel>>(response);
		}
		catch (Microsoft.Kiota.Abstractions.ApiException)
		{
			return Result.Fail<IEnumerable<ProductViewModel>>("An error occurred requesting products from catalog.");
		}
	}
}
