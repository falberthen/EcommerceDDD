namespace EcommerceDDD.CustomerManagement.Application.GettingCustomerDetails;

public record class GetCustomerDetails : IQuery<CustomerDetails>
{
    public static GetCustomerDetails Create()
    {        
        return new GetCustomerDetails();
    }

    private GetCustomerDetails() { }
}