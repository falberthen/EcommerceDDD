namespace EcommerceDDD.Application.Quotes.SaveQuote
{
    public record CreateQuoteRequest
    {
        public Guid CustomerId { get; init; }
        public ProductDto Product { get; init; }
        public string Currency { get; init; }
    }
}