using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.Quotes.Domain.Events;
using Marten.Events.Aggregation;

namespace EcommerceDDD.Quotes.Application.GettingOpenQuote;

public class QuoteDetails
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime CreatedAt { get;set; }
    public DateTime? ConfirmedAt { get; set; } = default;
    public DateTime? CancelledAt { get; set; } = default;
    public QuoteStatus QuoteStatus { get; set; }
    public IList<QuoteItemDetails> Items { get; set; } = default!;
    //public decimal TotalPrice { get; set; }

    public void Apply(QuoteCreated created)
    {
        QuoteStatus = QuoteStatus.Open;
        Id = created.QuoteId.Value;
        CustomerId = created.CustomerId.Value;
        CreatedAt = created.CreatedAt;        
        Items = new List<QuoteItemDetails>();
    }

    public void Apply(QuoteItemAdded itemAdded)
    {
        Id = itemAdded.QuoteId.Value;
        var quoteItem = new QuoteItemDetails(
            itemAdded.Id,
            itemAdded.ProductId.Value,
            itemAdded.Quantity);
        
        Items.Add(quoteItem);
    }

    public void Apply(QuoteItemQuantityChanged itemChanged)
    {
        Id = itemChanged.QuoteId.Value;
        var quoteItem = Items!
            .FirstOrDefault(p => p.ProductId == itemChanged.ProductId.Value);

        var itemIndex = Items!.IndexOf(quoteItem!);
        quoteItem = quoteItem! with
        {
            Quantity = itemChanged.Quantity,
        };
        Items[itemIndex] = quoteItem;
    }

    public void Apply(QuoteItemRemoved itemRemoved)
    {
        Id = itemRemoved.QuoteId.Value;
        var quoteItem = Items!
            .FirstOrDefault(i => i.ProductId == itemRemoved.ProductId.Value);
        Items!.Remove(quoteItem!);
    }

    public void Apply(QuoteCancelled cancelled)
    {
        QuoteStatus = QuoteStatus.Cancelled;
        Id = cancelled.QuoteId.Value;
        CancelledAt = cancelled.CancelledAt;
    }

    public void Apply(QuoteConfirmed confirmed)
    {
        QuoteStatus = QuoteStatus.Confirmed;
        Id = confirmed.QuoteId.Value;
        ConfirmedAt = confirmed.ConfirmedAt;
    }

    public record QuoteItemDetails(Guid Id, Guid ProductId, int Quantity);
}

public class QuoteDetailsProjection : SingleStreamAggregation<QuoteDetails>
{
    public QuoteDetailsProjection()
    {
        ProjectEvent<QuoteCreated>((item, @event) => item.Apply(@event));
        ProjectEvent<QuoteItemAdded>((item, @event) => item.Apply(@event));
        ProjectEvent<QuoteItemQuantityChanged>((item, @event) => item.Apply(@event));
        ProjectEvent<QuoteItemRemoved>((item, @event) => item.Apply(@event));
        ProjectEvent<QuoteCancelled>((item, @event) => item.Apply(@event));
        ProjectEvent<QuoteConfirmed>((item, @event) => item.Apply(@event));
    }
}

//https://martendb.io/events/projections/aggregate-projections.html#aggregate-by-stream