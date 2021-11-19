using System.Linq;
using System.Collections.Generic;
using EcommerceDDD.Domain.SeedWork;
using EcommerceDDD.Domain.Customers;

namespace EcommerceDDD.Domain.Quotes;

public class Quote : AggregateRoot<QuoteId>
{        
    public CustomerId CustomerId { get; private set; }
    public IReadOnlyCollection<QuoteItem> Items => _items;        
    public DateTime CreationDate { get; private set; }

    private readonly List<QuoteItem> _items = new List<QuoteItem>();

    public static Quote CreateNew(CustomerId customerId)
    {
        return new Quote(QuoteId.Of(Guid.NewGuid()), customerId);
    }

    public QuoteItem AddItem(QuoteItemProductData productData)
    {
        if (productData == null)
            throw new BusinessRuleException("The quote item must have a product.");

        if (productData.Quantity == 0)
            throw new BusinessRuleException("The product quantity must be at last 1.");

        var quoteItem = new QuoteItem(Guid.NewGuid(), 
            productData.ProductId, 
            productData.Quantity);

        _items.Add(quoteItem);

        return quoteItem;
    }

    public QuoteItem ChangeItem(QuoteItemProductData productData)
    {
        if (productData == null)
            throw new BusinessRuleException("The quote item must have a product.");

        var quoteItem = _items.FirstOrDefault((Func<QuoteItem, bool>)(i => i.ProductId.Value == productData.ProductId.Value));

        if (quoteItem == null)
            quoteItem = AddItem(productData); // Add item
        else if(productData.Quantity == 0) 
            RemoveItem(quoteItem.Id); // Remove Item
        else
            quoteItem.ChangeQuantity(productData.Quantity); // Change item quantity

        return quoteItem;
    }

    public void RemoveItem(Guid quoteItemId)
    {
        var quoteItem = _items.FirstOrDefault(i => i.Id == quoteItemId);
        if (quoteItem == null)
            throw new BusinessRuleException("Invalid quote item.");

        _items.Remove(quoteItem);
    }

    private Quote(QuoteId id, CustomerId customerId)
    {
        Id = id;
        CreationDate = DateTime.Now;

        if (customerId == null)
            throw new BusinessRuleException("The customer is required.");

        CustomerId = customerId;
    }

    // Empty constructor for EF
    private Quote() { }
}
