namespace EcommerceDDD.CustomerManagement.Api.Application.GettingCustomerDetails;

public record class GetCustomerDetails : IQuery<CustomerDetails>
{
    public CustomerId CustomerId { get; private set; }

    public static GetCustomerDetails Create(CustomerId customerId)
    {
        if (customerId is null)
            throw new ArgumentNullException(nameof(customerId));

        return new GetCustomerDetails(customerId);
    }

    private GetCustomerDetails(CustomerId customerId)
    {
        CustomerId = customerId;
    }
}