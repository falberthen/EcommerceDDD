namespace EcommerceDDD.PaymentProcessing.Application.ProcessingPayment;

public class CustomerCreditChecker(CustomerManagementClient customerManagementClient) : ICustomerCreditChecker
{
	private readonly CustomerManagementClient _customerManagementClient = customerManagementClient;

	public async Task<bool> CheckIfCreditIsEnoughAsync(CustomerId customerId, Money totalAmount,
		CancellationToken cancellationToken)
	{
		try
		{
			// Checking customer's credit
			var customerRequestBuilder = _customerManagementClient.Api.V2.Customers[customerId.Value];
			var response = await customerRequestBuilder
				.Credit.GetAsync(cancellationToken: cancellationToken);

			if (response is null)
				return false;

			return totalAmount.Amount < Convert.ToDecimal(response.CreditLimit);
		}
		catch (Exception)
		{
			return false;
		}
	}
}
