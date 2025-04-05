namespace EcommerceDDD.CustomerManagement.Application.GettingCustomerEventHistory;

public record class GetCustomerEventHistory : IQuery<IList<CustomerEventHistory>>
{
    public static GetCustomerEventHistory Create()
    {
        return new GetCustomerEventHistory();
    }

    private GetCustomerEventHistory(){ }
}