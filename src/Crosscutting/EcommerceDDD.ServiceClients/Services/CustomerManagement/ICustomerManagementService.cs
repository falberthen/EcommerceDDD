namespace EcommerceDDD.ServiceClients.Services.CustomerManagement;

public interface ICustomerManagementService
{
    Task<decimal?> GetCustomerCreditLimitAsync(Guid customerId, CancellationToken cancellationToken);
}
