namespace EcommerceDDD.CustomerManagement.Application.GettingCustomerDetailsById;

public record class GetCustomerDetailsById : IQuery<CustomerDetails>
{
    public CustomerId CustomerId { get; private set; }

    public static GetCustomerDetailsById Create(CustomerId customerId)
    {
        if (customerId is null)
            throw new ArgumentNullException(nameof(customerId));

        return new GetCustomerDetailsById(customerId);
    }

    private GetCustomerDetailsById(CustomerId customerId) => CustomerId = customerId;
}