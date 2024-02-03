namespace EcommerceDDD.ProductCatalog.Domain;

public record class ProductData(
    string Name, 
    string Category,
    string Description,
    string ImageUrl,
    Money UnitPrice);