namespace EcommerceDDD.QuoteManagement.Domain.Commands;

public record class OpenQuote : ICommand
{
    public CustomerId CustomerId { get; private set; }
    public Currency Currency { get; private set; }

    public static OpenQuote Create(
        CustomerId customerId,
        Currency currency)
    {
        if (customerId is null)
            throw new ArgumentNullException(nameof(customerId));
        if (currency is null)
            throw new ArgumentNullException(nameof(currency));

        return new OpenQuote(customerId, currency);
    }

    private OpenQuote(CustomerId customerId, Currency currency)
    {
        CustomerId = customerId;
        Currency = currency;
    }
}