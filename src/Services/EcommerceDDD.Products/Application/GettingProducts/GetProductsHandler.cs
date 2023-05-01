namespace EcommerceDDD.Products.Application.Products.GettingProducts;

public class GetProductsHandler : IQueryHandler<GetProducts, IList<ProductViewModel>> 
{
    private readonly IProducts _products;
    private readonly ICurrencyConverter _currencyConverter;

    public GetProductsHandler(
        IProducts products,
        ICurrencyConverter currencyConverter)
    {
        _products = products;
        _currencyConverter = currencyConverter;
    }

    public async Task<IList<ProductViewModel>> Handle(GetProducts query, CancellationToken cancellationToken)
    {
        var productsViewModel = new List<ProductViewModel>();

        var products = query.ProductIds.Count == 0
            ? await _products.ListAll(cancellationToken)
            : await _products.GetByIds(query.ProductIds);

        if (string.IsNullOrEmpty(query.CurrencyCode))
            throw new RecordNotFoundException("Currency code cannot be empty.");

        var currency = Currency.OfCode(query.CurrencyCode);
        foreach (var product in products)
        {
            var convertedPrice = _currencyConverter
                .Convert(product.UnitPrice.Amount, query.CurrencyCode);

            productsViewModel.Add(
                new ProductViewModel(
                    product.Id.Value,
                    product.Name,
                    product.Category,
                    product.Description,
                    product.ImageUrl,
                    Math.Round(convertedPrice, 2).ToString(),
                    currency.Symbol));
        }

        return productsViewModel;
    }
}
