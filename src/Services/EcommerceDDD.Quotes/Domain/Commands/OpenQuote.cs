namespace EcommerceDDD.Quotes.Domain.Commands;

public record class OpenQuote : ICommand
{
    public CustomerId CustomerId { get; private set; }

    public static OpenQuote Create(
        CustomerId customerId)
    {
        if (customerId is null)
            throw new ArgumentNullException(nameof(customerId));

        return new OpenQuote(customerId);
    }

    private OpenQuote(CustomerId customerId)
    {
        CustomerId = customerId;
    }
}