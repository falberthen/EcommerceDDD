namespace EcommerceDDD.OrderProcessing.Domain.Commands;

public record class ProcessOrder : ICommand
{
    public CustomerId CustomerId { get; private set; }
    public OrderId OrderId { get; private set; }
    public QuoteId QuoteId { get; private set; }

    public static ProcessOrder Create(
        CustomerId customerId,
        OrderId orderId,
        QuoteId quoteId)
    {
        if (customerId is null)
            throw new ArgumentNullException(nameof(customerId));
        if (orderId is null)
            throw new ArgumentNullException(nameof(orderId));
        if (quoteId is null)
            throw new ArgumentNullException(nameof(quoteId));

        return new ProcessOrder(customerId, orderId, quoteId);
    }

    private ProcessOrder(
        CustomerId customerId,
        OrderId orderId,
        QuoteId quoteId)
    {
        CustomerId = customerId;
        OrderId = orderId;
        QuoteId = quoteId;        
    }
}