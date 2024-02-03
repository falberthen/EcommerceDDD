namespace EcommerceDDD.OrderProcessing.Domain;

public record class OrderData(
    CustomerId CustomerId,
    QuoteId QuoteId,    
    Currency? Currency = null,
    IReadOnlyList<ProductItemData>? Items = null);

public record class ProductItemData()
{
    public ProductId ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public Money UnitPrice { get; set; }
}
