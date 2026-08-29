namespace EcommerceDDD.ProductCatalog.Application.Products.GettingProducts;

public record class GetProducts : IQuery<IReadOnlyList<ProductViewModel>>
{
    public string CurrencyCode { get; private set; }
    public IReadOnlyList<ProductId> ProductIds { get; private set; }

    public static GetProducts Create(
        string currencyCode,
        IReadOnlyList<ProductId> productIds)
    {
        if (string.IsNullOrEmpty(currencyCode))
            throw new ArgumentNullException(nameof(currencyCode));

        return new GetProducts(currencyCode, productIds);
    }

    private GetProducts(
        string currencyCode,
        IReadOnlyList<ProductId> productIds)
    {
        CurrencyCode = currencyCode;
        ProductIds = productIds;
    }
}