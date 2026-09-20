namespace EcommerceDDD.ServiceClients.Services.CustomerManagement;

public class CustomerManagementService(CustomerManagementClient customerManagementClient) : ICustomerManagementService
{
	private readonly CustomerManagementClient _customerManagementClient = customerManagementClient;

	public async Task<decimal?> GetCustomerStoreCreditAsync(Guid customerId, CancellationToken cancellationToken)
	{
		var response = await _customerManagementClient.Api.V2.Internal.Customers[customerId]
			.Credit.GetAsync(cancellationToken: cancellationToken);

		if (response?.StoreCredit is null)
			return null;

		return Convert.ToDecimal(response.StoreCredit);
	}

	public async Task<string?> GetShippingAddressAsync(Guid customerId, CancellationToken cancellationToken)
	{
		var response = await _customerManagementClient.Api.V2.Internal.Customers[customerId]
			.Details.GetAsync(cancellationToken: cancellationToken);

		return response?.ShippingAddress;
	}
}
