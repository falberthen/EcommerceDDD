using EcommerceDDD.Core.Domain;
using EcommerceDDD.Core.Exceptions;
using EcommerceDDD.Orders.Application.Quotes;
using EcommerceDDD.Orders.Domain.Events;

namespace EcommerceDDD.Orders.Domain;

public class Order : AggregateRoot<OrderId>
{
    public CustomerId CustomerId { get; private set; }
    public QuoteId QuoteId { get; private set; }
    public OrderStatus Status { get; private set; }
    public IReadOnlyList<OrderLine> OrderLines => _orderLines;
    public Money TotalPrice { get; private set; }
    public PaymentId PaymentId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CanceledAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private List<OrderLine> _orderLines { get; set; } = default!;

    public async static Task<Order> CreateNew(OrderId orderId, ConfirmedQuote confirmedQuote,
        IOrderProductsChecker _orderProductsChecker)
    {
        if (orderId == null)
            throw new DomainException("The order Id is required.");

        if (confirmedQuote == null)
            throw new DomainException("A quote is required.");

        if (confirmedQuote.CustomerId == null)
            throw new DomainException("The customer Id is required.");

        if (!confirmedQuote.Items.Any())
            throw new DomainException("An order should have at least one product.");

        await _orderProductsChecker.CheckFromQuote(confirmedQuote);

        return new Order(orderId, confirmedQuote);
    }

    public void RecordPayment(PaymentId paymentId, Money totalPaid)
    {
        var products = OrderLines
            .Select(ol => ol.ProductItem.ProductId).ToList();

        var @event = OrderPaid.Create(
            Id,
            paymentId, 
            products, 
            totalPaid);

        AppendEvent(@event);
        Apply(@event);
    }

    public void Complete()
    {
        if (Status != OrderStatus.Paid)
            throw new DomainException("The order must be paid before completed.");

        var @event = OrderCompleted.Create(Id, DateTime.UtcNow);

        AppendEvent(@event);
        Apply(@event);
    }

    public void Cancel(OrderCancellationReason cancellationReason)
    {
        if (Status == OrderStatus.Completed || Status == OrderStatus.Canceled)
            throw new DomainException("The order cannot be cancelled at this point.");
        
        var @event = OrderCanceled.Create(Id, PaymentId, cancellationReason, DateTime.UtcNow);

        AppendEvent(@event);
        Apply(@event);
    }

    private List<OrderLine> BuildOrderLines(IReadOnlyList<ConfirmedQuoteItem> items)
    {
        var orderLines = items.Select(c =>
            new OrderLine(
                new ProductItem(
                    c.ProductId, 
                    c.ProductName, 
                    c.UnitPrice, 
                    c.Quantity)
            ))
            .ToList();

        return orderLines;
    }

    private void Apply(OrderPlaced placed)
    {
        Id = placed.OrderId;
        QuoteId = placed.QuoteId;
        CustomerId = placed.CustomerId;
        CreatedAt = placed.CreatedAt;
        Status = OrderStatus.Placed;
        _orderLines = placed.OrderLines.ToList();
        TotalPrice = placed.TotalPrice;
    }

    private void Apply(OrderPaid paid)
    {
        PaymentId = paid.PaymentId;
        Status = OrderStatus.Paid;
    }

    private void Apply(OrderCompleted completed)
    {
        Status = OrderStatus.Completed;
        CompletedAt = completed.CompletedAt;
    }

    private void Apply(OrderCanceled canceled)
    {
        Status = OrderStatus.Canceled;
        CanceledAt = canceled.CanceledAt;
    }

    private Money CalculateTotalPrice(List<OrderLine> orderLines, string currencyCode)
    {
        var amount = orderLines.Sum(qi =>
        {
            return Money.Of(qi.ProductItem.UnitPrice.Value * qi.ProductItem.Quantity, currencyCode).Value;
        });

        return Money.Of(amount, currencyCode);
    }

    private Order(OrderId orderId, ConfirmedQuote confirmedQuote)
    {
        var orderLines = BuildOrderLines(confirmedQuote.Items);
        var totalPrice = CalculateTotalPrice(orderLines, 
            confirmedQuote.Currency.Code);

        var @event = OrderPlaced.Create(
            orderId,
            confirmedQuote.Id,
            confirmedQuote.CustomerId,
            DateTime.UtcNow,
            orderLines,            
            confirmedQuote.Currency,
            totalPrice);

        AppendEvent(@event);
        Apply(@event);
    }

    private Order() { }
}
