namespace EcommerceDDD.QuoteManagement.Infrastructure.Projections;

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
    public decimal TotalPrice { get; set; }

    internal void Apply(QuoteOpen @event)
    {
        Id = @event.QuoteId;
        QuoteStatus = QuoteStatus.Open;
        CustomerId = @event.CustomerId;
        CreatedAt = @event.Timestamp;
        CurrencyCode = @event.CurrencyCode;
        Items = new List<QuoteItemDetails>();
    }

    internal void Apply(QuoteItemAdded @event)
    {
        var quoteItem = new QuoteItemDetails(
            @event.ProductId,
            @event.Quantity);

        Items.Add(quoteItem);
    }

    internal void Apply(QuoteItemQuantityChanged @event)
    {
        var quoteItem = Items!
            .FirstOrDefault(p => p.ProductId == @event.ProductId);

        var itemIndex = Items!.IndexOf(quoteItem!);
        quoteItem = quoteItem! with
        {
            Quantity = @event.Quantity,
        };
        Items[itemIndex] = quoteItem;
    }

    internal void Apply(QuoteItemRemoved @event)
    {
        var quoteItem = Items!
            .FirstOrDefault(i => i.ProductId == @event.ProductId);
        Items!.Remove(quoteItem!);
    }

    internal void Apply(QuoteCanceled @event)
    {
        QuoteStatus = QuoteStatus.Cancelled;
        CancelledAt = @event.Timestamp;
    }

    internal void Apply(QuoteConfirmed @event)
    {
        QuoteStatus = QuoteStatus.Confirmed;
        ConfirmedAt = @event.Timestamp;
    }

    public record QuoteItemDetails(Guid ProductId, int Quantity);
}