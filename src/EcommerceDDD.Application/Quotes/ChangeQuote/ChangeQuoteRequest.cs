using System;

namespace EcommerceDDD.Application.Quotes.ChangeQuote;

public record ChangeQuoteRequest
{
    public Guid QuoteId { get; set; }
    public ProductDto Product { get; set; }
    public string Currency { get; set; }
}