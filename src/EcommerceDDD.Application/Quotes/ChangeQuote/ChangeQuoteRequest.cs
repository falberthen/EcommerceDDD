using System;

namespace EcommerceDDD.Application.Quotes.SaveQuote
{
    public record ChangeQuoteRequest
    {
        public Guid QuoteId { get; set; }
        public ProductDto Product { get; set; }
        public string Currency { get; set; }
    }
}
