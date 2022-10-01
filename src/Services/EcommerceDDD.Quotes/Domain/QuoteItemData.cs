namespace EcommerceDDD.Quotes.Domain;

public record class QuoteItemData(
    QuoteId QuoteId, 
    ProductId ProductId, 
    int Quantity);