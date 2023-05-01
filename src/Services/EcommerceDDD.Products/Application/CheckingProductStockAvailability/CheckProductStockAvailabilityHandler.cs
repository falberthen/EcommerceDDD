namespace EcommerceDDD.Products.Application.Products.CheckingProductStockAvailability;

public class CheckProductStockAvailabilityHandler : IQueryHandler<CheckProductStockAvailability, IList<ProductInStockViewModel>> 
{
    private readonly IProducts _products;
    private const int _maximumAmountInStock = 25;

    public CheckProductStockAvailabilityHandler(IProducts products)
    {
        _products = products;
    }

    public async Task<IList<ProductInStockViewModel>> Handle(CheckProductStockAvailability query, CancellationToken cancellationToken)
    {
        var productsInStockViewModel = new List<ProductInStockViewModel>();
        var products = await _products.GetByIds(query.ProductIds);

        foreach (var product in products)
        {
            var amountInStock = new Random().Next(0, _maximumAmountInStock);
            productsInStockViewModel.Add(new ProductInStockViewModel(product.Id.Value, amountInStock));
        }

        return productsInStockViewModel;
    }
}