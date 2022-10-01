namespace EcommerceDDD.Products.Application.Products.GettingProducts;

public record class ProductViewModel(Guid ProductId, string Name, string Price, string CurrencySymbol);