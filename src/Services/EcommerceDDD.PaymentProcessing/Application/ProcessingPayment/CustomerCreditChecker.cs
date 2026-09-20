namespace EcommerceDDD.PaymentProcessing.Application.ProcessingPayment;

public class CustomerStoreCreditChecker(ICustomerManagementService customerManagementService) : ICustomerStoreCreditChecker
{
	private readonly ICustomerManagementService _customerManagementService = customerManagementService;

	public async Task<bool> CheckIfStoreCreditIsEnoughAsync(CustomerId customerId, Money totalAmount,
		CancellationToken cancellationToken)
	{		
		var storeCredit = await _customerManagementService
			.GetCustomerStoreCreditAsync(customerId.Value, cancellationToken);

		if (storeCredit is null)
			return false;

		return totalAmount.Amount < storeCredit.Value;
	}
}
