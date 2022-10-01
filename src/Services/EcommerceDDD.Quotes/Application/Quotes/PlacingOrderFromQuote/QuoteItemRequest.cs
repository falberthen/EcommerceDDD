namespace EcommerceDDD.Quotes.Application.Quotes.PlacingOrderFromQuote;

public record class QuoteItemRequest(Guid ProductId, int Quantity);