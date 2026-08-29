using EcommerceDDD.Core.CQRS;

namespace EcommerceDDD.CustomerManagement.Application.GettingCustomerEventHistory;

public record class GetCustomerEventHistory : IQuery<IReadOnlyList<CustomerEventHistory>>
{
    public static GetCustomerEventHistory Create()
    {
        return new GetCustomerEventHistory();
    }

    private GetCustomerEventHistory(){ }
}