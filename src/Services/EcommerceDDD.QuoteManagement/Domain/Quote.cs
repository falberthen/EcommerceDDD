namespace EcommerceDDD.QuoteManagement.Domain;

public class Quote : AggregateRoot<QuoteId>
{
    public CustomerId CustomerId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ConfirmedAt { get; private set; }
    public DateTime? CanceledAt { get; private set; }
    public QuoteStatus Status { get; private set; }
    public Currency Currency { get; private set; }
    public IList<QuoteItem> Items => _quoteItems;

    private List<QuoteItem> _quoteItems = default!;

    public static Quote OpenQuote(CustomerId customerId, Currency currency)
    {
        if (customerId is null)
            throw new BusinessRuleException("The customer Id is required.");
        if (currency is null)
            throw new BusinessRuleException("The currency is required.");

        var quote = new Quote(customerId, currency);
        return quote;
    }

    public void AddItem(QuoteItemData quoteItemData)
    {
        if(quoteItemData is null)
            throw new BusinessRuleException("Quote item data is required.");

        if (Status == QuoteStatus.Confirmed 
            || Status == QuoteStatus.Cancelled)
            throw new BusinessRuleException("Quote cannot be changed at this point.");

        var quoteItem = Items.FirstOrDefault(p => 
            p.ProductItem.ProductId == quoteItemData.ProductId);

        dynamic @event = quoteItem is null
            ? QuoteItemAdded.Create(quoteItemData)
            : QuoteItemQuantityChanged.Create(quoteItemData);

        AppendEvent(@event);
        Apply(@event);
    }

    public void RemoveItem(ProductId productId)
    {
        if (productId is null)
            throw new BusinessRuleException("The ProductId is required.");

        var quoteItem = Items.FirstOrDefault(i => 
            i.ProductItem.ProductId.Value == productId.Value);

        if (quoteItem is null)
            throw new BusinessRuleException("Quote item not found.");

        var @event = QuoteItemRemoved.Create(
            Id.Value, quoteItem.ProductItem.ProductId.Value
        );

        AppendEvent(@event);
        Apply(@event);
    }

    public void Cancel()
    {
        if (Status != QuoteStatus.Open)
            throw new BusinessRuleException("Quote cannot be canceled at this point.");

        var @event = QuoteCanceled.Create(
            Id.Value);

        AppendEvent(@event);
        Apply(@event);
    }

    public void Confirm()
    {
        if (Status != QuoteStatus.Open)
            throw new BusinessRuleException("Quote cannot be confirmed at this point.");

        if (!Items.Any())
            throw new BusinessRuleException("Quote needs at least 1 item to be confirmed.");

        var @event = QuoteConfirmed.Create(Id.Value);

        AppendEvent(@event);
        Apply(@event);
    }

    private void Apply(QuoteOpen @event)
    {
        Id = QuoteId.Of(@event.QuoteId);
        Status = QuoteStatus.Open;
        CustomerId = CustomerId.Of(@event.CustomerId);
        CreatedAt = @event.Timestamp;
        Currency = Currency.OfCode(@event.CurrencyCode);
        _quoteItems = new List<QuoteItem>();
    }

    private void Apply(QuoteItemAdded @event)
    {
        ProductItem productItem = new ProductItem(
            ProductId.Of(@event.ProductId),
            @event.ProductName,
            Money.Of(@event.ProductPrice, Currency!.Code),
            @event.Quantity);

        _quoteItems.Add(QuoteItem.Create(productItem));
    }

    private void Apply(QuoteItemQuantityChanged @event)
    {
        var quoteItem = Items.FirstOrDefault(p => 
            p.ProductItem.ProductId == ProductId.Of(@event.ProductId));

        quoteItem!.ChangeQuantity(@event.Quantity);
    }

    private void Apply(QuoteItemRemoved @event)
    {
        var quoteItem = Items.First(p => 
            p.ProductItem.ProductId == ProductId.Of(@event.ProductId));

        Items.Remove(quoteItem);
    }

    private void Apply(QuoteCanceled @event)
    {
        Status = QuoteStatus.Cancelled;
        CanceledAt = @event.Timestamp;
    }

    private void Apply(QuoteConfirmed @event)
    {
        Status = QuoteStatus.Confirmed;
        ConfirmedAt = @event.Timestamp;
    }

    private Quote(CustomerId customerId, Currency currency)
    {
        var @event = QuoteOpen.Create(
            Guid.NewGuid(),
            customerId.Value,
            currency.Code);

        AppendEvent(@event);
        Apply(@event);
    }

    // needed for getting it from the event stream
    private Quote() {}
}
