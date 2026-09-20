namespace EcommerceDDD.ServiceClients.Services.CustomerManagement;

public interface ICustomerManagementService
{
    Task<decimal?> GetCustomerStoreCreditAsync(Guid customerId, CancellationToken cancellationToken);

    Task<string?> GetShippingAddressAsync(Guid customerId, CancellationToken cancellationToken);
}
