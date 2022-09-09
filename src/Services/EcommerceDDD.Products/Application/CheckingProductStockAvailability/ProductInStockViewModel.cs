namespace EcommerceDDD.Products.Application.Products.CheckingProductStockAvailability;

public record class ProductInStockViewModel(Guid ProductId, int AmountInStock);