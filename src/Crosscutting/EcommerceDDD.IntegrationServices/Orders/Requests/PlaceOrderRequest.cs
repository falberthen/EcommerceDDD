namespace EcommerceDDD.IntegrationServices.Orders.Requests;

public record class PlaceOrderRequest(
    Guid QuoteId,
    Guid CustomerId,
    List<QuoteItemRequest> Items,
    string CurrencyCode);

public record class QuoteItemRequest(
    Guid QuoteItemId,
    Guid ProductId,
    int Quantity);
