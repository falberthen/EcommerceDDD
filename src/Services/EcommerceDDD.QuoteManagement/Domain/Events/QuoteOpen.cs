namespace EcommerceDDD.QuoteManagement.Domain.Events;

public record class QuoteOpen : DomainEvent
{
    public Guid QuoteId { get; private set; }
    public Guid CustomerId { get; private set; }
    public string CurrencyCode { get; private set; }

    public static QuoteOpen Create(
        Guid quoteId,
        Guid customerId,
        string currencyCode)
    {
        if (quoteId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(quoteId));
        if (customerId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(customerId));
        if (string.IsNullOrEmpty(currencyCode))
            throw new ArgumentOutOfRangeException(nameof(currencyCode));

        return new QuoteOpen(
            quoteId, 
            customerId,
            currencyCode);
    }

    private QuoteOpen(
        Guid quoteId,
        Guid customerId,
        string currencyCode)
    {
        QuoteId = quoteId;
        CustomerId = customerId;
        CurrencyCode = currencyCode;
    }
}
