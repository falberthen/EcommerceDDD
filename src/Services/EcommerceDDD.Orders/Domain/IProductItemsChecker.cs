namespace EcommerceDDD.Orders.Domain;

public interface IProductItemsChecker
{
    Task EnsureProductItemsExist(IReadOnlyList<ProductItemData> productItems, Currency currency);
}