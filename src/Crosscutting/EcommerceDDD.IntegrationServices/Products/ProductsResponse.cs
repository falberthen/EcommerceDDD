namespace EcommerceDDD.IntegrationServices.Products;

public record class ProductsResponse
{
    public bool Success { get; set; }
    public List<ProductViewModel> Data { get; set; }
    public record class ProductViewModel(Guid Id, string Name, decimal Price, string CurrencySymbol);
}