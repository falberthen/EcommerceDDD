namespace EcommerceDDD.Quotes.Application.GettingOpenQuote;

public record QuoteViewModel()
{
    public Guid QuoteId { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string QuoteStatus { get; set; }
    public List<QuoteItemViewModel> Items { get; set; }
    public string CurrencySymbol { get; set; }
    public decimal TotalPrice => Items.Sum(i => i.UnitPrice);
}

public record class QuoteItemViewModel
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string CurrencySymbol { get; set; }
    public decimal TotalPrice { get; set; }
}