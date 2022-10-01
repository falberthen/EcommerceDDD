namespace EcommerceDDD.Quotes.Application.Products;

public record class ProductViewModel(Guid ProductId, string Name, decimal Price, string CurrencySymbol);