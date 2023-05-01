namespace EcommerceDDD.Quotes.Infrastructure.Projections;

public class QuoteDetails
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; } = default;
    public DateTime? CancelledAt { get; set; } = default;
    public QuoteStatus QuoteStatus { get; set; }
    public IList<QuoteItemDetails> Items { get; set; } = default!;
    public string CurrencyCode { get; private set; }

    public void Apply(QuoteOpen created)
    {
        QuoteStatus = QuoteStatus.Open;
        Id = created.QuoteId;
        CustomerId = created.CustomerId;
        CreatedAt = created.CreatedAt;
        Items = new List<QuoteItemDetails>();
    }

    public void Apply(QuoteItemAdded itemAdded)
    {
        Id = itemAdded.QuoteId;
        var quoteItem = new QuoteItemDetails(
            itemAdded.ProductId,
            itemAdded.Quantity);

        Items.Add(quoteItem);
    }

    public void Apply(QuoteItemQuantityChanged itemChanged)
    {
        Id = itemChanged.QuoteId;
        var quoteItem = Items!
            .FirstOrDefault(p => p.ProductId == itemChanged.ProductId);

        var itemIndex = Items!.IndexOf(quoteItem!);
        quoteItem = quoteItem! with
        {
            Quantity = itemChanged.Quantity,
        };
        Items[itemIndex] = quoteItem;
    }

    public void Apply(QuoteItemRemoved itemRemoved)
    {
        Id = itemRemoved.QuoteId;
        var quoteItem = Items!
            .FirstOrDefault(i => i.ProductId == itemRemoved.ProductId);
        Items!.Remove(quoteItem!);
    }

    public void Apply(QuoteCanceled cancelled)
    {
        QuoteStatus = QuoteStatus.Cancelled;
        Id = cancelled.QuoteId;
        CancelledAt = cancelled.CancelledAt;
    }

    public void Apply(QuoteConfirmed confirmed)
    {
        QuoteStatus = QuoteStatus.Confirmed;
        Id = confirmed.QuoteId;
        ConfirmedAt = confirmed.ConfirmedAt;
        CurrencyCode = confirmed.CurrencyCode;
    }

    public record QuoteItemDetails(Guid ProductId, int Quantity);
}