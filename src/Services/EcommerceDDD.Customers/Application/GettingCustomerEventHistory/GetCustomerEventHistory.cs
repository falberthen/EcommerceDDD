namespace EcommerceDDD.Customers.Application.GettingCustomerEventHistory;

public record class GetCustomerEventHistory : IQuery<IList<CustomerEventHistory>>
{
    public CustomerId CustomerId { get; private set; }

    public static GetCustomerEventHistory Create(CustomerId customerId)
    {
        if (customerId is null)
            throw new ArgumentNullException(nameof(customerId));

        return new GetCustomerEventHistory(customerId);
    }

    private GetCustomerEventHistory(CustomerId customerId)
    {
        CustomerId = customerId;
    }
}