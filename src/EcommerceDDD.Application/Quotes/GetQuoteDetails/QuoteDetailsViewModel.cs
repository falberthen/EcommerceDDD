using System.Linq;
using System.Collections.Generic;

namespace EcommerceDDD.Application.Quotes.GetQuoteDetails;

public record class QuoteDetailsViewModel
{
    public Guid QuoteId { get; set; }
    public List<QuoteItemDetailsViewModel> QuoteItems { get; set; } = new List<QuoteItemDetailsViewModel>();
    public double TotalPrice { get; private set; }
    public string CreatedAt { get; set; }

    public void CalculateTotalOrderPrice()
    {
        var sum = QuoteItems.Sum(s => s.ProductPrice * s.ProductQuantity);
        TotalPrice = Convert.ToDouble(sum);
    }
}

public record class QuoteItemDetailsViewModel
{
    public Guid ProductId { get; init; }
    public String ProductName { get; init; }
    public decimal ProductPrice { get; init; }
    public int ProductQuantity { get; init; }
    public String CurrencySymbol { get; init; }
}