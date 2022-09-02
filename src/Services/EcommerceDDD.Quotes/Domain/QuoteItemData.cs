namespace EcommerceDDD.Quotes.Domain;

public record class QuoteItemData(Guid Id, QuoteId QuoteId, ProductId ProductId, int Quantity);