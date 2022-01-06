using System.Linq;
using System.Collections.Generic;
using EcommerceDDD.Domain.Quotes;
using EcommerceDDD.Domain.SeedWork;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Orders.Events;
using EcommerceDDD.Domain.SharedKernel;

namespace EcommerceDDD.Domain.Orders;

public class Order : AggregateRoot<OrderId>
{
    public CustomerId CustomerId { get; private set; }
    public QuoteId QuoteId { get; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Money TotalPrice { get; private set; }
    public IReadOnlyList<OrderLine> OrderLines => _orderLines;
    private readonly List<OrderLine> _orderLines = new List<OrderLine>();

    internal static Order CreateNew(CustomerId customerId, QuoteId quoteId, List<QuoteItemProductData> products, 
        Currency currency, ICurrencyConverter converter)
    {
        var orderId = new OrderId(Guid.NewGuid());
        return new Order(orderId, customerId, quoteId, products, currency, converter);
    }

    public static Order PlaceOrder(CustomerId customerId, QuoteId quoteId, List<QuoteItemProductData> products,
        Currency currency, ICurrencyConverter currencyConverter)
    {
        if (customerId == null)
            throw new BusinessRuleException("The customer Id is required.");

        if (!products.Any())
            throw new BusinessRuleException("An order should have at least one product.");

        if (currency == null)
            throw new BusinessRuleException("The currency is required.");

        var order = Order.CreateNew(customerId, quoteId, products, currency, currencyConverter);            
        return order;
    }

    public void ChangeStatus(OrderStatus status)
    {
        Status = status;
    }

    private void CalculateTotalPrice(Currency currency)
    {
        var total = _orderLines.Sum(x => x.ProductExchangePrice.Value);
        TotalPrice = Money.Of(total, currency.Code);
    }

    private void BuildOrderLines(List<QuoteItemProductData> products,
        Currency currency, ICurrencyConverter converter)
    {
        var orderLines = products.Select(c =>
            OrderLine.CreateNew(
                Id,
                c.ProductId,
                c.ProductPrice,
                c.Quantity,
                currency,
                converter)
            ).ToArray();

        _orderLines.AddRange(orderLines);
        CalculateTotalPrice(currency);
    }

    private Order(OrderId id, CustomerId customerId, QuoteId quoteId, List<QuoteItemProductData> products,
        Currency currency, ICurrencyConverter converter)
    {
        Id = id;
        QuoteId = quoteId;
        CustomerId = customerId;
        CreatedAt = DateTime.Now;
        Status = OrderStatus.Placed;
        BuildOrderLines(products, currency, converter);
        AddDomainEvent(new OrderPlacedEvent(customerId, Id));
    }

    // Empty constructor for EF
    private Order() { }
}
