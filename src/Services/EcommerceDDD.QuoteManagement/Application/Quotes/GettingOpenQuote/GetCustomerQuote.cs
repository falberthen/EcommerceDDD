namespace EcommerceDDD.QuoteManagement.Application.Quotes.GettingOpenQuote;

public record class GetCustomerQuote : IQuery<QuoteViewModel>
{
    public CustomerId CustomerId { get; private set; }
    public QuoteId? QuoteId { get; private set; }

    public static GetCustomerQuote Create(
        CustomerId customerId,
        QuoteId? quoteId)
    {
        if (customerId is null)
            throw new ArgumentNullException(nameof(customerId));

        return new GetCustomerQuote(customerId, quoteId);
    }

    private GetCustomerQuote(
        CustomerId customerId,
        QuoteId? quoteId)
    {
        CustomerId = customerId;
        QuoteId = quoteId;
    }
}