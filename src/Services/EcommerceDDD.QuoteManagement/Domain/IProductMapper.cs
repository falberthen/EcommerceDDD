namespace EcommerceDDD.QuoteManagement.Domain;

public interface IProductMapper
{
	Task<Result<IEnumerable<ProductViewModel>>> MapProductFromCatalogAsync(IEnumerable<ProductId> productIds, Currency currency, CancellationToken cancellationToken);
}
