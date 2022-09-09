namespace EcommerceDDD.IntegrationServices.Products.Responses;

public record class ProductsResponse
{
    public bool Success { get; set; }
    public List<ProductViewModel> Data { get; set; }
    public record class ProductViewModel(Guid ProductId, string Name, decimal Price, string CurrencySymbol);
}