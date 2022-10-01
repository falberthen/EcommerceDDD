namespace EcommerceDDD.Orders.Application.Orders.GettingOrders;

public record OrderViewModel()
{
    public Guid OrderId { get; set; }
    public Guid QuoteId { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int StatusCode { get; set; }
    public string StatusText { get; set; }
    public List<OrderLineViewModel> OrderLines { get; set; } = default!;
    public string CurrencySymbol { get; set; }
    public decimal TotalPrice { get; set; }
}

public record class OrderLineViewModel
{
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}