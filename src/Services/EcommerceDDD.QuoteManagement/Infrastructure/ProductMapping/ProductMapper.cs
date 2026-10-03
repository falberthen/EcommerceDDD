namespace EcommerceDDD.QuoteManagement.Infrastructure.ProductMapping;

public class ProductMapper(IProductCatalogService productCatalogService) : IProductMapper
{
	public async Task<Result<IEnumerable<ProductViewModel>>> MapProductFromCatalogAsync(IEnumerable<ProductId> productIds,
		Currency currency, CancellationToken cancellationToken)
	{
		var productIdValues = productIds
			.Select(p => (Guid?)p.Value)
			.ToList();

		var response = await productCatalogService
			.GetProductsAsync(currency.Code, productIdValues, cancellationToken);

		if (response is null)
			throw new InvalidOperationException("The product catalog returned no products.");

		return Result.Ok<IEnumerable<ProductViewModel>>(response);
	}
}
