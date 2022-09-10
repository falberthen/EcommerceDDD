using EcommerceDDD.Core.Exceptions;
using EcommerceDDD.Core.Domain;
using EcommerceDDD.Quotes.Domain.Events;

namespace EcommerceDDD.Quotes.Domain;

public class Quote : AggregateRoot<QuoteId>
{
    public CustomerId CustomerId { get; private set; }
    public IList<QuoteItem> Items { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }
    public QuoteStatus Status { get; private set; }

    public static async Task<Quote> CreateNew(CustomerId customerId,
        ICustomerQuoteOpennessChecker checker)
    {
        if (!await checker.CanCustomerOpenNewQuote(customerId))
            throw new DomainException("The customer has an open quote already.");
        
        var quote = new Quote(customerId);
        return quote;
    }

    public void AddQuoteItem(QuoteItemData quoteItemData)
    {
        if (Status == QuoteStatus.Confirmed 
            || Status == QuoteStatus.Cancelled)
            throw new DomainException("Quote cannot be changed at this point.");

        var quoteItem = Items
            .FirstOrDefault(p => p.ProductId == quoteItemData.ProductId);

        dynamic @event = quoteItem == null
            ? QuoteItemAdded.Create(quoteItemData)
            : QuoteItemQuantityChanged.Create(quoteItemData);

        AppendEvent(@event);
        Apply(@event);
    }

    public void RemoveQuoteItem(ProductId productId)
    {
        if (productId.Value == Guid.Empty)
            throw new DomainException("The ProductId is missing.");

        var quoteItem = Items
            .FirstOrDefault(i => i.ProductId.Value == productId.Value);

        if (quoteItem == null)
            throw new DomainException("Quote item not found.");

        var @event = QuoteItemRemoved.Create(Id, quoteItem.ProductId);

        AppendEvent(@event);
        Apply(@event);
    }

    public void Cancel()
    {
        if (Status != QuoteStatus.Open)
            throw new DomainException("Quote cannot be canceled at this point.");

        var @event = QuoteCancelled.Create(Id,
            DateTime.UtcNow);

        AppendEvent(@event);
        Apply(@event);
    }

    public void Confirm()
    {
        if (Status != QuoteStatus.Open)
            throw new DomainException("Quote cannot be confirmed at this point.");

        if (!Items.Any())
            throw new DomainException("Quote needs at least 1 item to be confirmed.");

        var @event = QuoteConfirmed.Create(Id,
            DateTime.UtcNow);

        AppendEvent(@event);
        Apply(@event);
    }

    private void Apply(QuoteCreated created)
    {
        Id = created.QuoteId;
        Status = QuoteStatus.Open;
        CustomerId = created.CustomerId;
        CreatedAt = created.CreatedAt;
        Items = new List<QuoteItem>();
    }

    private void Apply(QuoteItemAdded added)
    {
        Items.Add(new QuoteItem(
            added.ProductId,
            added.Quantity));
    }

    private void Apply(QuoteItemQuantityChanged added)
    {
        var quoteItem = Items
            .FirstOrDefault(p => p.ProductId == added.ProductId);

        quoteItem!.ChangeQuantity(added.Quantity);
    }

    private void Apply(QuoteItemRemoved removed)
    {
        var quoteItem = Items
            .FirstOrDefault(p => p.ProductId.Value == removed.ProductId.Value);

        Items.Remove(quoteItem);
    }

    private void Apply(QuoteCancelled cancelled)
    {
        Status = QuoteStatus.Cancelled;
    }

    private void Apply(QuoteConfirmed finished)
    {
        Status = QuoteStatus.Confirmed;
    }

    private Quote(CustomerId customerId)
    {
        if (customerId == null)
            throw new DomainException("The customer is required.");

        var @event = new QuoteCreated(
            QuoteId.Of(Guid.NewGuid()), 
            customerId, 
            DateTime.UtcNow);

        AppendEvent(@event);
        Apply(@event);
    }

    // needed for getting it from the event stream
    private Quote() { }
}
