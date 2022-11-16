using EcommerceDDD.Core.Domain;
using EcommerceDDD.Core.Exceptions;
using EcommerceDDD.Quotes.Domain.Events;

namespace EcommerceDDD.Quotes.Domain;

public class Quote : AggregateRoot<QuoteId>
{
    public CustomerId CustomerId { get; private set; }
    public IList<QuoteItem> Items { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }
    public DateTime ConfirmedAt { get; private set; }
    public QuoteStatus Status { get; private set; }
    public Currency? Currency { get; private set; }

    public static Quote Create(CustomerId customerId)
    {
        if (customerId is null)
            throw new BusinessRuleException("The customer Id is required.");
        
        var quote = new Quote(customerId);
        return quote;
    }

    public void AddItem(QuoteItemData quoteItemData)
    {
        var (QuoteId, ProductId, Quantity) = quoteItemData
            ?? throw new BusinessRuleException("Quote item is required.");

        if (QuoteId is null)
            throw new BusinessRuleException("QuoteId is required.");

        if (ProductId is null)
            throw new BusinessRuleException("ProductId is required.");

        if (Quantity <= 0)
            throw new BusinessRuleException("Quantity is invalid.");

        if (Status == QuoteStatus.Confirmed 
            || Status == QuoteStatus.Cancelled)
            throw new BusinessRuleException("Quote cannot be changed at this point.");

        var quoteItem = Items
            .FirstOrDefault(p => p.ProductId == quoteItemData.ProductId);

        dynamic @event = quoteItem == null
            ? QuoteItemAdded.Create(quoteItemData)
            : QuoteItemQuantityChanged.Create(quoteItemData);

        AppendEvent(@event);
        Apply(@event);
    }

    public void RemoveItem(ProductId productId)
    {
        if (productId is null)
            throw new BusinessRuleException("The ProductId is required.");

        var quoteItem = Items
            .FirstOrDefault(i => i.ProductId.Value == productId.Value);

        if (quoteItem is null)
            throw new BusinessRuleException("Quote item not found.");

        var @event = QuoteItemRemoved.Create(
            Id.Value, 
            quoteItem.ProductId.Value);

        AppendEvent(@event);
        Apply(@event);
    }

    public void Cancel()
    {
        if (Status != QuoteStatus.Open)
            throw new BusinessRuleException("Quote cannot be canceled at this point.");

        var @event = QuoteCanceled.Create(
            Id.Value,
            DateTime.UtcNow);

        AppendEvent(@event);
        Apply(@event);
    }

    public void Confirm(Currency currency)
    {
        if (Status != QuoteStatus.Open)
            throw new BusinessRuleException("Quote cannot be confirmed at this point.");

        if (!Items.Any())
            throw new BusinessRuleException("Quote needs at least 1 item to be confirmed.");

        var @event = QuoteConfirmed.Create(
            Id.Value,
            currency.Code,
            DateTime.UtcNow);

        AppendEvent(@event);
        Apply(@event);
    }

    private void Apply(QuoteOpen created)
    {
        Id = QuoteId.Of(created.QuoteId);
        Status = QuoteStatus.Open;
        CustomerId = CustomerId.Of(created.CustomerId);
        CreatedAt = created.CreatedAt;
        Items = new List<QuoteItem>();
    }

    private void Apply(QuoteItemAdded added)
    {
        Items.Add(QuoteItem.Create(
            ProductId.Of(added.ProductId),
            added.Quantity));
    }

    private void Apply(QuoteItemQuantityChanged added)
    {
        var quoteItem = Items
            .FirstOrDefault(p => p.ProductId == ProductId.Of(added.ProductId));

        quoteItem!.ChangeQuantity(added.Quantity);
    }

    private void Apply(QuoteItemRemoved removed)
    {
        var quoteItem = Items
            .First(p => p.ProductId == ProductId.Of(removed.ProductId));

        Items.Remove(quoteItem);
    }

    private void Apply(QuoteCanceled canceled)
    {
        Status = QuoteStatus.Cancelled;
    }

    private void Apply(QuoteConfirmed confirmed)
    {
        Status = QuoteStatus.Confirmed;
        ConfirmedAt = confirmed.ConfirmedAt;
        Currency = Currency.OfCode(confirmed.CurrencyCode);
    }

    private Quote(CustomerId customerId)
    {
        var @event = QuoteOpen.Create(
            Guid.NewGuid(),
            customerId.Value, 
            DateTime.UtcNow);

        AppendEvent(@event);
        Apply(@event);
    }

    // needed for getting it from the event stream
    private Quote() {}
}
