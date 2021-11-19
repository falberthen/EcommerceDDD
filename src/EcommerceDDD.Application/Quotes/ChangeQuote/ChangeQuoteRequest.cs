namespace EcommerceDDD.Application.Quotes.ChangeQuote;

public record ChangeQuoteRequest
{
    public Guid QuoteId { get; init; }
    public ProductDto Product { get; init; }
    public string Currency { get; init; }
}