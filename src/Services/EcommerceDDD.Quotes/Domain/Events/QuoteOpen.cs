using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Quotes.Domain.Events;

public record class QuoteOpen : IDomainEvent
{
    public Guid QuoteId { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }

    public static QuoteOpen Create(
        Guid quoteId,
        Guid customerId, 
        DateTime createdAt)
    {
        if (quoteId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(quoteId));
        if (customerId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(customerId));
        if (createdAt == DateTime.MinValue)
            throw new ArgumentOutOfRangeException(nameof(createdAt));

        return new QuoteOpen(
            quoteId, 
            customerId,
            createdAt);
    }

    private QuoteOpen(
        Guid quoteId,
        Guid customerId,
        DateTime createdAt)
    {
        QuoteId = quoteId;
        CustomerId = customerId;
        CreatedAt = createdAt;
    }
}
