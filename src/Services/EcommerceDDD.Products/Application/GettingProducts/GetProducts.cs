using EcommerceDDD.Products.Domain;
using EcommerceDDD.Core.CQRS.QueryHandling;

namespace EcommerceDDD.Products.Application.Products.GettingProducts;

public record class GetProducts : IQuery<IList<ProductViewModel>>
{
    public string CurrencyCode { get; private set; }
    public IList<ProductId> ProductIds { get; private set; }

    public static GetProducts Create(
        string currencyCode,
        IList<ProductId> productIds)
    {
        if (string.IsNullOrEmpty(currencyCode))
            throw new ArgumentNullException(nameof(currencyCode));

        return new GetProducts(currencyCode, productIds);
    }

    private GetProducts(
        string currencyCode,
        IList<ProductId> productIds)
    {
        CurrencyCode = currencyCode;
        ProductIds = productIds;
    }
}