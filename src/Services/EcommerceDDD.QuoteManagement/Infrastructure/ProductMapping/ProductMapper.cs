namespace EcommerceDDD.QuoteManagement.Infrastructure.ProductMapping;

public class ProductMapper(IProductCatalogService productCatalogService) : IProductMapper
{
	public async Task<Result<IEnumerable<ProductViewModel>>> MapProductFromCatalogAsync(IEnumerable<ProductId> productIds,
		Currency currency, CancellationToken cancellationToken)
	{
		var productIdValues = productIds
			.Select(p => (Guid?)p.Value)
			.ToList();

		try
		{
			var response = await productCatalogService
				.GetProductsAsync(currency.Code, productIdValues, cancellationToken);

			if (response is null)
				return Result.Fail<IEnumerable<ProductViewModel>>("An error occurred while retrieving products from catalog.");

			return Result.Ok<IEnumerable<ProductViewModel>>(response);
		}
		catch (Exception)
		{
			return Result.Fail<IEnumerable<ProductViewModel>>("An error occurred requesting products from catalog.");
		}
	}
}
