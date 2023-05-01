namespace EcommerceDDD.Orders.Domain.Commands;

public record class PlaceOrder : ICommand 
{
    public QuoteId QuoteId { get; private set; }

    public static PlaceOrder Create(QuoteId quoteId)
    {
        if (quoteId is null)
            throw new ArgumentNullException(nameof(quoteId));
        
        return new PlaceOrder(quoteId);
    }

    private PlaceOrder(QuoteId quoteId)
    {
        QuoteId = quoteId;
    }
}