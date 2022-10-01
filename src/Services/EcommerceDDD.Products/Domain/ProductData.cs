namespace EcommerceDDD.Products.Domain;

public record class ProductData(
    string Name, 
    Money UnitPrice);