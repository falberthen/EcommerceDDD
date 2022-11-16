using static EcommerceDDD.Orders.Application.Orders.PlacingOrder.PlaceOrderHandler;

namespace EcommerceDDD.Orders.Domain;

public interface IProductItemsMapper
{
    Task<List<ProductItemData>> MatchProductsFromCatalog(IReadOnlyList<QuoteItemViewModel> quoteItems, Currency currency);
}