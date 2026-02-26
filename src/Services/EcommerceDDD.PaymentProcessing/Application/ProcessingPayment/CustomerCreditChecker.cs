namespace EcommerceDDD.PaymentProcessing.Application.ProcessingPayment;

public class CustomerCreditChecker(ICustomerManagementService customerManagementService) : ICustomerCreditChecker
{
	private readonly ICustomerManagementService _customerManagementService = customerManagementService;

	public async Task<bool> CheckIfCreditIsEnoughAsync(CustomerId customerId, Money totalAmount,
		CancellationToken cancellationToken)
	{
		try
		{
			// Checking customer's credit
			var creditLimit = await _customerManagementService
				.GetCustomerCreditLimitAsync(customerId.Value, cancellationToken);

			if (creditLimit is null)
				return false;

			return totalAmount.Amount < creditLimit.Value;
		}
		catch (Exception)
		{
			return false;
		}
	}
}
