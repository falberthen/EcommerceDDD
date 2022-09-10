using EcommerceDDD.Orders.Domain;
using EcommerceDDD.IntegrationServices.Orders.Requests;

namespace EcommerceDDD.Orders.Application.Quotes;

// Dto for placing orders
public record class ConfirmedQuote(
    QuoteId Id,
    CustomerId CustomerId,
    IReadOnlyList<ConfirmedQuoteItem> Items,
    Currency Currency)
{
    public static ConfirmedQuote FromRequest(PlaceOrderRequest request)
    {
        var customerId = CustomerId.Of(request.CustomerId);
        var quoteId = QuoteId.Of(request.QuoteId);
        var currency = Currency.OfCode(request.CurrencyCode);

        var items = request.Items.Select(qi =>
            new ConfirmedQuoteItem()
            {
                ProductId = ProductId.Of(qi.ProductId),
                ProductName = string.Empty,
                Quantity = qi.Quantity
            }).ToList();
        return new ConfirmedQuote(
            quoteId,
            customerId,
            items,
            currency);
    }
}

public record class ConfirmedQuoteItem()
{
    public ProductId ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public Money UnitPrice { get; set; }

    public void SetName(string name) => ProductName = name;
    public void SetPrice(Money unitPrice) => UnitPrice = unitPrice;
}
