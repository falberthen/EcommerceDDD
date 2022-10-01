namespace EcommerceDDD.Quotes.Application.Quotes.PlacingOrderFromQuote;

public record class PlaceOrderRequest(
    Guid QuoteId,
    Guid CustomerId,
    List<QuoteItemRequest> Items,
    string CurrencyCode);