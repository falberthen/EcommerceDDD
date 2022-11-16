namespace EcommerceDDD.Orders.Domain;

public record class OrderData(
    QuoteId QuoteId,
    CustomerId CustomerId,
    IReadOnlyList<ProductItemData> Items,
    Currency Currency);

public record class ProductItemData()
{
    public ProductId ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public Money UnitPrice { get; set; }
}
