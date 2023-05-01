namespace EcommerceDDD.Quotes.Application.Quotes.GettingOpenQuote;

public record class GetOpenQuote : IQuery<QuoteViewModel>
{
    public CustomerId CustomerId { get; private set; }
    public string CurrencyCode { get; private set; }

    public static GetOpenQuote Create(
        CustomerId customerId, 
        string currencyCode)
    {
        if (customerId is null)
            throw new ArgumentNullException(nameof(customerId));
        if(string.IsNullOrEmpty(currencyCode))
            throw new ArgumentNullException(nameof(currencyCode));

        return new GetOpenQuote(customerId, currencyCode);
    }

    private GetOpenQuote(
        CustomerId customerId,
        string currencyCode)
    {
        CustomerId = customerId;
        CurrencyCode = currencyCode;
    }
}