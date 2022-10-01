namespace EcommerceDDD.Orders.Domain;

public record class OrderData(
    OrderId OrderId,
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

    public void SetName(string name) => ProductName = name;
    public void SetPrice(Money unitPrice) => UnitPrice = unitPrice;
}
