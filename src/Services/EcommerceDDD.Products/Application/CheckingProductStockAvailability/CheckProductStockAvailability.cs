namespace EcommerceDDD.Products.Application.Products.CheckingProductStockAvailability;

public record class CheckProductStockAvailability : IQuery<IList<ProductInStockViewModel>>
{
    public IList<ProductId> ProductIds { get; private set; }

    public static CheckProductStockAvailability Create(IList<ProductId> productIds)
    {
        if (productIds.Count == 0)
            throw new ArgumentOutOfRangeException(nameof(productIds));

        return new CheckProductStockAvailability(productIds);
    }

    private CheckProductStockAvailability(
        IList<ProductId> productIds)
    {
        ProductIds = productIds;
    }
}