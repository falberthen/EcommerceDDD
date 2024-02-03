namespace EcommerceDDD.QuoteManagement.Domain;

public record class QuoteItemData(
    QuoteId QuoteId,
    ProductId ProductId,
    string ProductName,
    Money ProductPrice,
    int Quantity);