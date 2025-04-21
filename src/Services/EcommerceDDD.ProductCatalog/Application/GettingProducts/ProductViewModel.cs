namespace EcommerceDDD.ProductCatalog.Application.Products.GettingProducts;

public record class ProductViewModel(
    Guid ProductId, 
    string Name,
    string Category,
    string Description,
    string ImageUrl,
    decimal Price, 
    string CurrencySymbol,
    int QuantityInStock,
	int QuantityAddedToCart = 0);