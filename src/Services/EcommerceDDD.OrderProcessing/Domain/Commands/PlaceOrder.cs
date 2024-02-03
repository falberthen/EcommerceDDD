namespace EcommerceDDD.OrderProcessing.Domain.Commands;

public record class PlaceOrder : ICommand 
{
    public CustomerId CustomerId { get; private set; }
    public QuoteId QuoteId { get; private set; }

    public static PlaceOrder Create(
        CustomerId customerId,
        QuoteId quoteId)
    {
        if (customerId is null)
            throw new ArgumentNullException(nameof(customerId));
        if (quoteId is null)
            throw new ArgumentNullException(nameof(quoteId));

        return new PlaceOrder(customerId, quoteId);
    }

    private PlaceOrder(
        CustomerId customerId,
        QuoteId quoteId)
    {
        CustomerId = customerId;
        QuoteId = quoteId;
    }
}