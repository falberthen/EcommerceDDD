namespace EcommerceDDD.InventoryManagement.Domain.Commands;

public record class EnterProductInStock : ICommand
{
    public List<Tuple<ProductId, int>> ProductIdsQuantities { get; private set; }

    public static EnterProductInStock Create(
        List<Tuple<ProductId, int>> productIdsQuantities)
    {
        if (productIdsQuantities is null || !productIdsQuantities.Any())
            throw new ArgumentNullException(nameof(productIdsQuantities));
        
        return new EnterProductInStock(productIdsQuantities);
    }

    private EnterProductInStock(List<Tuple<ProductId, int>> productIdsQuantities)
    {
        ProductIdsQuantities = productIdsQuantities;
    }
}