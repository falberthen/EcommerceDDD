namespace EcommerceDDD.Application.Products;

public record ProductViewModel
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Price { get; init; }
    public string CurrencySymbol { get; init; }
}