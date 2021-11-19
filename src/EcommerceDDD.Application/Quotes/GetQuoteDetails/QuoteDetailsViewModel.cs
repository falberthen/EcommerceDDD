using System;
using System.Linq;
using System.Collections.Generic;

namespace EcommerceDDD.Application.Quotes.GetQuoteDetails;

public record QuoteDetailsViewModel
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

public record QuoteItemDetailsViewModel
{
    public Guid ProductId { get; set; }
    public String ProductName { get; set; }
    public decimal ProductPrice { get; set; }
    public int ProductQuantity { get; set; }
    public String CurrencySymbol { get; set; }
}