namespace EcommerceDDD.Products.Application.Products.GettingProducts;

public record class ProductViewModel(
    Guid ProductId, 
    string Name,
    string Category,
    string Description,
    string ImageUrl,
    string Price, 
    string CurrencySymbol);