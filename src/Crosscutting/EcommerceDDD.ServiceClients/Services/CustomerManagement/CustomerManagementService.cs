using EcommerceDDD.ServiceClients.CustomerManagement;

namespace EcommerceDDD.ServiceClients.Services.CustomerManagement;

public class CustomerManagementService(CustomerManagementClient customerManagementClient) : ICustomerManagementService
{
    private readonly CustomerManagementClient _customerManagementClient = customerManagementClient;

    public async Task<decimal?> GetCustomerCreditLimitAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var response = await _customerManagementClient.Api.V2.Internal.Customers[customerId]
            .Credit.GetAsync(cancellationToken: cancellationToken);

        if (response?.CreditLimit is null)
            return null;

        return Convert.ToDecimal(response.CreditLimit);
    }
}
