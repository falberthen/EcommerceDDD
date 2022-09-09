namespace EcommerceDDD.IntegrationServices.Products.Responses;

public record class ProductStockAvailabilityResponse
{
    public bool Success { get; set; }
    public List<ProductInStockViewModel> Data { get; set; }
    public record class ProductInStockViewModel(Guid ProductId, int AmountInStock);
}