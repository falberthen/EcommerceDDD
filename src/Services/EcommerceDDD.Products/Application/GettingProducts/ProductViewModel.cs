namespace EcommerceDDD.Products.Application.Products.GettingProducts;

public record class ProductViewModel(Guid Id, string Name, string Price, string CurrencySymbol);