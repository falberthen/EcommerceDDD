using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Quotes.Domain.Events;

public record class QuoteCreated(
    QuoteId QuoteId,
    CustomerId CustomerId, 
    DateTime CreatedAt) : IDomainEvent
{
    public static QuoteCreated Create(
        QuoteId quoteId,
        CustomerId customerId, 
        DateTime CreatedAt)
    {
        return new QuoteCreated(
            quoteId, 
            customerId, 
            CreatedAt);
    }
}
