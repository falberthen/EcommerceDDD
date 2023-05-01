namespace EcommerceDDD.Customers.Application.GettingCreditLimit;

public record class GetCreditLimit : IQuery<CreditLimitModel>
{
    public CustomerId CustomerId { get; private set; }

    public static GetCreditLimit Create(CustomerId customerId)
    {
        if (customerId is null)
            throw new ArgumentNullException(nameof(customerId));

        return new GetCreditLimit(customerId);
    }

    private GetCreditLimit(CustomerId customerId)
    {
        CustomerId = customerId;
    }
}