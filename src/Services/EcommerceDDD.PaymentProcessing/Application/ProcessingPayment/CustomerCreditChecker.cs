namespace EcommerceDDD.PaymentProcessing.Application.ProcessingPayment;

public class CustomerCreditChecker(ApiGatewayClient apiGatewayClient) : ICustomerCreditChecker
{
	private readonly ApiGatewayClient _apiGatewayClient = apiGatewayClient;

	public async Task<bool> CheckIfCreditIsEnoughAsync(CustomerId customerId, Money totalAmount,
		CancellationToken cancellationToken)
	{
		// Checking customer's credit
		var customerRequestBuilder = _apiGatewayClient.Api.V2.Customers[customerId.Value];
		var response = await customerRequestBuilder
			.Credit.GetAsync(cancellationToken: cancellationToken);

		if (response?.Success == false)
			throw new ApplicationLogicException($"An error ocurred trying to obtain the credit limit for customer {customerId.Value}");
		if (response?.Data is null)
			throw new RecordNotFoundException("No data was provided for customer credit limit.");

		// Simply comparing with the customer credit limit
		return totalAmount.Amount < Convert.ToDecimal(response.Data.CreditLimit);
	}
}