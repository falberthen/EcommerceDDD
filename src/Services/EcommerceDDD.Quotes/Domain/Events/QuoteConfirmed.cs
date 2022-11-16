using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Quotes.Domain.Events;

public record class QuoteConfirmed : IDomainEvent
{
    public Guid QuoteId { get; private set; }
    public string CurrencyCode { get; set; }
    public DateTime ConfirmedAt { get; private set; }

    public static QuoteConfirmed Create(
        Guid quoteId, 
        string currencyCode, 
        DateTime confirmedAt)
    {
        if (quoteId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(quoteId));
        if (string.IsNullOrEmpty(currencyCode))
            throw new ArgumentOutOfRangeException(nameof(currencyCode));
        if (confirmedAt == DateTime.MinValue)
            throw new ArgumentOutOfRangeException(nameof(confirmedAt));
        
        return new QuoteConfirmed(
            quoteId,
            currencyCode,
            confirmedAt);
    }
    private QuoteConfirmed(
        Guid quoteId,
        string currencyCode,
        DateTime confirmedAt)
    {
        QuoteId = quoteId;
        CurrencyCode = currencyCode;
        ConfirmedAt = confirmedAt;
    }
}