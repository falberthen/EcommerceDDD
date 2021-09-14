using System;

namespace EcommerceDDD.Application.Quotes.SaveQuote
{
    public record CreateQuoteRequest
    {
        public Guid CustomerId { get; set; }
        public ProductDto Product { get; set; }
        public string Currency { get; set; }
    }
}
