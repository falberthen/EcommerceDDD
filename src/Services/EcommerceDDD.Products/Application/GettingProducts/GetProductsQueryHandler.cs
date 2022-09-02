using EcommerceDDD.Core.Exceptions;
using EcommerceDDD.Products.Domain;
using EcommerceDDD.Core.CQRS.QueryHandling;

namespace EcommerceDDD.Products.Application.Products.GettingProducts;

public class GetProductsQueryHandler : QueryHandler<GetProducts, IList<ProductViewModel>> 
{
    private readonly IProducts _products;
    private readonly ICurrencyConverter _currencyConverter;

    public GetProductsQueryHandler(
        IProducts products,
        ICurrencyConverter currencyConverter)
    {
        _products = products;
        _currencyConverter = currencyConverter;
    }

    public override async Task<IList<ProductViewModel>> Handle(GetProducts query, CancellationToken cancellationToken)
    {
        var productsViewModel = new List<ProductViewModel>();

        var products = query.productIds.Count == 0
            ? await _products.ListAll(cancellationToken)
            : await _products.GetByIds(query.productIds);

        if (string.IsNullOrEmpty(query.CurrencyCode))
            throw new ApplicationDataException("Currency code cannot be empty.");

        var currency = Currency.OfCode(query.CurrencyCode);
        foreach (var product in products)
        {
            var convertedPrice = _currencyConverter
                .Convert(product.UnitPrice.Value, query.CurrencyCode);

            productsViewModel.Add(
                new ProductViewModel(
                    product.Id.Value,
                    product.Name,
                    Math.Round(convertedPrice, 2).ToString(),
                    currency.Symbol));
        }

        return productsViewModel;
    }
}
