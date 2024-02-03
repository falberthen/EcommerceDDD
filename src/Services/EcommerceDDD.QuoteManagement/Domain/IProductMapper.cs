namespace EcommerceDDD.QuoteManagement.Domain;

public interface IProductMapper
{
    Task<IEnumerable<ProductViewModel>> MapProductFromCatalogAsync(IEnumerable<ProductId> productIds, Currency currency);
}